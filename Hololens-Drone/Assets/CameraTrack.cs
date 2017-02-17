using System.Collections;

using System.Collections.Generic;
using UnityEngine;

public class CameraTrack : MonoBehaviour {
    public float x, y, z; 
	// Use this for initialization
	void Start () {
        x = 0;
        y = 0;
        z = 0;
	}
	
	void LateUpdate () {
        x = transform.rotation.eulerAngles.x;
        y = transform.rotation.eulerAngles.y;
        z = transform.rotation.eulerAngles.z;
        Debug.Log(x);
        Debug.Log(y);
        Debug.Log(z);
    }
}
