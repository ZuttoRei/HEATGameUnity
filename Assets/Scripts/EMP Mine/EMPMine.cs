using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EMPMine : MonoBehaviour {

    public bool Enabled = true;
    public bool Collided;
    public float DisableTime = 30;
    public bool DestroyOnTrigger = true;


    public List<string> VulnerableTags;
    public List<string> ImmuneTags;


    void Awake()
    {
        GetComponentInChildren<ParticleSystem>().Stop();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (!enabled)
            return;

        //If we collide with an object that is vulnerable to this
        if (VulnerableTags.Contains(collider.tag))
        {
            StartCoroutine(Explode());    
            Collided = true;
            collider.gameObject.GetComponent<Hibernation>().PowerOff(DisableTime);
        }
        else if(ImmuneTags.Contains(collider.tag))
        {
            //Destroy mine but don't affect the object colliding it
            StartCoroutine(Explode());
            Collided = true;
        }
    }

    public IEnumerator Explode()
    {
        GetComponentInChildren<ParticleSystem>().Play();
        yield return new WaitForSeconds(0.2f);
        if (DestroyOnTrigger)
            Destroy(gameObject);
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (VulnerableTags.Contains(collider.tag))
        {
            Collided = false;
        }
    }
}
