using UnityEngine;
using System.Collections.Generic;

public class AI_Camera : MonoBehaviour {

    [Range(0, 360)]
    public float ScanAngle;
    [Range(0, 20)]
    public float ScanRotationDuration;
    Transform _body;

    Animator anim;
    VisionScan scanner;
    CharControls playerScript;

    public List<Transform> LinkedAI;

	// Use this for initialization
	void Start () {
        _body = transform.Find("Camera");
        scanner = GetComponentInChildren<VisionScan>();
        anim = GetComponent<Animator>();
        playerScript = GameObject.Find("Player").GetComponent<CharControls>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        CheckDetection();
        ScanArea();
	}

    bool LinkedAINotified = false;
    private void CheckDetection()
    {
        if(scanner.Detected && !LinkedAINotified)
        {
            anim.SetTrigger("Detected");   
            LinkedAINotified = true;
            foreach(Transform AI in LinkedAI)
            {
                //Make all chained AI apprehend the player
                AI.GetComponent<AI_SharedVariables>().Apprehending = true;
            }
        }
    }

    private float _Time;
    void ScanArea()
    {
        //Look at player if detected
        if (scanner.Detected)
        {
            _body.rotation = Quaternion.RotateTowards(transform.rotation, playerScript.PlayerDirection(transform.position), 100f);
        }
        else
        {
            _Time = _Time + Time.deltaTime;
            float phase = Mathf.Sin(_Time / ScanRotationDuration);
            _body.localRotation = Quaternion.Slerp(_body.localRotation, Quaternion.Euler(new Vector3(0, 0, phase * ScanAngle / 2)), 0.03f);
        }
    }
}
