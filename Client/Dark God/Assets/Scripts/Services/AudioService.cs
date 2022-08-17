using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioService : MonoSingleton<AudioService>
{
    public AudioSource bgAudio;
    public AudioSource uiAudio;

    public void ServiceInit()
    {
        Debug.Log("AudioService Loading");
    }

    public void PlayerBGMusic(string name, bool isLoop = true)
    {
        AudioClip audio = ResService.Instance.LoadAudio("ResAudio/" + name, true);
        if(bgAudio.clip == null || bgAudio.clip.name != audio.name)
        {
            bgAudio.clip = audio;
            bgAudio.loop = isLoop;
            bgAudio.Play();
        }
    }

    public void PlayUIAudio(string name)
    {
        AudioClip audio = ResService.Instance.LoadAudio("ResAudio/" + name, true);
        uiAudio.clip = audio;
        uiAudio.Play();
    }
}
