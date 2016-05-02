using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class BarrelScript : MonoBehaviour {

    public Sprite NormalTexture;
    public Sprite FallenDownTexture;
    public GameObject substance;
    public CharControls playerScript;
    //Only ojects with this tag will knock this barrel over
    public List<string> VulnerableObjectTags;
    //Set a cap for performance
    public int MaxPuddlesOnMap = 250;
    Transform substanceStart;
    Transform Player;
    
    //To neatly store all spills in a single parent
    GameObject container;
    SpriteRenderer render;
    public int BarrelHP = 10;

    public ParticleSystem SplashEffect;
    public GameObject PoisonGas;
    Explodable explodable;
    private bool Explosive;

    bool Shrunk = false;
    public bool _Fallen;
    public bool Fallen
    {
        get { return _Fallen; }
        set 
        {
            _Fallen = value;

            if (value == true)
            {
                //Only allow it to be knocked over if it hasn't already been knocked over
                if (!Shrunk)
                {
                    Shrunk = true;
                    //Rescale barrel down by 20% when it falls
                    transform.localScale = transform.localScale - new Vector3(transform.localScale.x / 100 * 20, transform.localScale.y / 100 * 20, transform.localScale.z / 100 * 20);
                    render.sprite = FallenDownTexture;
                    gameObject.layer = 2;
                    //Move it down to layer 0 so other objects render over it, since it's .. on the floor now
                    gameObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
                    GetComponent<Rigidbody2D>().isKinematic = false;
                    ResetColliderSize();

                    //Make barrel fall the opposite way of the player
                    transform.rotation = FacePosition(Player.transform.position);

                }

                SpawnSubstance();
            }
            else
            {
                render.sprite = NormalTexture;
                ResetColliderSize();
            }
        }
    }


    Quaternion FacePosition(Vector3 position, float offset = 180)
    {
        return Quaternion.Euler(0, 0, Mathf.Atan2(position.y - transform.position.y, position.x - transform.position.x) * Mathf.Rad2Deg - offset);
    }

    void Awake()
    {
        render = GetComponent<SpriteRenderer>();
        substanceStart = transform.Find("SubstancePosition");
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<CharControls>();
        Player = GameObject.FindGameObjectWithTag("Player").transform;

        if (GameObject.Find("SubstanceContainer") == null)
        {
            container = new GameObject("SubstanceContainer");
        }

        explodable = GetComponent<Explodable>();

        if (explodable != null)
            Explosive = true;
    }

	// Use this for initialization
	void Start () {

        container = GameObject.Find("SubstanceContainer");
	}


    float elapsed;


    bool exploded;
	// Update is called once per frame
	void Update () {
        elapsed += Time.deltaTime;

        if(BarrelHP == 0)
        {
            if(Explosive && !exploded)
            {
                exploded = true;
                GetComponent<Explodable>().Explode();
            }
            Fallen = true;
        }
	}

    public void KnockOver()
    {
        Fallen = true;
    }



    //Variable to determine how many drops a barrel can drop.
    [Range(0, 30)]
    public int MaxSubstancePiles;
    [Range(0f, 1f)]
    public float DropDelay;

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Projectile")
        {
            //Make barrel splash
            Instantiate(SplashEffect, other.contacts[0].point, Quaternion.Euler(0, 90, -other.transform.rotation.z));

            //Only one leak per 2 seconds for projectiles, period
            if (elapsed > 1)
            {
                elapsed = 0;
                SpawnSubstance();
            }
            BarrelHP--;
        }

        if (VulnerableObjectTags.Contains(other.gameObject.tag))
        {
            if (elapsed > DropDelay)
            {
                elapsed = 0;
                Fallen = true;
            }
        }
    }
    void OnCollisionStay2D(Collision2D other)
    {
        if (VulnerableObjectTags.Contains(other.gameObject.tag))
        {
            if (elapsed > DropDelay)
            {
                elapsed = 0;
                Fallen = true;
            }
        }
    }

    public void SpawnSubstance()
    {
        if (substance != null)
        {
            if (MaxSubstancePiles > 0)
            {
                MaxSubstancePiles--;
                (Instantiate(substance, substanceStart.position, Quaternion.Euler(0, 0, Random.Range(0, 360))) as GameObject).transform.SetParent(container.transform, true); //Make it a child of substance container

                //If max limit is exceeded, destroy the first one
                if (container.transform.Cast<Transform>().Count() > MaxPuddlesOnMap)
                {
                    Destroy(container.transform.Cast<Transform>().FirstOrDefault().gameObject);
                }
            }
        }
    }

    void ResetColliderSize()
    {
        //Disabled, caused trouble with polynavobstacles
        Destroy(GetComponent<Collider2D>());
        gameObject.AddComponent<BoxCollider2D>();
    }
}