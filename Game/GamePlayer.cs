using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class GamePlayer : MonoBehaviour 
{
    public float moveSpeed = 5;
    public GameObject bulletPrefab;
    public Transform gunMount;
    public Action<GamePlayer> OnDeath;
    public Scrollbar hpSlider;

    private Camera _viewCamera;
    private Rigidbody _rigidBody;
    private bool _enableAttack;
    private float _attackTimer;
    private int _life;
    private int _killCount;
    private Player _player;

    public Player Player
    {
        get { return _player; }
    }

    public int KillCount
    {
        get { return _killCount; }
        set { _killCount = value; }
    }

    public int Life
    {
        get { return _life; }
        set { value = _life; }
    }

    public void Initialize(Player player, Action<GamePlayer> deathEvent)
    {
        _player = player;

        _viewCamera = Camera.main;
        _rigidBody = GetComponent<Rigidbody>();
        _attackTimer = 0.2f;
        _life = 10;
        _killCount = 0;
        _enableAttack = true;
        OnDeath = deathEvent;
        hpSlider.size = 1;

        transform.Find("Player ID Text").GetComponent<TextMeshPro>().text = _player.PlayerID;

        if(player.PlayerID == GameFramework.Instance.MyPlayer.PlayerID)
        {
            gameObject.name = "My Player";
        }
        else
        {
            gameObject.name = "Other Player";
        }
    }

    private void Update()
    {
        if(Player.IsMine())
        {
            Ray ray = _viewCamera.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            float rayDistance;

            if (groundPlane.Raycast(ray, out rayDistance))
            {
                Vector3 point = ray.GetPoint(rayDistance);
                Vector3 heightCorrectedPoint = new Vector3(point.x, transform.position.y, point.z);
                transform.LookAt(heightCorrectedPoint);
            }

            if (Input.GetMouseButton(0))
            {
                if (_enableAttack)
                {
                    Shoot();
                    _enableAttack = false;
                }
            }

            ResetAttackCoolTime();
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.IsGameStart)
        {
            if (Player.IsMine())
            {
                Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
                Vector3 moveVelocity = moveInput.normalized * moveSpeed;
                _rigidBody.MovePosition(_rigidBody.position + moveVelocity * Time.fixedDeltaTime);

                CLIENTtoSERVER_GamePlayerMovePacketData data = new CLIENTtoSERVER_GamePlayerMovePacketData();

                data.playerKey = GameFramework.Instance.MyPlayer.PlayerKey;
                data.gameKey = GameManager.Instance.GameKey;

                data.posX = transform.position.x;
                data.posY = transform.position.y;
                data.posZ = transform.position.z;

                data.angularX = transform.eulerAngles.x;
                data.angularY = transform.eulerAngles.y;
                data.angularZ = transform.eulerAngles.z;

                if (!GameManager.Instance.IsHost)
                    NetworkManager.Instance.SendCommand((int)PROTOCOL.GAME_PLAYER_MOVE, (int)EXTRA.CLIENT_TO_SERVER, data);
                else
                    NetworkManager.Instance.SendCommand((int)PROTOCOL.GAME_PLAYER_MOVE, (int)EXTRA.HOST_TO_SERVER, data);
            }
        }
    }

    public void Shoot()
    {
        if(GameManager.Instance.IsGameStart)
        {
            Vector3 angle = new Vector3(0, transform.eulerAngles.y, 0);
            Bullet bullet = Instantiate(bulletPrefab, gunMount.position, Quaternion.Euler(angle)).GetComponent<Bullet>();
            bullet.Owner = this;
            SoundManager.Instance.PlayShootSound();

            CLIENTtoSERVER_GamePlayerAttackPacketData data = new CLIENTtoSERVER_GamePlayerAttackPacketData();
            data.gameKey = GameManager.Instance.GameKey;
            data.playerKey = GameFramework.Instance.MyPlayer.PlayerKey;

            if (!GameManager.Instance.IsHost)
                NetworkManager.Instance.SendCommand((int)PROTOCOL.GAME_PLAYER_ATTACK, (int)EXTRA.CLIENT_TO_SERVER, data);
            else
                NetworkManager.Instance.SendCommand((int)PROTOCOL.GAME_PLAYER_ATTACK, (int)EXTRA.HOST_TO_SERVER, data);
        }
    }

    public void RemotePlayerShoot()
    {
        Vector3 angle = new Vector3(0, transform.eulerAngles.y, 0);
        Bullet bullet = Instantiate(bulletPrefab, gunMount.position, Quaternion.Euler(angle)).GetComponent<Bullet>();
        bullet.Owner = this;
    }

    private void ResetAttackCoolTime()
    {
        if (_attackTimer > 0 && !_enableAttack)
            _attackTimer -= Time.deltaTime;
        else
        {
            _enableAttack = true;
            _attackTimer = 0.2f;
        }
    }

    public void DecreaseHealth()
    {
        _life -= 1;

        if(_life > 0)
        {
            hpSlider.size -= 0.1f;
            //GUI 처리
        }
    }

    public void IncreaseHealth()
    {
        if(_life < 10)
        {
            _life += 1;
            hpSlider.size += 0.1f;
        }
    }

    public void SetHealth(int hp)
    {
        _life = hp;
        hpSlider.size = _life * 0.1f;
    }

    public void Hit()
    {
        if(GameManager.Instance.IsGameStart)
        {
            if (GameManager.Instance.IsHost)
            {
                DecreaseHealth();

                if (_life <= 0)
                {
                    CLIENTtoSERVER_GamePlayerDiePacketData data = new CLIENTtoSERVER_GamePlayerDiePacketData();

                    data.playerKey = GameFramework.Instance.MyPlayer.PlayerKey;
                    data.gameKey = GameManager.Instance.GameKey;
                    data.playerID = Player.PlayerID;

                    NetworkManager.Instance.SendCommand((int)PROTOCOL.GAME_PLAYER_DIE, (int)EXTRA.HOST_TO_SERVER, data);

                    if(Player.IsMine())
                    {
                        GameGUI.Instance.ShowGameOver();
                        SoundManager.Instance.PlayerDie();
                    }                        

                    if (OnDeath != null)
                        OnDeath(this);
                }
                else
                {
                    CLIENTtoSERVER_GamePlayerDamagedPacketData data = new CLIENTtoSERVER_GamePlayerDamagedPacketData();

                    data.playerKey = GameFramework.Instance.MyPlayer.PlayerKey;
                    data.gameKey = GameManager.Instance.GameKey;
                    data.playerID = Player.PlayerID;
                    data.hp = _life;

                    NetworkManager.Instance.SendCommand((int)PROTOCOL.GAME_PLAYER_DAMAGED, (int)EXTRA.HOST_TO_SERVER, data);                    
                }
            }
        }
    }
}
