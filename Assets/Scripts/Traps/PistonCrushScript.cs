using UnityEngine;
using System.Collections;

public class PistonCrushScript : MonoBehaviour {

    public float CrushDelay;
    //Spikes only deal damage when this is set to true
    //It will be set to true as the pistons velocitate towards eachother
    public bool IsCrushing;
    public bool IsClosed;
    public float CollisionDamage;
    Animator anim;
    CharControls playerScript;

    public Transform bloodExplosion;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        playerScript = GameObject.Find("Player").GetComponent<CharControls>();
        anim.speed = 0;
        StartCoroutine(StartCrushing());
	}

    IEnumerator StartCrushing()
    {
        yield return new WaitForSeconds(CrushDelay);
        anim.speed = 1;
    }

	
	// Update is called once per frame
	void Update () {
	    
	}

    public void SplashBlood()
    {
        Instantiate(bloodExplosion, playerScript.gameObject.transform.position, Quaternion.identity);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            if (IsCrushing && !IsClosed)
            {
                print("Rekt");
                anim.SetTrigger("Crush");
                playerScript.Crush();
            }
            else
            {
                playerScript.HP -= CollisionDamage;
                playerScript.LeakBloodPuddle();
            }
        }
    }


    //If the player walks into the side of the piston, it will refuse to open, to avoid cheesing it
    //by running into the sides carelessly
    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            if(IsClosed)
                anim.speed = 0;
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (IsClosed)
                anim.speed = 1;
        }
    }
}
