using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] Camera Camera;

    public float FieldOfView
    {
        get => Camera.fieldOfView;
        set => Camera.fieldOfView = value;
    }
    public float FarClip
    {
        get => Camera.farClipPlane;
        set => Camera.farClipPlane = value;
    }
}
