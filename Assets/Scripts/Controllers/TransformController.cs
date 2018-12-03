using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformController : MonoBehaviour

{
    public void SetLocalScaleZ(float value)
    {
        var currentLocalScale = transform.localScale;
        transform.localScale = new Vector3
		(
			currentLocalScale.x,
			currentLocalScale.y,
			value
		);
    }
}
