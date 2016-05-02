using UnityEngine;
using System.Collections.Generic;

public class Charger_Drill_Spark : MonoBehaviour {

    public List<string> CollidableObjects;

	// Use this for initialization
	void Start () {
        particles = transform.GetComponentInChildren<ParticleSystem>();
	}
	
	// Update is called once per frame
	void LateUpdate () {
        transform.localPosition = new Vector3(0, 1.43f, 0);
        transform.localRotation = Quaternion.Euler(81.64072f, -53.81219f, -54.10339f);
	}

    public ParticleSystem particles;

    void OnTriggerStay2D(Collider2D other)
    {
        if (CollidableObjects.Contains(other.gameObject.tag))
        {
            particles.Play();
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (CollidableObjects.Contains(other.gameObject.tag))
        {
            particles.Stop();
        }

    }
}
