using UnityEngine;
using System.Collections;

public class DoorScript : MonoBehaviour
{

    Animator anim;
    public Transform dozer;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Open()
    {
        anim.Play("Open");
    }

    public void Close()
    {
        anim.Play("Close");
    }

    public void ActivateDozer()
    {
        dozer.GetComponent<PolyNavAgent>().enabled = true;
    }
}
