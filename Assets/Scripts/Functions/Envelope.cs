using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Envelope : MonoBehaviour
{

    public bool Trigger = false;
    public float Length = 1f;
    public AnimationCurve Shape = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public bool Running { get; private set; }
    public float StartTime { get; private set; }
    public float Position { get; private set; }
    public float Value { get; private set; }

    [Range(0, 1)]
    public float position = 0f;
    [Range(0, 1)]
    public float value = 0f;

    // Update is called once per frame
    void Update()
    {
        if (Trigger) TriggerEnvelope();
        if (Running)
        {
            Position = (Time.time - StartTime) / Length;
            if (Position > 1) {
                Position = 1;
                Running = false;
            }
            Value = Shape.Evaluate(Position);
        }
        UpdateEditorUI();
    }

    public void TriggerEnvelope()
    {
        StartTime = Time.time;
        Position = 0;
        Running = true;
        Trigger = false;
    }

    [ExecuteInEditMode]
    void UpdateEditorUI() {

        position = Position;
        value = Value;
    }
}
