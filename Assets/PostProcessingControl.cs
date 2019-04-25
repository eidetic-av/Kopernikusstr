using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PostProcessingControl : MonoBehaviour
{
    [SerializeField] PostProcessingProfile Profile;

    BloomModel.Settings startingBloom;

    public void Awake()
    {
        startingBloom = Profile.bloom.settings;
    }

    public void OnDestroy()
    {
        Profile.bloom.settings = startingBloom;
    }

    public float BloomIntensity
    {
        get => Profile.bloom.settings.bloom.intensity;
        set
        {
            var bloomSettings = Profile.bloom.settings;
            bloomSettings.bloom.intensity = value;
            Profile.bloom.settings = bloomSettings;
        }
    }

}
