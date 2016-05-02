using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Linq;

public class CharControls : MonoBehaviour
{
    //Movement variables
    public float SneakSpeed = 3;
    public float WalkSpeed = 7;
    public float SprintSpeed = 10;
    public float CurrentSpeed = 6;
    public float SpeedPenaltyPercentage = 0;
    public bool IncapacitateMovementOnVisible;
    public bool Sprinting;

    //Don't allow sprinting everywhere
    public bool CanSprint = true;

    public float HP;
    public float SneakViewRadius = 1.5f;
    public float IndicatorRadius = 2f;

    Vector3 Movement;
    AssetReferences assets;

    //The ID of the current enemy tracking foot prints, used to ensure only one enemy tracks footprints at any given time
    public int CurrentFootPrintTracer { get; set; }

    //Image effects
    Grayscale grayscale;
    ColorCorrectionCurves ccc;
    NoiseAndGrain NAG;


    MeshRenderer[] Indicators;

    void Awake()
    {
        assets = GetComponent<AssetReferences>();
        grayscale = Camera.main.GetComponent<Grayscale>();
        ccc = Camera.main.GetComponent<ColorCorrectionCurves>();
        NAG = Camera.main.GetComponent<NoiseAndGrain>();

        Indicators = GameObject.FindGameObjectsWithTag("Indicator").Select(f => f.GetComponent<MeshRenderer>()).ToArray();
    }

    public bool IsWalking
    {
        get
        {
            if (Movement.x > 0 || Movement.y > 0 || Movement.x < 0 || Movement.y < 0)
                return true;
            else
                return false;
        }
    }

    //Items
    public GameObject EmpMine;
    public int EMPMines = 3;

    //Animator
    public Animator anim;

    //Visibility/detection variables
    public bool VisibleToEnemy = false;
    //Cap at when player will set off the alarm
    public float DetectionTreshold = 100;
    //Current detection status, if this reaches treshold alarms will go off
    public float DetectionPoints = 0;
    //Rate at which your detectionrate goes up for every frame you are seen. Better gear should lower this value
    public float DetectionRate = 2;
    //Rate at which your detection drops each frame
    public float DetectionDecay = 1f;
    public bool FaceCursor = false;

    public bool InputDisabled = false;
    public bool _Detected;
    public bool Detected
    {
        get { return _Detected; }
        set
        {
            _Detected = value;
            //Change players animation
            //anim.SetTrigger("Detected");
            //DetectionPoints = 0;
        }
    }
    public bool _Dead;
    public bool Dead
    {
        get { return _Dead; }
        set
        {
            NAG.enabled = false;
            Sneaking = false;
            _Dead = value;
            _Detected = value;
            GetComponent<FootprintTrailer>().enabled = false;
            GetComponent<PolygonCollider2D>().enabled = false;
        }
    }


    /*Death functions */
    public void Splatter()
    {
        anim.SetTrigger("Splattered");
        GenerateBlood();
        //Move player down a bit to make sure he renders underneath the dozer
        transform.position = transform.position + new Vector3(0, 0, 0.1f);
    }

    public void NormalKill()
    {
        //GenerateBlood();
        anim.SetTrigger("Dead");
        Dead = true;
    }

    public void Dissolve()
    {
        anim.SetTrigger("Dissolve");
        Dead = true;
    }

    public void Disintegrate()
    {
        //Move player down a unit to show them underneath the effects
        //transform.position += new Vector3(0, 0, 0.1f);
        anim.SetBool("Electrocuted", true);
        GetComponent<Explodable>().Explode(true, false);
        Dead = true;
    }

    public void StickToDrill(Vector3 drill)
    {
        anim.SetTrigger("DrillStruggle");
        FacePosition(drill);
    }

    public void BrainSplatter()
    {
        Dead = true;
        anim.SetTrigger("Splattered");
        GenerateBlood();
    }

    public void Crush()
    {
        Dead = true;
        anim.SetTrigger("Headshot");
        transform.position = transform.position + new Vector3(0, 0, 0.1f);
        GenerateBlood();
    }


    void GenerateBlood()
    {
        //Two blood scripts, one to render the blood underneath the player and one for on top of the player
        SpawnBlood[] bloodScripts = GetComponents<SpawnBlood>();
        bloodScripts[0].MakeBlood();
        bloodScripts[1].MakeBlood();
    }

