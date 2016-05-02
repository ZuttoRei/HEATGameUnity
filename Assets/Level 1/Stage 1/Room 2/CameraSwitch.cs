using UnityEngine;
using System.Collections;

public class CameraSwitch : MonoBehaviour {


    public float NewCameraSize;
    public float LerpSpeed;
    float OrginalCameraSize;

    Camera camera;
    bool Entered = false;
    bool triggered;

	// Use this for initialization
	void Start () {
        //Store camera's original size
        OrginalCameraSize = Camera.main.orthographicSize;
        camera = Camera.main;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        //Only start updating the camera once the player has atleast triggered us once
        if (!triggered)
            return;

        if (Entered)
            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, NewCameraSize, LerpSpeed);
        else
            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, OrginalCameraSize, LerpSpeed);
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        triggered = true;
        if(other.gameObject.tag == "Player")
        {
            Entered = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            Entered = false;
        }
    }
}
