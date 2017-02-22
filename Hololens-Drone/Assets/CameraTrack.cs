using System.Collections;

using System.Collections.Generic;
using UnityEngine;

public class CameraTrack : MonoBehaviour {
	// Use this for initialization
	void Start () {

	}
	
	void LateUpdate () {
        Debug.Log(transform.rotation.eulerAngles.x);
        Debug.Log(transform.rotation.eulerAngles.y);
        Debug.Log(transform.rotation.eulerAngles.z);
    }
}
