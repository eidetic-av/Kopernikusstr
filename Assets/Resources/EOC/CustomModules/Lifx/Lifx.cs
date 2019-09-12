using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Eidetic.URack.Base;
using UnityEngine;
using UnityEngine.Networking;

namespace Eidetic.URack.Lifx
{
    public class LifxLan : Module
    {
        static readonly string ServerAddress = @"http://127.0.0.1:8888";

        static float LastRequest;
        static Process Process;

        [SerializeField]
        LightState LightState = new LightState()
        {
            Hue = 0,
            Saturation = 50,
            Brightness = 100,
            Kelvin = 3500
        };

        public void Start()
        {
            Process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                FileName = @"C:\Users\matth\AppData\Local\Programs\Python\Python37\python.exe",
                Arguments = @"E:\lifx-simple-master\lifx_webservice.py"
                }
            };
            Process.Start();
        }

        public void Exit()
        {
            if (Process != null && Process.Responding) Process.CloseMainWindow();
        }

        [Input(0, 360), Knob] public float Hue
        {
            set
            {
                var state = LightState;
                state.Hue = (int) value;
                SetLightState(state);
            }
        }

        [Input(0, 100), Knob] public float Saturation
        {
            set
            {
                var state = LightState;
                state.Saturation = (int) value;
                SetLightState(state);
            }
        }

        [Input(0, 100), Knob] public float Brightness
        {
            set
            {
                var state = LightState;
                state.Brightness = (int) value;
                SetLightState(state);
            }
        }

        [Input(2500, 9000, 5, 3500), Knob] public float Kelvin
        {
            set
            {
                var state = LightState;
                state.Kelvin = (int) value;
                SetLightState(state);
            }
        }

        [Input(0, 1), Knob] public float Red
        {
            set
            {
                var color = LightState.Color;
                color.r = value;
                SetLightState(color.ToLightState());
            }
        }

        [Input(0, 1), Knob] public float Green
        {
            set
            {
                var color = LightState.Color;
                color.g = value;
                SetLightState(color.ToLightState());
            }
        }

        [Input(0, 1), Knob] public float Blue
        {
            set
            {
                var color = LightState.Color;
                color.b = value;
                SetLightState(color.ToLightState());
            }
        }

        void SetLightState(LightState newState)
        {
            LightState = newState;

            if (UnityEditor.EditorApplication.isPlaying)
                if (Time.time - LastRequest > 0.05) RackRuntimeUpdater.Instance.StartCoroutine(RequestState(newState));
        }

        static IEnumerator RequestState(LightState newState)
        {
            UnityWebRequest www = UnityWebRequest.Get(ServerAddress + "/192.168.1.2/" +
                newState.Hue + "/" + newState.Saturation + "/" + newState.Brightness + "/" + newState.Kelvin);
            yield return www.SendWebRequest();
            LastRequest = Time.time;
        }
    }

    [System.Serializable] public class LightState
    {
        public int Hue, Saturation, Brightness, Kelvin;
        public Color Color => Color.HSVToRGB(Hue / 360f, Saturation / 100f, Brightness / 100f);
    }

    public static class LifxExtensions
    {
        public static LightState ToLightState(this Color color)
        {
            Color.RGBToHSV(color, out var h, out var s, out var v);
            var state = new LightState()
            {
                Hue = (h * 360).RoundToInt(),
                Saturation = (s * 100).RoundToInt(),
                Brightness = (v * 100).RoundToInt(),
                Kelvin = 3500
            };
            return state;
        }
    }
}