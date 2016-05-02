using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class BreakDoorOnCollision : MonoBehaviour {

    public Transform BrokenDoor;
    public ParticleSystem particle;
    CargoDoorTrigger trigger;

	// Use this for initialization
	void Start () {
        BrokenDoor.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        particle.Stop();
        trigger = GetComponentInParent<CargoDoorTrigger>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public List<string> AllowedTags;
    void OnCollisionEnter2D(Collision2D other)
    {
        if(AllowedTags.Contains(other.gameObject.tag))
        {
            //Only break doors if they're unopened
            if (trigger.DoorsOpen)
            {
                if (other.gameObject.tag == "Charger")
                {
                    //Delay the breakage of the door slightly for chargers, for... cool visual effect
                    other.gameObject.transform.Find("Headlight").GetComponent<Light>().enabled = false;
                    StartCoroutine(DestroyDoor(other, 3));
                }
                else
                {
                    StartCoroutine(DestroyDoor(other, 1));
                }
            }
        }
    }

    bool started = false;
    IEnumerator DestroyDoor(Collision2D other, float breakDelay)
    {
        if (!started)
        {
            started = true;
            ParticleSystem p = Instantiate(particle, other.contacts[0].point, Quaternion.identity) as ParticleSystem;
            p.Play();
            yield return new WaitForSeconds(breakDelay);
            this.GetComponent<Collider2D>().enabled = false;
            Destroy(this.gameObject);
            Destroy(p.GetComponent<Light>());
            Destroy(p);
            BrokenDoor.gameObject.GetComponent<SpriteRenderer>().enabled = true;

            //Manually stop chargers' particles
            if (other.gameObject.tag == "Charger")
            {
                other.gameObject.GetComponentsInChildren<ParticleSystem>().ToList().ForEach(f => f.Stop());
                other.gameObject.transform.Find("Headlight").GetComponent<Light>().enabled = true;
            }
        }
    }
}
