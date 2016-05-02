using UnityEngine;
using System.Collections;

public class InfraredSensor : MonoBehaviour {

    public bool Enabled;
    public LayerMask LayersToRayscan = -1;

    public Transform IR;
    public Transform ConductorA;
    public Transform ConductorB;

    LineRenderer LaserBeam;

    public bool Debugging;
    public bool SeePlayer;

    private bool _Detected;
    public bool Detected
    {
        get
        {
            return _Detected;
        }
        set
        {
            _Detected = value;
            //Also change the value separatedly in the players' script
            playerScript.Detected = value;
        }
    }


    //Owner is the parent of the vision control
    Transform player;
    CharControls playerScript;

    public Color NeutralColor;
    public Color DetectionColor;
    private Color laserColor;
    public Color LaserColor
    {
        get { return laserColor; }
        set { laserColor = value; LaserBeam.SetColors(value, value); }
    }

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerScript = player.GetComponent<CharControls>();
        LaserBeam = IR.GetComponent<LineRenderer>();
        LaserBeam.useWorldSpace = true;
        PositionLaser();
    }
    // Update is called once per frame
    void Update()
    {
        SeePlayer = PlayerIsSeen();

        //If Player is seen, update players' bool as well
  //      if (SeePlayer)
//            playerScript.VisibleToEnemy = true;
        //else
            //playerScript.VisibleToEnemy = false;
    }

    void FixedUpdate()
    {
    }

    void LateUpdate()
    {
        PositionLaser();
    }

    void PositionLaser()
    {
        //Makes the end points of the sensors follow the conductors
        IR.transform.position = Vector3.zero;
        LaserBeam.SetPosition(0, ConductorA.position);
        LaserBeam.SetPosition(1, ConductorB.position);

        //Vector2[] vertices = new Vector2[2];
        //vertices[0] = transform.TransformPoint(ConductorA.position);
        //vertices[1] = transform.TransformPoint(ConductorB.position);

        ////Keep the collider adjusted correctly
        //collider.points = vertices;
    }


    public bool PlayerIsSeen()
    {
        //If the object is inside of the viewing radius
        if (IsInLineOfSight)
        {
            //Cast one ray from either side
            Ray2D conA = new Ray2D(ConductorA.position, ConductorB.position - ConductorA.position);
            Ray2D conB = new Ray2D(ConductorB.position, ConductorA.position - ConductorB.position);

            RaycastHit2D conductorHitA = Physics2D.Raycast(conA.origin, conA.direction, Vector3.Distance(ConductorA.position, ConductorB.position), LayersToRayscan.value);
            RaycastHit2D conductorHitB = Physics2D.Raycast(conB.origin, conB.direction, Vector3.Distance(ConductorA.position, ConductorB.position), LayersToRayscan.value);

            //Make sure player's hit by both conductors
            if ((conductorHitA.collider != null && conductorHitA.collider.tag == "Player") || (conductorHitB.collider != null && conductorHitB.collider.tag == "Player"))
            {
                SeePlayer = true;

                //Get the ray between the conductor and the player
                Ray2D hitRayA = new Ray2D(ConductorA.position, conductorHitA.point - (Vector2)ConductorA.position);
                Ray2D hitRayB = new Ray2D(ConductorB.position, conductorHitB.point - (Vector2)ConductorB.position);

                if (Debugging)
                {
                    Debug.DrawRay(hitRayA.origin, hitRayA.direction * Vector3.Distance(ConductorA.position, conductorHitA.point), Color.white);
                    Debug.DrawRay(hitRayB.origin, hitRayB.direction * Vector3.Distance(ConductorB.position, conductorHitB.point), Color.white);
                }

                //Avoid re-assigning the color on every frame
                if (LaserColor != DetectionColor)
                LaserColor = DetectionColor;

                return true;
            }
        }
        else
        {
            SeePlayer = false;

            //Avoid re-assigning the color on every frame
            if(LaserColor != NeutralColor)
                LaserColor = NeutralColor;
        }

        return false;
    }



    public bool IsInLineOfSight = false;
    void OnTriggerEnter2D(Collider2D other)
    {
        //If player enters viewing radius
        if (other.tag == "Player")
        {
            IsInLineOfSight = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        //If player leaves viewing radius
        if (other.tag == "Player")
        {
            IsInLineOfSight = false;
        }
    }
}

