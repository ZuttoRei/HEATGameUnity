using UnityEngine;
using System.Collections;

public class Raycast : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}


    public LayerMask mask = -1;

	// Update is called once per frame
	void Update () {

        //Vector3 mousePos = Input.mousePosition;
        //mousePos.z = 0f;

        //Vector2 v2 = Camera.main.ScreenToWorldPoint(mousePos);

        //Ray2D ray = new Ray2D(transform.position + new Vector3(0.1f, 0, 0), v2 - new Vector2(transform.position.x, transform.position.y));
        //RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 100, mask.value);

        //Color color = Color.green;
        //float distance = 100;

        //if(hit)
        //{
        //    distance = hit.distance;

        //    if (hit.collider.tag == "Wall")
        //    {
        //        color = Color.red;
        //    }
        //}

        //Debug.DrawRay(ray.origin, ray.direction * distance, color);

	}

}
