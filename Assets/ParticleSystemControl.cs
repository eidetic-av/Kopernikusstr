using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemControl : MonoBehaviour
{
    public ParticleSystem ParticleSystem;

    public void Awake()
    {
        if (ParticleSystem == null) ParticleSystem = GetComponent<ParticleSystem>();
    }

    public float NoiseIntensity
    {
        get => ParticleSystem.noise.strength.constant;
        set
        {
            var noiseModule = ParticleSystem.noise;
            noiseModule.strength = new ParticleSystem.MinMaxCurve(value);
        }
    }

    public float BillboardStretch
    {
        set
        {
            var rendererModule = ParticleSystem.GetComponent<ParticleSystemRenderer>();
            rendererModule.lengthScale = value;
            rendererModule.cameraVelocityScale = value;
            rendererModule.velocityScale = value;
        }
    }

    public float TintHue
    {
        set
        {
            var colorBySpeedModule = ParticleSystem.colorBySpeed;
            Color startingColor = colorBySpeedModule.color.gradient.colorKeys[0].color;

            float h, s, v;
            Color.RGBToHSV(startingColor, out h, out s, out v);

            h = value;

            colorBySpeedModule.range = new Vector2(1, 1);

            var gradient = new Gradient();
            gradient.colorKeys = new GradientColorKey[] { new GradientColorKey(Color.HSVToRGB(h, s, v), 0) };

            var color = colorBySpeedModule.color;
            color.gradient = gradient;

            colorBySpeedModule.color = color;
        }
    }

    public float TintSaturation
    {
        set
        {
            var colorBySpeedModule = ParticleSystem.colorBySpeed;
            Color startingColor = colorBySpeedModule.color.gradient.colorKeys[0].color;

            float h, s, v;
            Color.RGBToHSV(startingColor, out h, out s, out v);

            s = value;

            colorBySpeedModule.range = new Vector2(1, 1);

            var gradient = new Gradient();
            gradient.colorKeys = new GradientColorKey[] { new GradientColorKey(Color.HSVToRGB(h, s, v), 0) };

            var color = colorBySpeedModule.color;
            color.gradient = gradient;

            colorBySpeedModule.color = color;
        }
    }

    public float TintValue
    {
        set
        {
            var colorBySpeedModule = ParticleSystem.colorBySpeed;
            Color startingColor = colorBySpeedModule.color.gradient.colorKeys[0].color;

            float h, s, v;
            Color.RGBToHSV(startingColor, out h, out s, out v);

            v = value;

            colorBySpeedModule.range = new Vector2(1, 1);

            var gradient = new Gradient();
            gradient.colorKeys = new GradientColorKey[] { new GradientColorKey(Color.HSVToRGB(h, s, v), 0) };

            var color = colorBySpeedModule.color;
            color.gradient = gradient;

            colorBySpeedModule.color = color;
        }
    }
}
