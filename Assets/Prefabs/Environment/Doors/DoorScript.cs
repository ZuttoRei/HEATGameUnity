using UnityEngine;
using System.Collections;

public class DoorScript : MonoBehaviour
{

    Animator anim;
    public Transform dozer;
    bool IsOpen = false;
    public float AnimationSpeed;
    public bool AutomaticDoor;


    //properties that are public are shown here 

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        anim.speed = AnimationSpeed;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Open()
    {
        IsOpen = true;
        anim.SetTrigger("Open");
    }

    public void Close()
    {
        IsOpen = false;
        anim.SetTrigger("Close");
    }

    public void ActivateDozer() //????
    {
        dozer.GetComponent<PolyNavAgent>().enabled = true;
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && !AutomaticDoor)
        {
                if (Input.GetKeyDown(KeyCode.E) && !IsOpen) //bap
                {
                    Open();
                }
                else if (Input.GetKeyDown(KeyCode.E) && IsOpen)
                {
                    Close();
                }
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (IsOpen)
                Close();
        }
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!IsOpen && AutomaticDoor)
                Open();
        }
    }
}
