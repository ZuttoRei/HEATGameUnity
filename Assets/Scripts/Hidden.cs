using UnityEngine;
using System.Collections;

public class Hidden : MonoBehaviour {

	// Use this for initialization
	void Start () {
        this.GetComponent<Renderer>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {

	}
}
