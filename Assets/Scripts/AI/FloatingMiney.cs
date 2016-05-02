using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FloatingMiney : MonoBehaviour {

    [Range(0.1f, 5)]
    public float IndicatorSpeed;
    [Range(0.1f, 10)]
    public float ShockwaveRadius = 1f;
    Animator anim;
    public Transform Explosion;
    public Transform FloorBurn;
    public bool BeginHidden;
    public List<string> VulnerableTags = new List<string>();
    CircleCollider2D collider;
    Explodable explo;
    GameObject player;
    


	// Use this for initialization
	void Start () {
        anim = transform.Find("Indicator").GetComponent<Animator>();
        collider = transform.Find("Indicator").GetComponent<CircleCollider2D>();
        explo = GetComponent<Explodable>();
        player = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update () {
        if(anim != null)
            anim.speed = IndicatorSpeed;
	}

    void LateUpdate()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            player.GetComponent<Explodable>().Explode(true, false);
            player.GetComponent<CharControls>().Disintegrate();
        }

        if (VulnerableTags.Contains(other.gameObject.tag))
        {
            explo.Explode();   
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            player.GetComponent<Explodable>().Explode(true, false);
            player.GetComponent<CharControls>().Disintegrate();
        }

        if (VulnerableTags.Contains(other.gameObject.tag))
        {
            explo.Explode();
        }
    }
}
