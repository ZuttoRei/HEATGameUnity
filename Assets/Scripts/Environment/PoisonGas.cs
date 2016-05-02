using UnityEngine;
using System.Collections;

public class PoisonGas : MonoBehaviour {

    public float Damage;
    public float DurationInSeconds;
    public bool LoopForever;
    CharControls playerScript;
    ParticleSystem particles;

    

	// Use this for initialization
	void Start () {
        playerScript = GameObject.Find("Player").GetComponent<CharControls>();
        particles = GetComponent<ParticleSystem>();
        particles.loop = LoopForever;
	}


    bool stopped = false;

	// Update is called once per frame
	void FixedUpdate () {

        if (LoopForever)
            return;

        DurationInSeconds -= Time.deltaTime;

        if(DurationInSeconds <= 0 && !stopped)
        {
            stopped = true;
            particles.Stop();
            Destroy(gameObject, 2);
        }
	}

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            playerScript.HP -= Damage * Time.deltaTime; 
            if(playerScript.HP <= 0)
            {
                playerScript.Dissolve();
            }
        }
    }
}
