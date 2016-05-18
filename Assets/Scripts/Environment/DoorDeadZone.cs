using UnityEngine;
using System.Collections;

public class DoorDeadZone : MonoBehaviour {


    Animator anim;

	// Use this for initialization
	void Start () {
        anim = GetComponentInParent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {

	}


    void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            if(anim.GetCurrentAnimatorStateInfo(0).IsName("Close"))
            {
                GameObject.FindGameObjectWithTag("Player").GetComponentInParent<CharControls>().Crush();
            }
        }
    }
}
