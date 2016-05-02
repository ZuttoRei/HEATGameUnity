using UnityEngine;
using System.Collections;

public class DoorTriggerDozerCorridor : MonoBehaviour {

    public Transform LinkedDoor;
	// Use this for initialization
	void Start () {
	    
	}
	
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            //Open the door
            LinkedDoor.GetComponent<DoorScript>().SendMessage("Open");
        }
    }
    
}
