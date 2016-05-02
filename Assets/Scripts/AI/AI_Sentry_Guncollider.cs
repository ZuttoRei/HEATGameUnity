using UnityEngine;
using System.Collections;

public class AI_Sentry_Guncollider : MonoBehaviour {

    AI_Sentry MainScript;

	// Use this for initialization
	void Start () {
        MainScript = GetComponentInParent<AI_Sentry>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
            MainScript.InViewRadius = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
            MainScript.InViewRadius = false;
    }

    
}
