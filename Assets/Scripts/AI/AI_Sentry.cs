using UnityEngine;
using System.Collections;
using System.Linq;

public class AI_Sentry : MonoBehaviour {


    VisionScan scanner;
    BulletScript bulletSettings;
    Transform player;
    CharControls playerScript;
    public float AggressiveRotationSpeed;
    public float BulletSpeed;
    public float BulletDamage;

    public float FocusTreshold;

    [Range(0, 360)]
    public float ScanAngle;
    [Range(0, 20)]
    public float ScanRotationDuration;

    [Range(0.1f, 1)]
    public float ViewTreshold;

    Transform _body;
    Transform _base;
    Animator anim;

    public GameObject BulletProjectile;
    public Transform BarrelLeft;
    public Transform BarrelRight;
    public ParticleSystem muzzleLeft;
    public ParticleSystem muzzleRight;

    public bool TargetInRange
    {
        get
        {
            return scanner.IsInLineOfSight;
        }
    }

    public bool InViewRadius { get; set; }

    public bool InShootSight
    {
        get
        {
            return (InViewRadius && scanner.SightLevel >= ViewTreshold) ? true : false;
        }
    }


	// Use this for initialization
	void Start () {
        //Make body ignore the base
        Physics2D.IgnoreCollision(transform.Find("Sentry body").GetComponent<Collider2D>(), transform.Find("Sentry base").GetComponent<Collider2D>());
        scanner = GetComponentInChildren<VisionScan>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerScript = player.GetComponent<CharControls>();
        _body = transform.Find("Sentry body");
        _base = transform.Find("Sentry base");
        anim = GetComponentInChildren<Animator>();
        muzzleLeft = _body.GetComponentsInChildren<ParticleSystem>()[0];
        muzzleRight = _body.GetComponentsInChildren<ParticleSystem>()[1];
        bulletSettings = BulletProjectile.GetComponent<BulletScript>();
	}

	// Update is called once per frame
    void FixedUpdate()
    {
        bulletSettings.Speed = BulletSpeed;
        bulletSettings.Damage = BulletDamage;
        elapsed += Time.deltaTime;
        DecideActions();
    }


    float elapsed;
    public float RateOfFireDelay = 0.1f;
    bool left = true;
    void FireBullet()
    {
        if (elapsed >= RateOfFireDelay)
        {
            elapsed = 0;
            if (left)
            {
                muzzleLeft.Play();
                left = false;
                Instantiate(BulletProjectile, BarrelLeft.position + new Vector3(0, 0, -1), Quaternion.Euler(_body.transform.rotation.eulerAngles.x, _body.transform.rotation.eulerAngles.y, _body.transform.rotation.eulerAngles.z - 90));
            }
            else
            {
                muzzleRight.Play();
                left = true;
                Instantiate(BulletProjectile, BarrelRight.position + new Vector3(0, 0, -1), Quaternion.Euler(_body.transform.rotation.eulerAngles.x, _body.transform.rotation.eulerAngles.y, _body.transform.rotation.eulerAngles.z - 90));
            }
        }
    }


    public FocusType Focustype;
    
    public enum FocusType
    {
        Level0,
        Level1,
        Level2,
        Level3
    }

    private void DecideActions()
    {
        if (TargetInRange && !playerScript.Dead)
        {
            switch(Focustype)
            {
                case FocusType.Level0:
                    if(InViewRadius && InShootSight)
                    {
                        RotateTowardsPlayer();
                        anim.SetBool("Firing", true);
                        FireBullet();
                    }
                    else
                    {
                        anim.SetBool("Firing", false);
                        ScanArea();
                    }
                    break;
                case FocusType.Level1:
                if(scanner.DetectionPoints >= FocusTreshold)
                {
                    RotateTowardsPlayer();
                    anim.SetBool("Firing", true);
                    FireBullet();
                }
                else
                {
                    anim.SetBool("Firing", false);
                    ScanArea();
                }
                break;

                case FocusType.Level2:
                RotateTowardsPlayer();
                if(InShootSight)
                {
                    anim.SetBool("Firing", true);
                    FireBullet();
                }
                else
                {
                    anim.SetBool("Firing", false);
                }
                break;

                case FocusType.Level3:
                RotateTowardsPlayer();
                    anim.SetBool("Firing", true);
                    FireBullet();
                break;
            }
        }
        else
        {
            anim.SetBool("Firing", false);
            ScanArea();
        }
    }

    private float _Time;
    void ScanArea()
    {
        _Time = _Time + Time.deltaTime;
        float phase = Mathf.Sin(_Time / ScanRotationDuration);
        _body.localRotation = Quaternion.Slerp(_body.localRotation, Quaternion.Euler(new Vector3(0, 0, phase * ScanAngle / 2)), 0.03f);
    }

    void RotateTowardsPlayer()
    {
        Quaternion newRotation = FacePosition(player.position);
        _body.rotation = Quaternion.Slerp(_body.rotation, newRotation, AggressiveRotationSpeed * Time.deltaTime);
    }

    Quaternion FacePosition(Vector3 position)
    {
        return Quaternion.Euler(0, 0, Mathf.Atan2(position.y - transform.position.y, position.x - transform.position.x) * Mathf.Rad2Deg - 90);
    }
}
