using Eidetic.URack.Base;
using Eidetic.URack.Base.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Eidetic.URack.VFX
{
    public class TextureTable : VFXModule
    {
        public override VisualEffectAsset TemplateAsset => Resources.Load<VisualEffectAsset>("TextureTable");

        [Input, Knob]
        public float ImageA
        {
            set
            {
                var imageIndex = value.Map(-1, 1, 0, Images.Length - 1).RoundToInt();
                var image = Images[imageIndex];
                VisualEffect.SetTexture("ImageA", image);
                VisualEffect.SetUInt("WidthA", (uint)image.width);
                VisualEffect.SetUInt("HeightA", (uint)image.height);
            }
        }

        [Input, Knob]
        public float ImageB
        {
            set
            {
                var imageIndex = value.Map(-1, 1, 0, Images.Length - 1).RoundToInt();
                var image = Images[imageIndex];
                VisualEffect.SetTexture("ImageB", image);
                VisualEffect.SetUInt("WidthB", (uint)image.width);
                VisualEffect.SetUInt("HeightB", (uint)image.height);
            }
        }

        [Input, Knob]
        public float Blend
        {
            set => VisualEffect.SetFloat("Blend", value.Map(0, 1));
        }

        [Input(0, 5, 2, 1), Knob]
        public float ImageScale
        {
            set => VisualEffect.SetFloat("ImageScale", value);
        }

        Texture2D[] Images;
        new public void Start()
        {
            Images = Resources.LoadAll<Texture2D>("Discount");
            base.Start();
        }

        [Input(0, 50, 2, 1), Knob]
        public float Speed
        {
            set => VisualEffect.playRate = value;
        }

        [Input(0, 1, 1, .2f), Knob]
        public float Glow
        {
            set => VisualEffect.SetFloat("Glow", value);
        }

        [Input(0, 1, 3, 0), Knob]
        public float NoiseIntensity
        {
            set => VisualEffect.SetFloat("NoiseIntensity", value);
        }
        [Input(0, 10, 4, 1), Knob]
        public float NoiseFrequency
        {
            set => VisualEffect.SetFloat("NoiseFrequency", value);
        }
        [Input(0, 10, 4, 1), Knob]
        public float NoiseScaleX
        {
            set => VisualEffect.SetFloat("NoiseScaleX", value);
        }
        [Input(0, 10, 4, 1), Knob]
        public float NoiseScaleY
        {
            set => VisualEffect.SetFloat("NoiseScaleY", value);
        }

        [Input(0, 10, 2, 0), Knob]
        public float Warp
        {
            set => VisualEffect.SetFloat("Warp", value);
        }
        [Input(0, 1, 4, 1), Knob]
        public float Shards
        {
            set => VisualEffect.SetFloat("WarpShards", value);
        }
        [Input, Knob]
        public float WarpDirectionX
        {
            set => VisualEffect.SetFloat("WarpDirectionX", value);
        }
        [Input, Knob]
        public float WarpDirectionY
        {
            set => VisualEffect.SetFloat("WarpDirectionY", value);
        }
    }
}