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
}
