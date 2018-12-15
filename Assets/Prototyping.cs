using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Prototyping : MonoBehaviour
{

    public List<float> _floats;
    public static List<float> Floats { get; private set; }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_floats != Floats)
        {
            Floats = _floats;
        }
    }
}
