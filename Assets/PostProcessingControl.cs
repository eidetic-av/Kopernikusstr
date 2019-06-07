using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PostProcessingControl : MonoBehaviour
{
    PostProcessingProfile Profile;

    BloomModel.Settings startBloom;
    DepthOfFieldModel.Settings startDepthOfField;

    public void Awake()
    {
        Profile = GetComponent<PostProcessingBehaviour>().profile;
        startBloom = Profile.bloom.settings;
        startDepthOfField = Profile.depthOfField.settings;
    }

    public void OnDestroy()
    {
        Profile.bloom.settings = startBloom;
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

    public float DepthOfFieldFocusDistance
    {
        get => Profile.depthOfField.settings.focusDistance;
        set
        {
            var depthOfFieldSettings = Profile.depthOfField.settings;
            depthOfFieldSettings.focusDistance = value;
            Profile.depthOfField.settings = depthOfFieldSettings;
        }
    }

}
