using UnityEngine;
using System.Collections;

public class NonSprintArea : MonoBehaviour {


    CharControls playerScript;
	// Use this for initialization
	void Start () {
        playerScript = GameObject.Find("Player").GetComponent<CharControls>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerScript.CanSprint = false;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerScript.CanSprint = true;
        }
    }
}
