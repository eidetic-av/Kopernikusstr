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
        [Input, Range(0, 12)] public int MultipleIndexX = 10;
        [Input, Range(0, 12)] public int MultipleIndexY = 10;
        [Input, Range(0, 12)] public int MultipleIndexZ = 10;

        /// <summary> Speed of the oscillator in BPM </summary>
        [Input] public float Speed { get; set; } = 120f;

        /// <summary> Sine wave output value. </summary>
        [Output] public float Sine => CalculateSin();
        [Output] public float SineX => CalculateSin(Multipliers.ElementAt(MultipleIndexX).Value);
        [Output] public float SineY => CalculateSin(Multipliers.ElementAt(MultipleIndexY).Value);
        [Output] public float SineZ => CalculateSin(Multipliers.ElementAt(MultipleIndexZ).Value);

        /// <summary> Triangle wave output value.</summary>
        [Output] public float Triangle => CalculateTriangle();
        [Output] public float TriangleX => CalculateSin(Multipliers.ElementAt(MultipleIndexX).Value);
        [Output] public float TriangleY => CalculateSin(Multipliers.ElementAt(MultipleIndexY).Value);
        [Output] public float TriangleZ => CalculateSin(Multipliers.ElementAt(MultipleIndexZ).Value);

        /// <summary> Sawtooth wave output value. </summary>
        [Output] public float Sawtooth => CalculateSawTooth();
        [Output] public float SawtoothX => CalculateSin(Multipliers.ElementAt(MultipleIndexX).Value);
        [Output] public float SawtoothY => CalculateSin(Multipliers.ElementAt(MultipleIndexY).Value);
        [Output] public float SawtoothZ => CalculateSin(Multipliers.ElementAt(MultipleIndexZ).Value);

        /// <summary> Ramp (inverse-sawtooth) wave output value. </summary>
        [Output] public float Ramp => CalculateRamp();
        [Output] public float RampX => CalculateSin(Multipliers.ElementAt(MultipleIndexX).Value);
        [Output] public float RampY => CalculateSin(Multipliers.ElementAt(MultipleIndexY).Value);
        [Output] public float RampZ => CalculateSin(Multipliers.ElementAt(MultipleIndexZ).Value);

        public float CalculateSin(float multiplier = 1) => Mathf.Sin((Phase * multiplier) * (Mathf.PI * 2));
        public float CalculateTriangle(float multiplier = 1) => (Mathf.PingPong((Phase * multiplier), .5f) * 4) - 1;
        public float CalculateSawTooth(float multiplier = 1) => CalculateRamp(multiplier) * -1;
        public float CalculateRamp(float multiplier = 1) => (((Phase * multiplier) % 1f) * 2) - 1;

        /// <summary> Length of the oscillator cycle in seconds. </summary>
        public float Length => (60 / Speed) * 4;

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