using UnityEngine;
using System.Collections;

public class VisionScan : MonoBehaviour {

    public bool Debugging;
    public bool PassiveMode;
    public bool DetectFootPrints;
    public bool RaisePlayerDetection;

    [Range(0f, 1f)]
    public float VisionTreshold;
    
    [Range(0f, 100)]
    public float DetectionModifier;

    public float _DetectionPoints;
    public float DetectionPoints
    {
        get
        {
            return _DetectionPoints;
        }
        set
        {
            //Add the difference to the players' variable to avoid overwriting it when detected by multiple enemies
            _DetectionPoints = value;
        }
    }

    public bool  _Detected;
    public bool  Detected
    {
        get
        {
            return _Detected;
        }
        set
        {
            _Detected = value;
            //Also change the value separatedly in the players' script
            //playerScript.Detected = value;
        }
    }

    public bool HasDetectedFootPrints;

    public float SightLevel
    {
        get
        {
            return GetSightLevel(false);
        }
    }
    
    public bool SeePlayer
    {
        get
        {
            return GetSightLevel(true) > 0 ? true : false;
        }
    }

    //Owner is the parent of the vision control
    Transform player;
    CharControls playerScript;

    //Layers to raycast on
    public LayerMask LayersToRayscan = -1;


    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerScript = player.GetComponent<CharControls>();
    }

    void DebugSettings()
    {
        //Set owner to the parent of the vision
        //transform.GetComponent<SpriteRenderer>().enabled = Debugging;
    }

    // Update is called once per frame
    void Update()
    {
        DebugSettings();

        //If Player is seen, update players' bool as well
        if(playerScript.VisibleToEnemy != SeePlayer)
            playerScript.VisibleToEnemy = SeePlayer;
    }

    void FixedUpdate()
    {
        CalculateDetectionStatus();
    }

    void CalculateDetectionStatus()
    {
        //If the player is detected already, don't bother with changing their detection values
        //if (playerScript.Detected)
            //return;

        //Decay detection points overtime
        DetectionPoints = Mathf.Clamp(DetectionPoints - playerScript.DetectionDecay, 0, playerScript.DetectionTreshold);

        //If we're in passive mode and the player isn't moving, we do not raise detection
        if (PassiveMode && !playerScript.IsWalking)
            return;

        //In passive mode only raise detection when sight level is over specific level
        if (PassiveMode && SightLevel <= VisionTreshold)
            return;

        //Increment detection while player is in true sight
        if (SeePlayer)
        {
           //Do not exceed treshold
            DetectionPoints = Mathf.Clamp(DetectionPoints + DetectionModifier, 0, playerScript.DetectionTreshold);


            if(RaisePlayerDetection)
                playerScript.DetectionPoints = Mathf.Clamp(playerScript.DetectionPoints + DetectionModifier, 0, playerScript.DetectionTreshold);
        }

        //If treshold is reached, player is detected
        if (DetectionPoints >= playerScript.DetectionTreshold)
        {
            Detected = true;
        }
    }


    void LateUpdate()
    {
        //To avoid falling out of sync due to parent being moved by physics
        transform.localPosition = new Vector3(0, 0, 0);
        transform.localRotation = new Quaternion(0, 0, 0, 0);
    }


    public float GetSightLevel(bool Strict) //If strict is set to true, it will only return 1 upon sight.
    {
        //Don't raise suspicion if the player is sneaking and not walking
        if (playerScript.Sneaking && !playerScript.IsWalking)
            return 0;

        float level = 0;

        //If the object is inside of the viewing radius
        if (IsInLineOfSight)
        {
            int Total = player.GetComponent<PolygonCollider2D>().points.Length;

            //Holds our raycast hit
            RaycastHit2D hit = new RaycastHit2D();
            //Iterate through each vertice of our polygon collider
            foreach (Vector2 point in player.GetComponent<PolygonCollider2D>().points)
            {
                //Casts a ray to each vertice of the collider
                Ray2D ray = new Ray2D(transform.position, (Vector2)player.transform.TransformPoint(point) - (Vector2)transform.position);
                hit = Physics2D.Raycast(ray.origin, ray.direction, Vector3.Distance(transform.position, (Vector2)player.transform.TransformPoint(point)), LayersToRayscan.value);

                //If one of the rays collides with the player
                if (hit.collider != null && hit.collider.tag == "Player")
                {
                    //Only display rays when we choose to (debugging purposes)
                    if (Debugging)
                    {
                        Ray2D ray2 = new Ray2D(transform.position, hit.point - (Vector2)transform.position);
                        Debug.DrawRay(ray2.origin, ray2.direction * hit.distance, Color.red);
                    }

                    level++;

                    //Return first positive hit
                    if (Strict)
                        return level;
                }
            }

            //Returns a float between 0 - 1.0 
            return level / Total;
        }
        else
        {
            //Not seen
            return 0;
        }
    }



    public bool IsInLineOfSight = false;
    //The footstep where the AI will start investigating 
    public int FirstDetectedFootStep { get; set; }
    void OnTriggerEnter2D(Collider2D other)
    {
        //If player enters viewing radius
        if (other.tag == "Player")
        {
            IsInLineOfSight = true;
        }
        else if(other.tag == "Footstep")
        {
            if (DetectFootPrints)
            {
                if(!HasDetectedFootPrints)
                {
                    HasDetectedFootPrints = true;
                    FirstDetectedFootStep = playerScript.GetComponent<FootprintTrailer>().Steps.IndexOf(other.gameObject);
                }
            }
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
