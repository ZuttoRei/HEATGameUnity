using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AI_Charger : MonoBehaviour {

    //Waypoints to visit
    public List<Vector3> Waypoints;
    Transform player;
    CharControls playerScript;
    PolyNavAgent agent;
    Hibernation hibernationScript;
    Animator anim;
    VisionScan scanner;

    //List to encapsulate certain variables across other enemies
    AI_SharedVariables AIVars;

    public float RotationSpeed;
    //Apprehension variables
    public bool Apprehending = false;
    public float NormalSpeed;
    public float ApprehensionSpeed;
    [Range(0, 1)]
    public float ChaseVisionTreshold;
    
    [Range(0, 10)]
    public float ChargeSpeed;

    //Bool to keep track whether we're subscribed to the OnDestinationReached event
    private bool _Patrolling;
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

    public bool IsHibernating
    {
        get { return hibernationScript.IsHibernating; }
    }

    //The waypoint the AI is currently traveling to
    int currentWayPoint = 0;


    void Awake()
    {
        
    }

    // Use this for initialization
    void Start()
    {
        //Instantiate pathfinding agent
        agent = GetComponent<PolyNavAgent>();
        Patrolling = true;

        //To reference players' variables
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerScript = player.GetComponent<CharControls>();
        hibernationScript = GetComponent<Hibernation>();
        scanner = GetComponentInChildren<VisionScan>();
        AIVars = GetComponent<AI_SharedVariables>();
        anim = GetComponent<Animator>();
        NavigateNextWayPoint();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsHibernating)
            return;

        DecideActions();
    }

    void DecideActions()
    {

        float detectionPoints = scanner._DetectionPoints;

        if (Apprehending && !playerScript.Dead || scanner.SightLevel > ChaseVisionTreshold)
        {
            agent.SetDestination(player.position);
            //agent.maxSpeed = ApprehensionSpeed - ApprehensionSpeed / 100 * AIVars.MovementSpeedPenalty;
        }
        else
        {
            Apprehending = false;
            GoToWayPoint();
            //Keep movement speed penalty into account
            //agent.maxSpeed = NormalSpeed - NormalSpeed / 100 * AIVars.MovementSpeedPenalty;

        }


        
        agent.maxSpeed = Mathf.Lerp(agent.maxSpeed, ApprehensionSpeed + (ChargeSpeed * detectionPoints), 0.1f);
        agent.maxSpeed = agent.maxSpeed - agent.maxSpeed / 100 * AIVars.MovementSpeedPenalty;
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

    void OnDestinationReached()
    {
        NavigateNextWayPoint();
    }

    void SplatterBlood()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collider)
    {
        if (!hibernationScript.Hibernating)
        {
            if (collider.gameObject.tag == "Player" && !playerScript.Dead)
            {
                StartCoroutine(KillPlayer());
            }
        }
    }


    bool BloodStarted = false;
    void StartEmittingBlood()
    {
        if (!BloodStarted)
        {
            BloodStarted = true;
            ParticleSystem[] bloodParticleEffects = transform.Find("Blood").GetComponentsInChildren<ParticleSystem>();

            foreach (ParticleSystem ps in bloodParticleEffects)
            {
                ps.Play();

            }
        }
    }

    void LateUpdate()
    {
        if(LockPlayerPosition)
        {
            player.localPosition = pos;
        }
    }


    bool LockPlayerPosition = false;
    Vector3 pos;
    public IEnumerator KillPlayer()
    {
        transform.Find("BloodBack").GetComponent<ParticleSystem>().Play();
        playerScript.InputDisabled = true;
        player.parent = transform;
        player.localPosition = new Vector3(0, 1.914f, 0);
        playerScript.StickToDrill(transform.position);
        player.GetComponent<Collider2D>().enabled = false;
        player.GetComponent<SpriteRenderer>().sortingOrder = 5;
        agent.maxSpeed = NormalSpeed;
        LockPlayerPosition = true;

        //Slowly slide player over the drill
        for (int i = 0; i < 10; i++)
        {
            pos = Vector3.Lerp(player.localPosition, new Vector3(0, 0, 0), 0.05f);
            player.localPosition = pos;
            yield return new WaitForSeconds(0.05f);
        }

        anim.SetBool("KilledPlayer", true);
        playerScript.Dead = true;
        Destroy(player.GetComponent<SpriteRenderer>());

        foreach (SpawnBlood blood in GetComponents<SpawnBlood>())
        {
            blood.MakeBlood();
        }
    }


    //Returns the angle to this object from the players current position. Used to make player face the dozer when getting...dozed
    Quaternion Direction
    {
        get
        {
            return Quaternion.Euler(0, 0, Mathf.Atan2(transform.position.y - player.position.y, transform.position.x - player.position.x) * Mathf.Rad2Deg - 270);
        }
    }
}
