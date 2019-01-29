using UnityEngine;
using System.Text;

public class NetworkThreadRunner : MonoBehaviour
{
    void Start()
    {
        if (NetworkThread.Connect(5004))
            Debug.Log("Started NetworkThread");
    }

    void OnDestroy()
    {
        NetworkThread.Disconnect();
    }

    void Update()
    {
        while (NetworkThread.ReceivedData.Count > 0)
        {
            Debug.Log(Encoding.UTF8.GetString(NetworkThread.ReceivedData.Dequeue()));
        }
    }
}
