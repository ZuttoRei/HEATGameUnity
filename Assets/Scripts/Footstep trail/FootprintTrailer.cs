using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class FootprintTrailer : MonoBehaviour
{
    public bool Enabled;
    [Range(0f, 1f)]
    public double PrintRatePerMovementspeedUnit = 0.405;
    private double PrintRate;
    public float FadeTimeInSeconds;
    public float MovementPenalty;
    public PrintType FootprintType;
    public GameObject OilPrint, BloodPrint, OozePrint;
    public Vector2 OffsetLeftFoot = new Vector2(-0.376f, -0.573f);
    public Vector2 OffsetRightFoot = new Vector2(0.352f, -0.576f);
    Transform player;
    CharControls playerScript;
    public List<GameObject> Steps;


    //A container for all the footstep objects to keep the hierachy clean and tidy
    GameObject FootStepParent;

    //Keep track of direction
    bool Right;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerScript = player.GetComponent<CharControls>();
        Steps = new List<GameObject>();
        FootStepParent = new GameObject("Footstep parent");
	}

    public void Start(float DurationSeconds, PrintType Type, float SpeedPenalty)
    {
        StartCoroutine(EnableFootSteps(DurationSeconds, Type, SpeedPenalty));
    }

    IEnumerator EnableFootSteps(float DurationSeconds, PrintType Type, float SpeedPenalty)
    {
        //Only allow to run once
        if (!Enabled)
        {
            FootprintType = Type;
            Enabled = true;
            playerScript.SpeedPenaltyPercentage = SpeedPenalty; //Debuff movement
            yield return new WaitForSeconds(DurationSeconds);
            Enabled = false;
            playerScript.SpeedPenaltyPercentage = 0; //Undo movement debuff
        }
    }

    private float Elapsed; //Used to track footstep rate
    private float ElapsedSecondsSinceLastPrint; //Used to track decay time
    void FixedUpdate()
    {
        if (!Enabled)
            return;

        ElapsedSecondsSinceLastPrint += Time.deltaTime;
        DecayFootPrints();

        //Only make moves if player is actually moving
        if (!playerScript.anim.GetBool("Walking"))
            return;

        Elapsed += Time.deltaTime;

        if (Elapsed > PrintRate)
        {
            SetRate();
            Elapsed = 0;
            MakeFootPrint();
        }
    }

    void SetRate()
    {
        PrintRate = PrintRatePerMovementspeedUnit / playerScript.CurrentSpeed;
    }

    void DecayFootPrints()
    {
        if(ElapsedSecondsSinceLastPrint >= FadeTimeInSeconds)
        {
            ElapsedSecondsSinceLastPrint = 0;
            if(Steps.Count > 0)
            {
                GameObject obj = Steps.FirstOrDefault();
                Destroy(obj);
                Steps.Remove(obj);
            }
        }
    }


    //Add a collider to every n footstep to save performance. Better than placing a collider on every single step
    public int ColliderRate = 1;
    int ColliderCount;

    public void MakeFootPrint()
    {
        Quaternion rotation = player.rotation * Quaternion.Euler(0, 0, 180);
        ColliderCount++;

        if(Right)
        {
            //Calculate right footstep relative to players current position
            Vector3 position = player.transform.TransformPoint(OffsetLeftFoot);

            if (FootprintType == PrintType.Blood)
            {
                Steps.Add(Instantiate(BloodPrint, position, rotation) as GameObject);
            }
            else if (FootprintType == PrintType.Oil)
            {
                Steps.Add(Instantiate(OilPrint, position, rotation) as GameObject);
            }
            else if (FootprintType == PrintType.Ooze)
            {
                Steps.Add(Instantiate(OozePrint, position, rotation) as GameObject);
            }
            Right = false;
        }
        else
        {
            //Calculate left footstep relative to players current position
            Vector3 position = player.transform.TransformPoint(OffsetRightFoot);

            if (FootprintType == PrintType.Blood)
            {
                Steps.Add(Instantiate(BloodPrint, position, rotation) as GameObject);
            }
            else if (FootprintType == PrintType.Oil)
            {
                Steps.Add(Instantiate(OilPrint, position, rotation) as GameObject);
            }
            else if (FootprintType == PrintType.Ooze)
            {
                Steps.Add(Instantiate(OozePrint, position, rotation) as GameObject);
            }
            Right = true;
        }

        //Place it in our footstep container
        GameObject step = Steps.LastOrDefault();
        step.transform.parent = FootStepParent.transform;
        step.AddComponent<CircleCollider2D>().isTrigger = true;


        //Add a collider to every nth footstep
        if(ColliderCount >= ColliderRate)
        {
            ColliderCount = 0;
            
        }
    }

    public enum PrintType
    {
        Oil,
        Blood,
        Ooze,
        Fire
    }

}
