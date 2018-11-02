using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour 
{
    public enum State
    {
        IDLE,
        MOVING,
        ATTACKING
    }

    public int life;
    public Action<Enemy> DieEvent;
    public Scrollbar hpSlider;

    private State _state;
    private NavMeshAgent _navMesh;
    private Transform _target;
    private float _attackTimer;
    private bool _enableAttack;
    private string _enemyID;

    public string EnemyID
    {
        get { return _enemyID; }
        set { _enemyID = value; }
    }

    public void Initialize()
    {
        _state = State.MOVING;
        _navMesh = GetComponent<NavMeshAgent>();

        SetTargetUsingDistance();

        _attackTimer = 0.5f;
        _enableAttack = true;
        hpSlider.size = 1;
        StartCoroutine(UpdatePath());
    }

    private void Update()
    {
        if(GameManager.Instance.IsHost)
        {
            if (IsAttack())
            {
                _state = State.ATTACKING;
                _navMesh.enabled = false;

                transform.GetChild(0).GetComponent<Animation>().CrossFade("Enemy Attack");
                _target.GetComponent<GamePlayer>().Hit();

                if(_target == null)
                    SetTargetUsingDistance();

                _enableAttack = false;

                if(GameManager.Instance.IsGameStart)
                {
                    CLIENTtoSERVER_EnemyAttackPacketData data = new CLIENTtoSERVER_EnemyAttackPacketData();
                    data.playerKey = GameFramework.Instance.MyPlayer.PlayerKey;
                    data.gameKey = GameManager.Instance.GameKey;
                    data.enemyID = EnemyID;
                    NetworkManager.Instance.SendCommand((int)PROTOCOL.ENEMY_ATTACK, (int)EXTRA.HOST_TO_SERVER, data);
                }
            }
            else
            {
                _navMesh.enabled = true;
                _state = State.MOVING;
            }

            ResetAttackCoolTime();
        }
    }

    private void FixedUpdate()
    {
       if (GameManager.Instance.IsHost)
        {
            if (GameManager.Instance.IsGameStart)
            {
                CLIENTtoSERVER_EnemyMovePacketData data = new CLIENTtoSERVER_EnemyMovePacketData();

                data.playerKey = GameFramework.Instance.MyPlayer.PlayerKey;
                data.gameKey = GameManager.Instance.GameKey;
                data.enemyID = EnemyID;

                data.posX = transform.position.x;
                data.posY = transform.position.y;
                data.posZ = transform.position.z;

                data.angularX = transform.eulerAngles.x;
                data.angularY = transform.eulerAngles.y;
                data.angularZ = transform.eulerAngles.z;

                NetworkManager.Instance.SendCommand((int)PROTOCOL.ENEMY_MOVE, (int)EXTRA.HOST_TO_SERVER, data);
            }
        }
    }

    private void SetTargetUsingDistance()
    {
        float distance = 100;
        for (int i = 0; i < GameManager.Instance.GamePlayers.Count; i++)
        {
            float newDistance = Vector3.Distance(GameManager.Instance.GamePlayers[i].transform.position, transform.position);
            if (newDistance < distance)
            {
                distance = newDistance;
                _target = GameManager.Instance.GamePlayers[i].transform;
            }
        }
    }

    private bool IsAttack()
    {
        if(_target != null)
            if (Vector3.Distance(transform.position, _target.position) < 2.5f)
                if (_enableAttack)
                    return true;

        return false;
    }

    private void ResetAttackCoolTime()
    {
        if (_attackTimer > 0 && !_enableAttack)
            _attackTimer -= Time.deltaTime;
        else
        {
            _enableAttack = true;
            _attackTimer = 0.5f;
        }
    }

    private IEnumerator UpdatePath()
    {
        float refreshRate = 0.5f;

        while (true)
        {
            if(GameManager.Instance.IsGameStart)
            {
                if (_state == State.MOVING)
                {
                    SetTargetUsingDistance();
                    _navMesh.SetDestination(_target.position);
                }
            }

            yield return new WaitForSeconds(refreshRate);
        }
    }

    public void DecreaseHealth()
    {
        life -= 1;

        if (life >= 0)
        {
            //GUI 처리
            hpSlider.size -= 0.2f;
        }
        SoundManager.Instance.MobDamaged();
    }

    public void Hit(GamePlayer attackPlayer)
    {
        if (GameManager.Instance.IsHost)
        {
            DecreaseHealth();
            
            if (life <= 0)
            {
                SoundManager.Instance.MobDie();
                DieEvent(this);
                attackPlayer.KillCount += 1;

                //Enemy Die 패킷 서버로 전송
                CLIENTtoSERVER_EnemyDiePacketData data = new CLIENTtoSERVER_EnemyDiePacketData();

                data.playerKey = GameFramework.Instance.MyPlayer.PlayerKey;
                data.gameKey = GameManager.Instance.GameKey;
                data.enemyID = EnemyID;

                NetworkManager.Instance.SendCommand((int)PROTOCOL.ENEMY_DIE, (int)EXTRA.HOST_TO_SERVER, data);
                Destroy(gameObject);
            }
            else
            {
                //Enemy Damaged 패킷 서버로 전송
                CLIENTtoSERVER_EnemyDamagedPacketData data = new CLIENTtoSERVER_EnemyDamagedPacketData();

                data.playerKey = GameFramework.Instance.MyPlayer.PlayerKey;
                data.gameKey = GameManager.Instance.GameKey;
                data.enemyID = EnemyID;

                NetworkManager.Instance.SendCommand((int)PROTOCOL.ENEMY_DAMAGED, (int)EXTRA.HOST_TO_SERVER, data);
            }
        }
    }
}
