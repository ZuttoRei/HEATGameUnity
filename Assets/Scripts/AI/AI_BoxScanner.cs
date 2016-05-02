using UnityEngine;
using System.Collections;
using Vectrosity;


public class AI_BoxScanner : MonoBehaviour {

    GameObject player;
    public Material lineMaterial;
    public Transform PointA;
    public Transform PointB;
    public float LaserWidth;

    public LayerMask LayersToRayscan = -1;

	// Use this for initialization
	void Start () {
        player = GameObject.Find("Player");
        
	}
	
	// Update is called once per frame
	void LateUpdate () {
        
        Ray2D rayA = new Ray2D(PointA.position, PointB.position - PointA.position);
        Ray2D rayB = new Ray2D(PointB.position, PointA.position - PointB.position);

        RaycastHit2D hitA = Physics2D.Raycast(rayA.origin, rayA.direction, Vector3.Distance(PointA.position, PointB.position), LayersToRayscan);
        RaycastHit2D hitB = Physics2D.Raycast(rayB.origin, rayB.direction, Vector3.Distance(PointB.position, PointA.position), LayersToRayscan);

        if(hitA != null && hitA.collider != null)
        {
            VectorLine line = VectorLine.SetLine(Color.red, 0.000001f, PointA.position, (Vector3)hitA.point);
            line.color = Color.red;
            line.material = lineMaterial;
            line.lineWidth = LaserWidth;

            line.smoothWidth = true;
            line.Draw();
        }
        else
        {
            VectorLine line = VectorLine.SetLine(Color.red, 0.000001f, PointA.position, PointB.position);
            line.color = Color.red;
            line.material = lineMaterial;
            line.lineWidth = LaserWidth;
            line.smoothWidth = true;
            line.Draw();
        }

        if (hitB != null && hitB.collider != null)
        {
            VectorLine line = VectorLine.SetLine(Color.red, 0.000001f, PointB.position, (Vector3)hitB.point);
            line.color = Color.red;
            line.material = lineMaterial;
            line.lineWidth = LaserWidth;

            line.smoothWidth = true;
            line.Draw();
        }
        else
        {
            VectorLine line = VectorLine.SetLine(Color.red, 0.000001f, PointB.position, PointA.position);
            line.color = Color.red;
            line.material = lineMaterial;
            line.lineWidth = LaserWidth;
            line.smoothWidth = true;
            line.Draw();
        }

        



    }
}