    public void LeakBloodPuddle()
    {
        Instantiate(assets.Assets[1], transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
    }


    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        ReloadLevel();

        if (Dead)
        {
            grayscale.effectAmount = Mathf.Lerp(grayscale.effectAmount, 1, 0.01f);
        }

        //Don't allow movement when the player is detected or dead
        if (InputDisabled || Dead)
            return;

        DecayDetectionMeter();
        ReadActionsInput();
        ReadMovementInput();
    }


    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), "HP: " + HP.ToString());
        GUI.Label(new Rect(10, 30, 100, 20), "Detection: " + DetectionPoints.ToString());
    }


    private void SwitchLights(bool IsEnabled)
    {
        //Hide lights that are out of range
        foreach(Light light in FindObjectsOfType<Light>())
        {
            if (light != null && light.name == "PlayerLight")
                continue;


            float distance = Vector3.Distance(transform.position, light.gameObject.transform.position);

            if(!IsEnabled)
            {
                if (distance > SneakViewRadius)
                    light.enabled = false;
                else
                    light.enabled = true;
            }
            else
            {
                light.enabled = true;
            }
        }
    }

    private void SetIndicatorVisibility()
    {
        //Hide indicators that are out of range
        foreach (MeshRenderer indicator in Indicators)
        {
            if (indicator == null)
                continue;

            if (GetDistance(indicator.gameObject.transform.position) > IndicatorRadius)
            {
                indicator.enabled = false;
            }
            else
            {
                indicator.enabled = true;
            }
        }
    }


    float GetDistance(Vector3 _object)
    {
        return Vector3.Distance(transform.position, _object);
    }


    private bool _Sneaking;
    public float SneakDetectionLimit;
    public bool Sneaking
    {
        get { return _Sneaking; }
        set 
        {
            if (value == true && DetectionPoints < SneakDetectionLimit)
            {
                anim.SetBool("Sneaking", true);
                //grayscale.effectAmount = Mathf.Lerp(grayscale.effectAmount, 1, 0.1f);
                NAG.intensityMultiplier = Mathf.Lerp(NAG.intensityMultiplier, 0.64f, 0.05f);
                ccc.enabled = true;
                ccc.saturation = Mathf.Lerp(ccc.saturation, 1.25f, 0.05f);
                anim.speed = 1f;
                RenderSettings.ambientIntensity = Mathf.Lerp(RenderSettings.ambientIntensity, 0.1f, 0.15f);

                SwitchLights(false);
                SetIndicatorVisibility();

                _Sneaking = true;

            }
            else
            {
                anim.SetBool("Sneaking", false);
                NAG.intensityMultiplier = Mathf.Lerp(NAG.intensityMultiplier, 0f, 0.05f);
                ccc.enabled = false;
                ccc.saturation = 0;
                RenderSettings.ambientIntensity = Mathf.Lerp(RenderSettings.ambientIntensity, 0.35f, 0.15f);

                SwitchLights(true);
                SetIndicatorVisibility();

                _Sneaking = false;
            }
        }
    }
    

    void MovementSpeedManager()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Sneaking = true;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            if(CanSprint)
                Sprinting = true;
        }
        else
        {
            Sprinting = false;
            Sneaking = false;
        }



        if (Sneaking)
        {
            CurrentSpeed = SneakSpeed;
        }
        else if (Sprinting && CanSprint)
        {
            CurrentSpeed = SprintSpeed;
            anim.speed = SprintSpeed / WalkSpeed;
        }
        else
        {
            CurrentSpeed = WalkSpeed;
            anim.speed = 1f;
        }


        //If movementspeed is debuffed
        if (SpeedPenaltyPercentage > 0)
        {
            //Apply movement debuff modifier, no running allowed
            CurrentSpeed = WalkSpeed - WalkSpeed / 100 * SpeedPenaltyPercentage;

            //Adjust animation as well
            anim.speed = 1 - (SpeedPenaltyPercentage / 150);
        }
    }

    void ReadMovementInput()
    {
        MovementSpeedManager();

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");


        //If any movement is detected
        if (horizontal > 0 || vertical > 0 || horizontal < 0 || vertical < 0)
        {
            // Rotate player
            float angle = Mathf.Atan2(horizontal, vertical) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 180, Vector3.back);

            anim.SetBool("Walking", true);
        }
        else
        {
            anim.SetBool("Walking", false);
        }


        Movement = new Vector3(horizontal, vertical, 0);

        this.transform.position += Movement * CurrentSpeed * Time.deltaTime;
    }

    void ReadActionsInput()
    {
        if (Input.GetKeyUp(KeyCode.Q))
        {
            placeEmpMine();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            //   Interact();
            Application.LoadLevel(0);
        }
    }

    void ReloadLevel()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            //   Interact();
            Application.LoadLevel(0);
        }
    }

    void DecayDetectionMeter()
    {
        DetectionPoints = Mathf.Clamp(DetectionPoints - DetectionDecay, 0, DetectionTreshold);
    }

    void FollowCursor()
    {
        if (FaceCursor)
        {
            Vector3 mouseScreen = Input.mousePosition;
            Vector3 mouse = Camera.main.ScreenToWorldPoint(mouseScreen);

            transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(mouse.y - transform.position.y, mouse.x - transform.position.x) * Mathf.Rad2Deg - 90);
        }
    }



    public void FacePosition(Vector3 position)
    {
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(position.y - transform.position.y, position.x - transform.position.x) * Mathf.Rad2Deg - 270);
    }

    void Interact()
    {
        anim.SetTrigger("Interact");
    }

    void placeEmpMine()
    {
        if (EMPMines > 0 && EmpMine != null)
        {
            EMPMines--;
            Instantiate(EmpMine, transform.position, Quaternion.identity);
        }
    }

    public Quaternion PlayerDirection(Vector3 position, float offset = 90)
    {
        return Quaternion.Euler(0, 0, Mathf.Atan2(transform.position.y - position.y, transform.position.x - position.x) * Mathf.Rad2Deg - offset);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(CharControls))]
public class DrawSneakViewRadius : Editor
{
    CharControls playerScript;

    public void OnEnable()
    {
        Handles.color = Color.green;
        playerScript = (CharControls)target;
    }


    void OnSceneGUI()
    {
        Handles.color = Color.green;
        Handles.CircleCap(0, playerScript.transform.position, playerScript.transform.rotation, playerScript.SneakViewRadius);

        Handles.color = Color.red;
        Handles.CircleCap(0, playerScript.transform.position, playerScript.transform.rotation, playerScript.IndicatorRadius);
    }

}
#endif