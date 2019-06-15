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
        [Input, Range(0, 12)] public int MultipleIndexY = 10;
        [Input, Range(0, 12)] public int MultipleIndexZ = 10;
        [Input, Range(0, 12)] public int MultipleIndexW = 10;

        /// <summary> Sine wave output value. </summary>
        [Output] public float SinX => CalculateSin();
        [Output] public float SinY => CalculateSin(Multipliers.ElementAt(MultipleIndexY).Value);
        [Output] public float SinZ => CalculateSin(Multipliers.ElementAt(MultipleIndexZ).Value);
        [Output] public float SinW => CalculateSin(Multipliers.ElementAt(MultipleIndexW).Value);

        /// <summary> Triangle wave output value.</summary>
        [Output] public float TriX => CalculateTri();
        [Output] public float TriY => CalculateTri(Multipliers.ElementAt(MultipleIndexY).Value);
        [Output] public float TriZ => CalculateTri(Multipliers.ElementAt(MultipleIndexZ).Value);
        [Output] public float TriW => CalculateTri(Multipliers.ElementAt(MultipleIndexW).Value);

        /// <summary> Sawtooth wave output value. </summary>
        [Output] public float SawX => CalculateSaw();
        [Output] public float SawY => CalculateSaw(Multipliers.ElementAt(MultipleIndexY).Value);
        [Output] public float SawZ => CalculateSaw(Multipliers.ElementAt(MultipleIndexZ).Value);
        [Output] public float SawW => CalculateSaw(Multipliers.ElementAt(MultipleIndexW).Value);

        /// <summary> Ramp (inverse-sawtooth) wave output value. </summary>
        [Output] public float RampX => CalculateRamp();
        [Output] public float RampY => CalculateRamp(Multipliers.ElementAt(MultipleIndexY).Value);
        [Output] public float RampZ => CalculateRamp(Multipliers.ElementAt(MultipleIndexZ).Value);
        [Output] public float RampW => CalculateRamp(Multipliers.ElementAt(MultipleIndexW).Value);

        public float CalculateSin(float multiplier = 1) => Mathf.Sin((Phase * multiplier) * (Mathf.PI * 2));
        public float CalculateTri(float multiplier = 1) => (Mathf.PingPong((Phase * multiplier), .5f) * 4) - 1;
        public float CalculateSaw(float multiplier = 1) => CalculateRamp(multiplier) * -1;
        public float CalculateRamp(float multiplier = 1) => (((Phase * multiplier) % 1f) * 2) - 1;

        /// <summary> Length of the oscillator cycle in seconds. </summary>
        public float Length => (60 / Clock) * 4;

        int LastPhaseCalculationFrame;
        float phase;
        /// <summary> The current normalised phase of the oscillator. </summary>
        public float Phase
        {
            get
            {
                if (Time.frameCount != LastPhaseCalculationFrame)
                    phase += Time.time - (Time.time - Time.deltaTime) / Length;
                LastPhaseCalculationFrame = Time.frameCount;
                return phase;
            }
        }

        public static Dictionary<string, float> Multipliers = new Dictionary<string, float>()
        {
            { "1/32", (1 / 32f) },
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
            { "16", (16f) }
        };
    }
}