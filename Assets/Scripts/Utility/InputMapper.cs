using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InputMapper : MonoBehaviour
{
    public float Input;

    public ComplexFloat Output;


    [SerializeField]
    public UpdateEvent Callback;

    public bool DebounceCallback = true;
    public float DebounceTime = 0.05f;
    public bool CallbackOnZero = true;
    private float LastCallbackTime;

    private void Start()
    {    
    }

    void OnEnable()
    {
    }

    void Update() 
    {
        Output.Input = Input;
        Output.Update();

        if (Callback != null)
        {
            if (!DebounceCallback ||
            (DebounceCallback && (Time.time - LastCallbackTime) > DebounceTime))
            {
                if (!CallbackOnZero && Output.Value == 0)
                    return;
                    
                Callback.Invoke((float)Output.Value);
                LastCallbackTime = Time.time;
            }
        }
    }

    public void SetInput(float input)
    {
        Input = input;
    }


    [System.Serializable]
    public class UpdateEvent : UnityEvent<float> { }
}
