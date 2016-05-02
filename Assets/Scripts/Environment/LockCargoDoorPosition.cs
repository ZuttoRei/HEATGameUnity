using UnityEngine;
using System.Collections;

public class LockCargoDoorPosition : MonoBehaviour {


    public Vector3 position;
    

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate () {
        transform.position = position;
	}
}
