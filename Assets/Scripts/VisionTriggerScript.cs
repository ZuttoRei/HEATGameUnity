using UnityEngine;
using System.Collections;

public class VisionTriggerScript : MonoBehaviour {

    //Hide object by placing them under the map, easier to accomplish
    Vector3 initialPosition;
    public bool Enabled = true;

	// Use this for initialization
	void Start () {
        
        //Not enabled? Won't need it
        if (!Enabled)
        {
            Destroy(this);
        }

        initialPosition = GetComponentInParent<Transform>().position;
        HideObject();
	}
	
    void HideObject()
    {
        transform.root.position = new Vector3(initialPosition.x, initialPosition.y, 100);
    }

    void ShowObject()
    {
        transform.root.position = new Vector3(initialPosition.x, initialPosition.y, initialPosition.z);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            ShowObject();
            //Trigger won't be necessary anymore
            Destroy(gameObject);
        }
    }
}
