using System.Collections.Generic;
using System.Linq;
using Eidetic.URack.Base;
using Eidetic.Utility;
using UnityEngine;

namespace Eidetic.URack.Transform
{
    public class RotationController : Module
    {

        [Input] public float XRotation, YRotation, ZRotation;
        [Input] public float DampingRate = 3f;

        public float XMultiply = 180f;

        public GameObject Target { get; private set; }

        float currentX, currentY, currentZ;

        // Update is called once per frame
        internal override void Update()
        {
            if (Target == null) Target = GameObject.Find("Tester");

            var newX = XRotation * XMultiply;

            if (Mathf.Abs(currentX - newX) > 0.005f)
                currentX = currentX + (newX - currentX) / DampingRate;
            else currentX = newX;
            if (Mathf.Abs(currentY - YRotation) > 0.005f)
                currentY = currentY + (YRotation - currentY) / DampingRate;
            else currentY = YRotation;
            if (Mathf.Abs(currentZ - ZRotation) > 0.005f)
                currentZ = currentZ + (ZRotation - currentZ) / DampingRate;
            else currentZ = ZRotation;

            Target.transform.rotation = Quaternion.Euler(currentX, currentY, currentZ);
        }
    }
}