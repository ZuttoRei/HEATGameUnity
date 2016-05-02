using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class AI_Gropey : MonoBehaviour {

    //Delay in seconds determining how often the AI will investigate a suspicious area
    public int InvestigationTimeOutInSeconds = 4;
    //Treshold when the AI will grow wary and start to investigate the players' area
    public int InvestigationTreshold = 20;
    //Waypoints to visit
    public List<Vector3> Waypoints;
    Transform player;
    //Spot where gropey will throw player into
    public Transform DeliveryLocation;
    CharControls playerScript;
    PolyNavAgent agent;
    VisionScan scanner;
    
    //Initial light intensity, in case we need to revert back to them
    float[] lightIntensities;

    //The waypoint the AI is currently traveling to
    int currentWayPoint = 0;

    //Variables for kill animation
    public bool IsHoldingPlayer = false;
    public bool KillSequenceStarted = false;



	// Use this for initialization
	void Start () {
        //Instantiate pathfinding agent
        agent = GetComponent<PolyNavAgent>();
        agent.OnDestinationReached += OnDestinationReached;

        //To reference players' variables
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerScript = player.GetComponent<CharControls>();

        //Get the child scanner of this object
        scanner = GetComponentInChildren<VisionScan>();

        lightIntensities = GetComponentsInChildren<Light>(true).Select(f => f.intensity).ToArray();

        //Start patrolling
        NavigateNextWayPoint();
	}
	
	// Update is called once per frame
    void FixedUpdate()
    {
        CheckPlayerDetectionState();

        //Keep player in arms of robot
        if (IsHoldingPlayer)
        {
            player.localPosition = Vector3.zero;
            player.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 180));
        }
    }

    void CheckPlayerDetectionState()
    {
        //Don't investigate if player is detected already

        if (scanner.DetectionPoints > InvestigationTreshold)
        {
            agent.SetDestination(player.position);            
        }
        else if(IsHoldingPlayer)
        {
            agent.SetDestination(DeliveryLocation.position);
        }
        else
        {
            agent.SetDestination(Waypoints[currentWayPoint - 1]);
        }
        
    }


    public void PickUpPlayer()
    {
        playerScript.anim.SetTrigger("Grabbed");
        //Bool used to lock players' position to gropey's position and rotation
        IsHoldingPlayer = true;
        //Make it a child object of gropey
        player.parent = transform;
        //We won't need the players collider anymore
        player.GetComponent<PolygonCollider2D>().enabled = false;
    }

    public void DropPlayer()
    {
        player.GetComponent<PolygonCollider2D>().enabled = true;
        IsHoldingPlayer = false;
        //Detatch from gropey
        player.parent = null;
    }

    public void PowerOff(float HibernationTime)
    {
        StartCoroutine(Off(HibernationTime));
    }

    public void PowerOn()
    {
        StartCoroutine(On());
    }

    IEnumerator Off(float HibernationTime)
    {
        Light[] lights = GetComponentsInChildren<Light>(true);

        foreach (Light light in lights)
        {
            while (light.intensity > 0)
            {
                light.intensity -= 0.3f;
                yield return new WaitForEndOfFrame();
            }
            light.enabled = false;
        }
    }

    IEnumerator On()
    {
        Light[] lights = GetComponentsInChildren<Light>(true);


        for (int i = 0; i < lights.Length; i++ )
        {
            lights[i].enabled = true;

            while (lights[i].intensity < lightIntensities[i])
            {
                lights[i].intensity += 0.1f;
                yield return new WaitForEndOfFrame();
            }
        }
    }

    void NavigateNextWayPoint(bool IsContinuation = false)
    {       
        //Make sure waypoints exist
        if (Waypoints.Count == 0)
            return;

        //if (IsContinuation)
            //currentWayPoint++;

        //If final waypoint reached, navigate back to the first, else move on to the next
        currentWayPoint = currentWayPoint >= Waypoints.Count ? 1 : currentWayPoint + 1;


        //Move to current waypoint
        agent.SetDestination(Waypoints[currentWayPoint - 1]);
    }

    void OnDestinationReached()
    {
        if (IsHoldingPlayer)
            DropPlayer();

        NavigateNextWayPoint();
    }
    Quaternion PlayerDirection()
    {
        return Quaternion.Euler(0, 0, Mathf.Atan2(player.position.y - transform.position.y, player.position.x - transform.position.x) * Mathf.Rad2Deg - 90);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.tag == "Player" && !playerScript.Dead)
        {
            //Stop and look towards player
            transform.rotation = PlayerDirection();
            agent.Stop();
            scanner.Detected = true;
            scanner.DetectionPoints = 0;
            //Pick him up
            PickUpPlayer();
            GetComponentInParent<BoxCollider2D>().enabled = false;
        }
    }
}
