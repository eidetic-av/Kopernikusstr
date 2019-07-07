using UnityEngine;

public class DisplayActivation : MonoBehaviour
{
    void Start()
    {
        if (Display.displays.Length > 1)
        {
            Display.displays[1].Activate();
            Display.displays[1].SetRenderingResolution(720, 1280);
            Display.displays[1].SetParams(720, 1280, 0, 0);
        }
        if (Display.displays.Length > 2)
        {
            Display.displays[2].Activate();
            Display.displays[1].SetRenderingResolution(720, 1280);
            Display.displays[1].SetParams(720, 1280, 0, 0);
        }

        Screen.SetResolution(720, 1280, false);
    }
}