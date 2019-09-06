using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;
using System.Linq;
using Eidetic.URack.Base;
using Eidetic.URack.CustomTypes;
using static UnityEngine.SystemInfo;


namespace Eidetic.URack
{

    public class PointCloudToVFX : MonoBehaviour
    {
        public VisualEffect VisualEffect;

        public Texture2D PositionTexture;
        public Texture2D ColorTexture;

        public PointCloud PointCloudInput;

        void Start()
        {
            PositionTexture = new Texture2D(maxTextureSize, 1, TextureFormat.RGBAFloat, false);
            ColorTexture = new Texture2D(maxTextureSize, 1, TextureFormat.RGBAFloat, false);
        }

        void Update()
        {
            var reciever = LiveScanReceiver.Instance;
            if (reciever != null && reciever.Connected && PointCloudInput != null)
            {

                NativeArray<Color> positionData = PositionTexture.GetRawTextureData<Color>();
                NativeArray<Color> colorData = ColorTexture.GetRawTextureData<Color>();

                for (int i = 0; i < maxTextureSize; i++)
                {
                    var point = PointCloudInput.Points[i];
                    positionData[i] = point.Position.ToColor();
                    colorData[i] = point.Color;
                }

                PositionTexture.Apply();
                ColorTexture.Apply();

                VisualEffect.SetTexture("PositionTexture", PositionTexture);
                VisualEffect.SetTexture("ColorTexture", ColorTexture);
                //Texture2D colorTexture;
                //colorTexture = Texture2D.whiteTexture;
                //reciever.Modules.First().PointCloud
                //    .GetTextures(out positionTexture, out colorTexture);
                //VisualEffect.SetTexture("ColorTexture", colorTexture);
            }
        }
    }
}