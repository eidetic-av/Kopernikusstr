using UnityEngine;

public class LightControl : MonoBehaviour
{
    [SerializeField] Light Light;

    public float Intensity
    {
        get => Light.intensity;
        set => Light.intensity = value;
    }
}