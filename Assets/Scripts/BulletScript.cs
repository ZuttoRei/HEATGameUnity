using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour {

    Vector3 startPosition;
    public float Speed = 10;
    public float Damage = 25;
    public Transform ImpactEffect;
    public Transform BloodSplash;
    public Transform BloodLeak;
    

	// Use this for initialization
	void Start () {
        //Don't make bullets collide with eachother
        Physics2D.IgnoreLayerCollision(15, 15, true);
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(Vector3.left * Speed * Time.deltaTime);
	}

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            //Splash some blood
            Instantiate(BloodSplash, other.contacts[0].point, Quaternion.identity);
            //Take damage
            other.gameObject.GetComponent<CharControls>().HP -= Damage;
            //Leave a blood stain
            other.gameObject.GetComponent<CharControls>().LeakBloodPuddle();

            //Check if player has died
            if (other.gameObject.GetComponent<CharControls>().HP <= 0)
            {
                Instantiate(BloodSplash, other.transform.position - other.transform.forward, Quaternion.identity);
                other.gameObject.GetComponent<CharControls>().FacePosition(transform.position);
                other.gameObject.GetComponent<CharControls>().BrainSplatter();
                return;
            }
        }

        //Small delay so barrel script can activate the collision event as well
        Destroy(gameObject, 0.001f);

        if(other.gameObject.tag != "Player")
            Instantiate(ImpactEffect, (Vector3)other.contacts[0].point, Quaternion.identity);
    }

    Quaternion FacePosition(Vector3 position, float offset = 180)
    {
        return Quaternion.Euler(0, 0, Mathf.Atan2(position.y - transform.position.y, position.x - transform.position.x) * Mathf.Rad2Deg - offset);
    }
}
