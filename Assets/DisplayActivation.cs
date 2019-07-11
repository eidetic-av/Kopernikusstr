using UnityEngine;

public class DisplayActivation : MonoBehaviour
{
    void Start()
    {
        if (Display.displays.Length > 1)
        {
            if (!Display.displays[1].active)
            {
                Display.displays[1].Activate();
                Display.displays[1].SetRenderingResolution(720, 1280);
                Display.displays[1].SetParams(720, 1280, 0, 0);
            }
        }
        if (Display.displays.Length > 2)
        {
            if (!Display.displays[2].active)
            {
                Display.displays[2].Activate();
                Display.displays[1].SetRenderingResolution(720, 1280);
                Display.displays[1].SetParams(720, 1280, 0, 0);
            }
        }

        Screen.SetResolution(720, 1280, false);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            var camera1 = GameObject.Find("Camera1");
            if (Input.GetKey(KeyCode.LeftArrow)) {
                camera1.transform.position = new Vector3(camera1.transform.position.x - 0.005f, camera1.transform.position.y, camera1.transform.position.z);
            } else if (Input.GetKey(KeyCode.RightArrow))
                {
                    camera1.transform.position = new Vector3(camera1.transform.position.x + 0.005f, camera1.transform.position.y, camera1.transform.position.z);
                }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                camera1.transform.position = new Vector3(camera1.transform.position.x, camera1.transform.position.y - 0.005f, camera1.transform.position.z);
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                camera1.transform.position = new Vector3(camera1.transform.position.x, camera1.transform.position.y + 0.005f, camera1.transform.position.z);
            }
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            var camera2 = GameObject.Find("Camera2");
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                camera2.transform.position = new Vector3(camera2.transform.position.x - 0.005f, camera2.transform.position.y, camera2.transform.position.z);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                camera2.transform.position = new Vector3(camera2.transform.position.x + 0.005f, camera2.transform.position.y, camera2.transform.position.z);
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                camera2.transform.position = new Vector3(camera2.transform.position.x, camera2.transform.position.y - 0.005f, camera2.transform.position.z);
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                camera2.transform.position = new Vector3(camera2.transform.position.x, camera2.transform.position.y + 0.005f, camera2.transform.position.z);
            }
        }

        if (Input.GetKey(KeyCode.RightControl))
        {
            var camera1 = GameObject.Find("Camera1");
            if (Input.GetKey(KeyCode.UpArrow))
            {
                camera1.transform.position = new Vector3(camera1.transform.position.x, camera1.transform.position.y, camera1.transform.position.z - 0.005f);
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                camera1.transform.position = new Vector3(camera1.transform.position.x, camera1.transform.position.y, camera1.transform.position.z + 0.005f);
            }
        }
        if (Input.GetKey(KeyCode.RightShift))
        {
            var camera2 = GameObject.Find("Camera2");
            if (Input.GetKey(KeyCode.UpArrow))
            {
                camera2.transform.position = new Vector3(camera2.transform.position.x, camera2.transform.position.y, camera2.transform.position.z - 0.005f);
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                camera2.transform.position = new Vector3(camera2.transform.position.x, camera2.transform.position.y, camera2.transform.position.z + 0.005f);
            }
        }
    }
}