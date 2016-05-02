using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Explodable : MonoBehaviour {

    [Range(0f, 10)]
    public float ShockwaveRadius = 1f;
    public List<Transform> Explosions;
    public List<Transform> Scorchmarks;
    private Transform Explosion;
    private Transform Scorchmark;
    public bool DestroyOnFinish;
    public List<string> VulnerableTags = new List<string>();
    public bool DamagesPlayer = true;
    public Material BurnMaterial;



    Collider2D[] colliders;

	// Use this for initialization
	void Start () {
        colliders = GetComponents<Collider2D>();

        if (DamagesPlayer)
            VulnerableTags.Add("Player");

        //Get a random effect
        Explosion = Explosions[Random.Range(0, Explosions.Count)];
        Scorchmark = Scorchmarks[Random.Range(0, Scorchmarks.Count)];
	}

    public void Explode(bool ShowScorchMark = true, bool ShowExplosion = true)
    {
        StartCoroutine(explode(ShowScorchMark, ShowExplosion));
    }


    IEnumerator explode(bool ShowScorchMark = true, bool ShowExplosion = true)
    {
        //Disable any colliders to avoid them trigger again
        if(colliders.Length > 0)
        {
            foreach(Collider2D c in colliders)
            {
                if(c != null)
                {
                    c.enabled = false;
                }
            }
        }

        if (DestroyOnFinish)
        {
            //Hide it immediately
            foreach (Transform t in transform)
            {
                Renderer r = t.GetComponent<Renderer>();
                if (r != null)
                    r.enabled = false;
            }
        }

        //Summon the blast & simulate damaged ground
        if(ShowExplosion)
            Instantiate(Explosion, new Vector3(transform.position.x, transform.position.y, -5), Quaternion.identity);
        if(ShowScorchMark)
            Instantiate(Scorchmark, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.Euler(0, 0, Random.Range(0, 360)));

        //Wait for a short duration for a nice chain explosion effect
        yield return new WaitForSeconds(Random.Range(0.2f, 0.5f));
        
        //Explode each item in range & sight
        foreach(GameObject obj in AffectedObjects(transform.position, ShockwaveRadius))
        {
            Explodable ex = obj.GetComponent<Explodable>();
            if (ex != null)
                ex.Explode();

            //Turn oil into fire
            if(obj.tag == "Oil")
            {
                obj.GetComponent<Renderer>().material = BurnMaterial;
                obj.GetComponent<SubstanceTrigger>().SubstanceType = FootprintTrailer.PrintType.Fire;
            }
            //Knock over if it's a barrel
            if(obj.tag.Contains("Barrel"))
            {
                obj.GetComponent<BarrelScript>().BarrelHP = 0;
            }

            //If exploding near a toxic barrel, create a gas cloud over it
            if(obj.tag == "Toxic Barrel")
            {
                Instantiate(obj.GetComponent<BarrelScript>().PoisonGas, obj.transform.position, obj.transform.rotation);
            }
            //Kill player
            else if (DamagesPlayer && obj.tag == "Player")
            {
                GameObject.Find("Player").GetComponent<CharControls>().Disintegrate();
            }
        }

        if (DestroyOnFinish)
            Destroy(gameObject);
    }

    GameObject[] AffectedObjects(Vector3 position, float radius)
    {
        //Avoid raycasting self
        //Physics2D.IgnoreCollision()

        //Return the objects in range
        return Physics2D.OverlapCircleAll(position, radius).Select(f => f.gameObject).ToArray();

        //Commented out version makes it so it only explodes objects are in line of sight, couldn't get it to work. I'll try again next time

        /*
        List<GameObject> newobjects = new List<GameObject>();

        //Iterate through these nearby objects
        foreach(Collider2D col in colliders)
        {
            //Raycast them individually, to see if they are in sight
            Ray2D ray = new Ray2D(transform.position, (Vector2)col.gameObject.transform.TransformPoint(col.gameObject.transform.position) - (Vector2)transform.position);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Vector3.Distance(transform.position, (Vector2)col.gameObject.transform.TransformPoint(col.gameObject.transform.position)), LayersToRaycast.value);
            
            //If in sight, add object to our list
            if(hit.collider != null && VulnerableTags.Contains(hit.collider.gameObject.tag) && hit.collider.gameObject.tag != gameObject.tag) //Ignore self
            {
                newobjects.Add(hit.collider.gameObject);
            }
        }
        return newobjects.ToArray();
         */
    }

	// Update is called once per frame
	void Update () {
	
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(Explodable))]
public class DrawRadius : Editor
{
    Explodable explosion;

    public void OnEnable()
    {
        Handles.color = Color.green;
        explosion = (Explodable)target;
    }


    void OnSceneGUI()
    {
        Handles.CircleCap(0, explosion.transform.position, explosion.transform.rotation, explosion.ShockwaveRadius);
    }

}
#endif