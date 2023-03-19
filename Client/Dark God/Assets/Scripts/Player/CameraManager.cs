using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Cinemachine;

namespace Game.Manager
{
    public class CameraManager : MonoBehaviour
    {
        public CinemachineBrain brain;

        public CinemachineFreeLook freeLookCam;

        public void SetTarget(Transform target)
        {
            freeLookCam.LookAt = target;
            freeLookCam.Follow = target;
        }
    }
}
