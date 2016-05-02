using UnityEngine;
using System.Collections.Generic;

public class CargoDoorTrigger : MonoBehaviour {


    
    public float OpenSpeed;
    public float CloseSpeed;
    public Animation DoorAAnim;
    public Animation DoorBAnim;
    public List<string> EnemiesThatCanOpen;

    public bool _DoorsOpen;
    public bool DoorsOpen
    {
        get { return _DoorsOpen; }
        set 
        {
            _DoorsOpen = value; 
            if(!value)
            {
                OpenDoors();
            }
            else
            {
                CloseDoors();
            }
        }
    }
    
    public void OpenDoors()
    {
        if (!DoorsOpen)
        {
            DoorAAnim["Cargo_DoorA_Open"].speed = OpenSpeed;
            DoorBAnim["Cargo_DoorB_Open"].speed = OpenSpeed;
            DoorAAnim.Play("Cargo_DoorA_Open");
            DoorBAnim.Play("Cargo_DoorB_Open");
        }
    }

    public void CloseDoors()
    {
        if (DoorsOpen)
        {
            DoorAAnim["Cargo_DoorA_Close"].speed = CloseSpeed;
            DoorBAnim["Cargo_DoorB_Close"].speed = CloseSpeed;
            DoorAAnim.Play("Cargo_DoorA_Close");
            DoorBAnim.Play("Cargo_DoorB_Close");
        }
    }



	// Use this for initialization
	void Start () {
        if(!DoorsOpen)
        {
            OpenDoors();
        }
        if(DoorsOpen)
        {
            CloseDoors();
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (PlayerInTrigger)
        {
            if (!DoorsOpen)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    DoorsOpen = true;
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    DoorsOpen = false;
                }
            }
        }
	}

    bool PlayerInTrigger = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            PlayerInTrigger = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            PlayerInTrigger = false;
        }
    }
}
