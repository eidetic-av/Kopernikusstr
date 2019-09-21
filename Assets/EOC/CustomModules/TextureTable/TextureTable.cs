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

        [Input, TextEntry] public string FolderName = "Discount";

        int imageIndex;
        [Input, Knob]
        public float Image
        {
            get => imageIndex;
            set
            {
                imageIndex = value.Map(-1, 1, 0, Images.Length - 1).RoundToInt();
                var image = Images[imageIndex];
                VisualEffect.SetTexture("Image", image);
                VisualEffect.SetInt("Width", image.width);
                VisualEffect.SetInt("Height", image.height);
            }
        }

        Texture2D[] images;
        Texture2D[] Images => images ?? (images = Resources.LoadAll<Texture2D>("Discount"));
    }
}