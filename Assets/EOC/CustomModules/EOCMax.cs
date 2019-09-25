using Eidetic.URack.Base;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Eidetic.URack.Networking
{
    public class EOCMax : Module
    {
        [Output] public float Energy;
        [Output] public bool Onset;
        [Output] public float Angle;
        [Output] public float OnsetAngle;
        [Output] public int Track;

        List<EOCReceiver> Receivers => Rack.ActiveModules.OfType<EOCReceiver>().ToList();
        EOCReceiver Loudest;

        public void Start()
        {
            Loudest = Receivers.First();
        }

        public void EarlyUpdate()
        {
            
            for (int i = 0; i < Receivers.Count(); i++)
            {

            }
        }

        public void LateUpdate()
        {
            Onset = false;
        }
    }
}