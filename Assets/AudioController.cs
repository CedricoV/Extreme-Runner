using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioClip scoreSound;
    public AudioClip fallSound;
    public AudioClip outOfEnergy;

    public AudioSource src;

    #region singleton
    private static AudioController _instance;

    public static AudioController Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion


    public void PlayFallSound()
    {
        src.PlayOneShot(fallSound);
    }

    public void PlayScoreSound()
    {
        src.PlayOneShot(scoreSound);
    }
    public void PlayOutOfEnergySound()
    {
        src.volume = .5f;
        src.PlayOneShot(outOfEnergy);
    }
}
