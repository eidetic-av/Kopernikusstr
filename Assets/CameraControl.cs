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
}
