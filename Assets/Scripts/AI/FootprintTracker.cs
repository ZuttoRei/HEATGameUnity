using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class FootprintTracker : MonoBehaviour {


    PolyNavAgent agent;
    CharControls playerScript;
    VisionScan scanner;

    public float RotateSpeed;
    public float TrackingSpeed;

    private float OriginalRotationSpeed;
    private float OriginalTrackingSpeed;
    private float OriginalDecelerationRate;
    private float OriginalSlowingDistance;

    public bool Patrol { get; set; }

	// Use this for initialization
	void Start () {
        agent = GetComponent<PolyNavAgent>();
        scanner = GetComponentInChildren<VisionScan>();
        playerScript = GameObject.Find("Player").GetComponent<CharControls>();

        OriginalRotationSpeed = agent.rotateSpeed;
        OriginalTrackingSpeed = agent.maxSpeed;
        OriginalDecelerationRate = agent.decelerationRate;
        OriginalSlowingDistance = agent.slowingDistance;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    int CurrentFootPrintTrack = 0;
    public bool FootPrintTrackingStarted = false;

    List<GameObject> FootWaypoints;
    List<GameObject> steps;

    public void TrackFootPrints()
    {
        if (!FootPrintTrackingStarted)
        {
            //Start tracking footprints
            FootPrintTrackingStarted = true;
            agent.OnDestinationReached += FootPrintReached;
            agent.rotateSpeed = RotateSpeed;
            agent.decelerationRate = 0;
            agent.slowingDistance = 0;
            agent.maxSpeed = TrackingSpeed;

            steps = playerScript.GetComponent<FootprintTrailer>().Steps;

            //Remove all footsteps made before the first detected footstep
            steps.Where(f => steps.IndexOf(f) < scanner.FirstDetectedFootStep).ToList().ForEach(x => Destroy(x));
            steps.Where(f => steps.IndexOf(f) < scanner.FirstDetectedFootStep).ToList().ForEach(x => playerScript.GetComponent<FootprintTrailer>().Steps.Remove(x));
        }

        //Player found? Cancel tracking
        if(scanner.SeePlayer)
        {
            StopTrackingFootPrints();
        }

        steps = playerScript.GetComponent<FootprintTrailer>().Steps;
        FootWaypoints = steps;

        if(CurrentFootPrintTrack < FootWaypoints.Count)
        {
            agent.SetDestination(FootWaypoints[CurrentFootPrintTrack].transform.position);
        }
        else
        {
            StopTrackingFootPrints();
        }
    }
    void FootPrintReached()
    {
        Destroy(FootWaypoints[CurrentFootPrintTrack]);

        CurrentFootPrintTrack++;
    }

    public void StopTrackingFootPrints()
    {
        playerScript.GetComponent<FootprintTrailer>().Steps.ForEach(x => Destroy(x)); //Destroy any left overs
        scanner.HasDetectedFootPrints = false;
        FootPrintTrackingStarted = false;
        agent.OnDestinationReached -= FootPrintReached;
        agent.rotateSpeed = OriginalRotationSpeed;
        agent.maxSpeed = OriginalTrackingSpeed;
        agent.decelerationRate = OriginalDecelerationRate;
        agent.slowingDistance = OriginalSlowingDistance;

        //Notify other bots that no one is tracking footprints anymore, in case new ones are found
        playerScript.CurrentFootPrintTracer = 0;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
    }
}
