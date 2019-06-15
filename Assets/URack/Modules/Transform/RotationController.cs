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

        public float MultiplierX = 60f, MultiplierY = 60f, MultiplierZ = 60f;

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

            if (Mathf.Abs(currentX - EulerX) > 0.005f)
                currentX = currentX + (EulerX - currentX) / DampingRate;
            else currentX = EulerX;

            if (Mathf.Abs(currentY - EulerY) > 0.005f)
                currentY = currentY + (EulerY - currentY) / DampingRate;
            else currentY = EulerY;

            if (Mathf.Abs(currentZ - EulerZ) > 0.005f)
                currentZ = currentZ + (EulerZ - currentZ) / DampingRate;
            else currentZ = EulerZ;

            Target.transform.rotation = Quaternion.Euler(currentX * MultiplierX, currentY * MultiplierY, currentZ * MultiplierZ);
        }
    }
}