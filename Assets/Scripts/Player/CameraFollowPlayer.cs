using UnityEngine;
using System.Collections;

public class CameraFollowPlayer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    //Smoothness
    public float dampTime = 0.15f;
    //Target the camera follows
    public Transform target;
    private Vector3 velocity = Vector3.zero;

	// Update is called once per frame
	void FixedUpdate () {
	    if(target)
        {
            Vector3 point = Camera.main.WorldToViewportPoint(target.position);
            Vector3 delta = target.position - Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
            Vector3 destination = transform.position + delta;
            transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
        }
	}
}
