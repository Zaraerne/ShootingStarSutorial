using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : PresistentSingleton<AudioManager>
{
    [SerializeField] AudioSource sFXPlayer;
    const float MIN_PITCH = 0.9f;
    const float MAX_PITCH = 1.1f;


    public void PlayerSFX(AudioData audioData)
    {
        sFXPlayer.PlayOneShot(audioData.audioClip, audioData.volume);
    }

    public void PlayerRandomSFX(AudioData audioData)
    {
        sFXPlayer.pitch = Random.Range(MIN_PITCH, MAX_PITCH);
        PlayerSFX(audioData);
    }
    public void PlayerRandomSFX(AudioData[] audioData)
    {
        PlayerRandomSFX(audioData[Random.Range(0, audioData.Length)]);
    }

}

[System.Serializable]
public class AudioData
{
    public AudioClip audioClip;
    public float volume;
}
