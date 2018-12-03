using OscJack;
using UnityEngine;

public class OscReceiver : MonoBehaviour
{
    public static OscServer Server = new OscServer(9000);

    void Awake() 
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void OnDestroy() 
    {
        Server.Dispose();
    }
}