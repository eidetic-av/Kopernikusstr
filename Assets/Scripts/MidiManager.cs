using UnityEngine;
using MidiJack;

public class MidiManager : MonoBehaviour
{
    public bool DebugInput = false;

    public InputMapper Target;

    void NoteOn(MidiChannel channel, int note, float velocity)
    {
    }

    void NoteOff(MidiChannel channel, int note)
    {
    }

    void Knob(MidiChannel channel, int knobNumber, float knobValue)
    {
        if (DebugInput) {
            Debug.Log(channel + ", " + knobNumber + " = " + knobValue);
        }
        Target.SetInput(knobValue);
    }

    void OnEnable()
    {
        MidiMaster.noteOnDelegate += NoteOn;
        MidiMaster.noteOffDelegate += NoteOff;
        MidiMaster.knobDelegate += Knob;
    }

    void OnDisable()
    {
        MidiMaster.noteOnDelegate -= NoteOn;
        MidiMaster.noteOffDelegate -= NoteOff;
        MidiMaster.knobDelegate -= Knob;
    }
}
