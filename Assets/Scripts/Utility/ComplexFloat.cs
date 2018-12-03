using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;
using System;

[Serializable]
public class ComplexFloat
{
    public float Input;
    public float InputPower = 1f;
    public bool PseudoBoolean = false;
    public bool AbsoluteValue = false;
    public bool Wrap;
    public Vector2 InputWrap = new Vector2(0, 1);
    public bool Clamp;
    public Vector2 InputClamp = new Vector2(0, 1);
    public Vector2 OutputClamp = new Vector2(0, 1);
    public Vector2 InputMap = new Vector2(0, 1);
    public Vector2 OutputMap = new Vector2(0, 1);
    public bool DampChanges = false;
    public float DampRate = 15f;
    public bool Debounce = false;
    public float DebounceTime = 0.01f;

    private float LastInput;
    private float LastInputUpdateTime;
    private float NewValue;

    public float Value;

    public bool PrintInput = false;
    public bool PrintOutput = false;

    public void Update()
    {
        if (PseudoBoolean)
        {
            if (Input > 0) Input = 1;
        }
        if (AbsoluteValue)
        {
            Input = Mathf.Abs(Input);
        }
        if (Input != LastInput)
        {
            if (!Debounce ||
            (Debounce && (Time.time - LastInputUpdateTime) > DebounceTime))
            {
                if (Wrap)
                {
                    if (Input > InputWrap.y)
                        Input = InputWrap.x + (Input - InputWrap.y);
                    else if (Input < InputWrap.x)
                        Input = InputWrap.y - Mathf.Abs(Input - InputWrap.x);
                }
                if (Clamp)
                {
                    Input = Input.Clamp(InputClamp.x, InputClamp.y);
                }
                var inputRaised = Mathf.Pow(Input, InputPower);
                var mapped = inputRaised.Map(InputMap.x, InputMap.y, OutputMap.x, OutputMap.y);
                if (Clamp)
                {
                    NewValue = mapped.Clamp(OutputClamp.x, OutputClamp.y);
                }
                else
                {
                    NewValue = mapped;
                }
                LastInputUpdateTime = Time.time;
                if (!DampChanges) Value = NewValue;
            }
        }
        if (DampChanges)
        {
            if (NewValue != Value)
            {
                Value = Value + (NewValue - Value) / (DampRate);
                if (AbsoluteValue)
                    Value = Mathf.Abs(Value);
            }
        }

        LastInput = Input;

        if (PrintInput)
        {
            Debug.Log(Input);
        }
        if (PrintOutput)
        {
            Debug.Log(Value);
        }
    }

    public ComplexFloat Clone()
    {
        return MemberwiseClone() as ComplexFloat;
    }
}
