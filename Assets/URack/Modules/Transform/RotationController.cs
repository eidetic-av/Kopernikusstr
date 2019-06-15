using System.Collections.Generic;
using System.Linq;
using Eidetic.URack.Base;
using Eidetic.Utility;
using UnityEngine;

namespace Eidetic.URack.Transform
{
    public class RotationController : Module
    {

        [Input] public float EulerX, EulerY, EulerZ;
        [Input] public float DampingRate = 3f;

        public float MultiplierX, MultiplierY, MultiplierZ = 60f;

        public GameObject Target { get; private set; }

        float currentX, currentY, currentZ;

        internal override void Start()
        {
            Target = GameObject.Find("Tester");
        }

        // Update is called once per frame
        internal override void Update()
        {
            if (Target == null) return;

            var newX = EulerX * MultiplierX;
            var newY = EulerY * MultiplierY;
            var newZ = EulerZ * MultiplierZ;

            if (Mathf.Abs(currentX - newX) > 0.005f)
                currentX = currentX + (newX - currentX) / DampingRate;
            else currentX = newX;

            if (Mathf.Abs(currentY - newY) > 0.005f)
                currentY = currentY + (newY - currentY) / DampingRate;
            else currentY = newY;

            if (Mathf.Abs(currentZ - newZ) > 0.005f)
                currentZ = currentZ + (newZ - currentZ) / DampingRate;
            else currentZ = newZ;

            Target.transform.rotation = Quaternion.Euler(currentX, currentY, currentZ);
        }
    }
}