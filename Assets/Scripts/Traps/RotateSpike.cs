using UnityEngine;
using System.Collections;

public class RotateSpike : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.rotation = transform.rotation * Quaternion.Euler(0, 0, 20000 * Time.deltaTime);
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<CharControls>().NormalKill();
            col.GetComponent<CharControls>().Dead = true;
        }
    }
}
