using Game.Common;
using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Game.Service
{
    public class DirectorService : MonoSingleton<DirectorService>, IService
    {
        [SerializeField]
        private PlayableDirector director;
        ResService resSvc;
        
        public TimelineAsset CurrentTimelineAsset { get; private set;}       
        public void ServiceInit()
        {
            resSvc = ResService.Instance;          
            director = GetComponentInChildren<PlayableDirector>();
            PECommon.Log($"DirectorService Loading");
        }

        public void PlayTimeLine(string name)
        {
            CurrentTimelineAsset = resSvc.GetTimelineAsset(name);
            director.playableAsset = CurrentTimelineAsset;
            director.Play();
        }

        public void RegisterTimelineEvent(TimelineEventType type, System.Action<PlayableDirector> action)
        {
            switch (type)
            {
                case TimelineEventType.None:
                    break;
                case TimelineEventType.Played:                   
                    director.played += action;
                    break;
                case TimelineEventType.Stopped:
                    director.stopped += action;
                    break;
                default:
                    break;
            }

        }

        public void UnRegisterTimelineEvent(TimelineEventType type, System.Action<PlayableDirector> action)
        {
            switch (type)
            {
                case TimelineEventType.None:
                    break;
                case TimelineEventType.Played:
                    director.played -= action;
                    break;
                case TimelineEventType.Stopped:
                    director.stopped -= action;
                    break;
                default:
                    break;
            }
        }



    }

    public enum TimelineEventType
    {
        None = 0,
        Played = 1,
        Stopped = 2,
    }

}


