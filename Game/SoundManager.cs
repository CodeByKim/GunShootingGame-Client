using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour 
{
    public static SoundManager Instance;

    public AudioClip uiButtonClick;
    public AudioClip shoot;
    public AudioClip mobDamaged;
    public AudioClip mobDie;
    public AudioClip playerDie;

    private AudioSource audioSource;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        Instance = this;
    }

    void Start () 
    {
        audioSource = GetComponent<AudioSource>();	
	}	

    public void PlayUIButtonClick()
    {
        audioSource.PlayOneShot(uiButtonClick);
    }

    public void PlayShootSound()
    {
        audioSource.PlayOneShot(shoot);
    }

    public void MobDamaged()
    {
        audioSource.PlayOneShot(mobDamaged);
    }

    public void MobDie()
    {
        audioSource.PlayOneShot(mobDie);
    }

    public void PlayerDie()
    {
        audioSource.PlayOneShot(playerDie);
    }
}
