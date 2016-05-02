using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AI_Norton : MonoBehaviour {

    //Waypoints to visit
    public List<Vector3> Waypoints;
    public List<Transform> AlarmSwitches;
    private Vector3 NearestbySwitch;

    Transform player;
    Animator anim;
    CharControls playerScript;
    PolyNavAgent agent;
    Hibernation hibernationScript;
    VisionScan scanner;
    FootprintTracker FootTracker;
    public float RotationSpeed;
    //Delay in seconds determining how often the AI will investigate a suspicious area
    public int InvestigationTimeOutInSeconds = 4;
    //Treshold when the AI will grow wary and start to investigate the players' area
    public int InvestigationTreshold = 20;
    private int ID;

    public float NormalSpeed;
    public float RunSpeed;

    //List to encapsulate certain variables across other enemies
    AI_SharedVariables AIVars;

    //Bool to keep track whether we're subscribed to the OnDestinationReached event
    public bool _Patrolling;
    public bool Patrolling
    {
        get { return _Patrolling; }
        set
        {
            _Patrolling = value;
            if (value)
            {
                agent.OnDestinationReached += OnDestinationReached;
            }
            else
            {
                agent.OnDestinationReached -= OnDestinationReached;
            }
        }
    }

    public bool _Investigating;
    public bool Investigating
    {
        get { return _Investigating; }
        set
        {
            _Investigating = value;
            if (value == true)
            {
                agent.OnDestinationReached += OnInvestigationFinished;
            }
            else
            {
                agent.OnDestinationReached -= OnInvestigationFinished;
            }
        }
    }

    public bool _Alarmed;
    public bool Alarmed
    {
        get { return _Alarmed; }
        set
        {
            _Alarmed = value;
            if (value == true)
            {
                agent.OnDestinationReached += RingAlarm;
            }
            else
            {
                agent.OnDestinationReached -= RingAlarm;
            }
        }
    }

    private void RingAlarm()
    {
        print("WEEOOOWEOOOWEOO BEEP BEEP BEEP BOOP BOOP BOOP ALARM!!!!! AHHHHHHHHHHHHHHHHHH!!");
        Destroy(gameObject);
    }


    bool TraversingToAlarm = false;
    private void OnDestinationReached()
    {
        NavigateNextWayPoint();
    }


    public bool IsHibernating
    {
        get { return hibernationScript.IsHibernating; }
    }

    //The waypoint the AI is currently traveling to
    int currentWayPoint = 0;


	// Use this for initialization
	void Start () {
        agent = GetComponent<PolyNavAgent>();
        Patrolling = true;
        //To reference players' variables
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerScript = player.GetComponent<CharControls>();

        //Get the child scanner of this object
        scanner = GetComponentInChildren<VisionScan>();
        hibernationScript = GetComponent<Hibernation>();
        anim = GetComponent<Animator>();
        FootTracker = GetComponent<FootprintTracker>();
        AIVars = GetComponent<AI_SharedVariables>();
        ID = GetInstanceID();

        //Start patrolling
        NavigateNextWayPoint();
	}

    float elapsed;
    float sightLevel;
    Vector3 LastSuspiciousArea;


    Vector3 FurthestSwitchFromPlayer
    {
        get
        {
            var PointsFromPlayer = new Dictionary<float, Vector3>();
            //Get players' distances
            AlarmSwitches.ForEach(f => PointsFromPlayer.Add(Vector3.Distance(f.position, player.transform.position), f.position));

            //Return furthest switch from player
            return PointsFromPlayer[PointsFromPlayer.Keys.Max()];
        }
    }

    float elapsedoil;
	// Update is called once per frame
    void FixedUpdate()
    {
        if (IsHibernating)
        {
            Alarmed = false;
            return;
        }
        elapsed += Time.deltaTime;
        elapsedoil += Time.deltaTime;

        if(scanner.DetectionPoints >= 99)
            Alarmed = true;

        if(Alarmed)
        {
            //Extra bool to make sure we only run this once
            if (!TraversingToAlarm)
            {
                Patrolling = false;
                Investigating = false;
                agent.rotateTransform = true;

                TraversingToAlarm = true;
                RunToAlarm();
            }

            //Limit oil to 1 spot / 0.1 second
            if (elapsedoil >= 0.05f)
            {
                elapsedoil = 0;
                DropOil();
            }
            return;
        }

        sightLevel = scanner.GetSightLevel(false);
        agent.maxSpeed = NormalSpeed;

        //If in sight, look toward player direction
        if (sightLevel >= scanner.VisionTreshold)
        {
            agent.rotateTransform = false;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, playerScript.PlayerDirection(transform.position, 90), RotationSpeed);
        }
        else
        {
            agent.rotateTransform = true;
        }

        DecideActions();

    }

    public void RunToAlarm()
    {
        //Don't call 'NearestSwitch' too often as it performs square root calculations for every
        //Alarm switch on the map, once is enough
        agent.SetDestination(FurthestSwitchFromPlayer);
        anim.SetBool("Alerted", true);
        anim.SetBool("Investigating", false);
        agent.maxSpeed = RunSpeed;
    }

    private void DecideActions()
    {
        //If player has raised 'x' awareness, start investigating the spot where the player raised awareness
        if (elapsed >= InvestigationTimeOutInSeconds)
        {
            if (scanner.DetectionPoints >= InvestigationTreshold)
            {
                if (!Investigating)
                {
                    Investigating = true;
                    LastSuspiciousArea = player.transform.position;
                }
            }
        }


        if (Investigating)
        {
            if (Patrolling)
            {
                Patrolling = false;
                anim.SetBool("Investigating", true);
            }

            agent.SetDestination(LastSuspiciousArea);
        }
        //If foot prints are detected, follow them. (If player is not dead and nobody else is assigned to do so) 
        else if (scanner.HasDetectedFootPrints && !playerScript.Dead && (playerScript.CurrentFootPrintTracer == ID || playerScript.CurrentFootPrintTracer == 0))
        {
            //Stop patrolling
            if (Patrolling)
            {
                Patrolling = false;

                //Take task as primary foot print tracker
                playerScript.CurrentFootPrintTracer = ID;
            }

            FootTracker.TrackFootPrints();
        }
        else
        {
            //Continue patrolling
            if (!Patrolling)
            {
                anim.SetBool("Investigating", false);
                Patrolling = true;
            }

            if (sightLevel >= scanner.VisionTreshold)
            {
                agent.SetDestination(player.transform.position);
            }
            else
            {
                GoToWayPoint();
            }
        }
    }

    private void OnInvestigationFinished()
    {
        elapsed = 0;
        Investigating = false;
    }


    void InvestigateSpot()
    {
        Patrolling = false;
    }

    void NavigateNextWayPoint()
    {
        //Make sure waypoints exist
        if (Waypoints.Count == 0)
            return;

        //If final waypoint reached, navigate back to the first, else move on to the next
        currentWayPoint = currentWayPoint >= Waypoints.Count ? 1 : currentWayPoint + 1;

        //Move to current waypoint
        GoToWayPoint();
    }

    void GoToWayPoint()
    {
        agent.SetDestination(Waypoints[currentWayPoint - 1]);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Player")
            Alarmed = true;
    }

    public Transform substance;
    void DropOil()
    {
        Transform t = Instantiate(substance, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360))) as Transform; //Make it a child of substance container
        Destroy(t.gameObject, 100);
    }
}
