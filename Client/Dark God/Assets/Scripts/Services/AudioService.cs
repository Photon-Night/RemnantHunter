using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PEProtocol;
using Game.Common;

public class AudioService : MonoSingleton<AudioService>, IService
{
    public AudioSource bgAudio;
    public AudioSource uiAudio;

    private Dictionary<string, AudioSource> entityAudioDic;

    public void ServiceInit()
    {
        PECommon.Log("AudioService Loading");
    }

    public void PlayBGMusic(string name, bool isLoop = true)
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
        AudioClip audio = ResService.Instance.LoadAudio("ResAudio/" + name, false);
        uiAudio.clip = audio;
        uiAudio.Play();
    }

    public void AddAudio(string name, AudioSource audio)
    {
        if(!entityAudioDic.ContainsKey(name))
        {
            entityAudioDic.Add(name, audio);
        }
    }

    public void ClearAudioDic()
    {
        entityAudioDic.Clear();
    }

    public void PlayEntityAudio(string name, string path)
    {
        AudioSource audioSource;
        if (!entityAudioDic.TryGetValue(name, out audioSource))
        {
            PECommon.Log("You Have Not Entity AudioSource Named " + name);
            return;
        }
        
        AudioClip audio = ResService.Instance.LoadAudio("ResAudio/" + path, true);
        audioSource.clip = audio;
        audioSource.Play();
    }

    public void PlayeEntityAudioByAudioSource(AudioSource audioSource, string path)
    {
        AudioClip audio = ResService.Instance.LoadAudio("ResAudio/" + path, true);
        audioSource.clip = audio;
        audioSource.Play();
    }
    public void StopBGAudio()
    {
        bgAudio.Stop();
    }
}
