using System.Collections.Generic;
using System.Linq;
using Eidetic.URack.Base;
using Eidetic.Utility;
using UnityEngine;

namespace Eidetic.URack.Function
{
    public class Oscillator4D : Module
    {
        /// <summary> Master clock rate in beats per minute. </summary>
        [Input, Range(40, 70)] public float Clock = 120f;
        [Input] public int MultipleIndexY = 8;
        [Input] public int MultipleIndexZ = 3;
        [Input] public int MultipleIndexW = 12;

        /// <summary> Sine wave output value. </summary>
        [Output] public float SinX => CalculateSin(Phase.x);
        [Output] public float SinY => CalculateSin(Phase.y);
        [Output] public float SinZ => CalculateSin(Phase.z);
        [Output] public float SinW => CalculateSin(Phase.w);

        /// <summary> Triangle wave output value.</summary>
        [Output] public float TriX => CalculateTri(Phase.x);
        [Output] public float TriY => CalculateTri(Phase.y);
        [Output] public float TriZ => CalculateTri(Phase.z);
        [Output] public float TriW => CalculateTri(Phase.w);

        /// <summary> Sawtooth wave output value. </summary>
        [Output] public float SawX => CalculateSaw(Phase.x);
        [Output] public float SawY => CalculateSaw(Phase.y);
        [Output] public float SawZ => CalculateSaw(Phase.z);
        [Output] public float SawW => CalculateSaw(Phase.w);

        /// <summary> Ramp (inverse-sawtooth) wave output value. </summary>
        [Output] public float RampX => CalculateRamp(Phase.x);
        [Output] public float RampY => CalculateRamp(Phase.y);
        [Output] public float RampZ => CalculateRamp(Phase.z);
        [Output] public float RampW => CalculateRamp(Phase.w);

        public float CalculateSin(float phase) => Mathf.Sin(phase * (Mathf.PI * 2));
        public float CalculateTri(float phase) => (Mathf.PingPong(phase, .5f) * 4) - 1;
        public float CalculateSaw(float phase) => CalculateRamp(phase) * -1;
        public float CalculateRamp(float phase) => ((phase % 1f) * 2) - 1;


        float LastPhaseUpdateTime;
        int LastPhaseUpdateFrame;

        Vector4 phase;

        public Vector4 Phase
        {
            get
            {
                if (Time.frameCount != LastPhaseUpdateFrame)
                {
                    var cycleLengthX = (60 / Clock);
                    var cycleLengthY = (60 / Clock) * Multipliers.ElementAt(MultipleIndexY).Value;
                    var cycleLengthZ = (60 / Clock) * Multipliers.ElementAt(MultipleIndexZ).Value;
                    var cycleLengthW = (60 / Clock) * Multipliers.ElementAt(MultipleIndexW).Value;

                    var phaseDelta = Time.time - LastPhaseUpdateTime;

                    phase.x += phaseDelta / cycleLengthX;
                    phase.y += phaseDelta / cycleLengthY;
                    phase.z += phaseDelta / cycleLengthZ;
                    phase.w += phaseDelta / cycleLengthW;

                    LastPhaseUpdateTime = Time.time;
                    LastPhaseUpdateFrame = Time.frameCount;
                }
                return phase;
            }
        }

        public static Dictionary<string, float> Multipliers = new Dictionary<string, float>()
        {
            { "1/16", (1 / 16f) },
            { "1/16T", (1 / 16f) * (4 / 3f) },
            { "1/8", (1 / 8f) },
            { "1/8T", (1 / 8f) * (4 / 3f) },
            { "1/4", (1 / 4f) },
            { "1/4T", (1 / 4f) * (4 / 3f) },
            { "1/2", (1 / 2f) },
            { "2/3", (2 / 3f) },
            { "1", (1f) },
            { "4/3", (4 / 3f) },
            { "2", (2f) },
            { "4", (4f) },
            { "6", (6f) },
            { "8", (8f) },
            { "12", (12f) },
            { "16", (16f) },
            { "32", (32f) }
        };
    }
}