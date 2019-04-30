using UnityEngine;

public class LightControl : MonoBehaviour
{
    [SerializeField] Light Light;

    public float Intensity
    {
        get => Light.intensity;
        set => Light.intensity = value;
    }
    
    public float SetHue
    {
        set
        {
            Threads.RunOnMain(() =>
            {
                float h, s, v;
                Color.RGBToHSV(Light.color, out h, out s, out v);
                h = value;

                Light.color = Color.HSVToRGB(h, s, v);
            });
        }
    }
}