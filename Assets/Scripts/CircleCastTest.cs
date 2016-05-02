using UnityEngine;
using System.Collections.Generic;

public class CircleCastTest : MonoBehaviour
{

	// Use this for initialization
	void Start () {
	
	}

    public LayerMask Layers = -1;
    public List<string> HiddenObjects = new List<string>();

	// Update is called once per frame
	void Update ()
    {
        //for(int i = 0; i < colliders.Count; i++)
        //{
        //    Ray2D ray = new Ray2D(transform.position, colliders[i].transform.position - transform.position);
        //    RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Vector3.Distance(transform.position, colliders[i].transform.position), Layers);
        //    Debug.DrawLine(transform.position, hit.point);

        //    if (hit.collider != null && hit.collider.tag == "Trap")
        //    {
        //        colliders[i].gameObject.GetComponent<Hidden>().Visible = true;
        //    }
        //}
	}

    void LateUpdate()
    {
        transform.localPosition = new Vector3(0, 0, 0);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (HiddenObjects.Contains(other.gameObject.tag))
        {
            other.gameObject.GetComponent<Visibility>().Visible = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (HiddenObjects.Contains(other.gameObject.tag))
        {
            other.gameObject.GetComponent<Visibility>().Visible = false;
        }
    }
}
