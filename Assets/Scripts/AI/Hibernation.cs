using UnityEngine;
using System.Collections;
using System.Linq;

public class Hibernation : MonoBehaviour {

    //Initial light intensity, in case we need to revert back to them
    float[] lightIntensities;
    Animator animator;
    public float HibernationTime;
    public bool Hibernating;
    public bool IsHibernating
    {
        get { return Hibernating; }
        set
        {
            Hibernating = value;
            GetComponent<PolyNavAgent>().Stop();
            GetComponent<PolyNavAgent>().enabled = !value;
            
            VisionScan scan = GetComponentInChildren<VisionScan>();
            if(scan != null)
                GetComponentInChildren<VisionScan>().enabled = !value;
            animator.enabled = !value;
        }
    }

	// Use this for initialization
	void Start () {
        //Saving light intensities so we know what to revert to
        lightIntensities = GetComponentsInChildren<Light>(true).Select(f => f.intensity).ToArray();
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        if (HibernationTime > 0)
        {
            HibernationTime -= Time.deltaTime;
            //Don't do anything if robot is hibernating
            return;
        }

        //Turn on robot if he isn't hibernating
        if (IsHibernating)
        {
            PowerOn();
        }
	}

    public void PowerOff(float HibernationDuration)
    {
        StartCoroutine(Off(HibernationDuration));
    }

    public void PowerOn()
    {
        StartCoroutine(On());
    }

    IEnumerator Off(float HibernationDuration)
    {
        IsHibernating = true;
        HibernationTime = HibernationDuration;

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


        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].enabled = true;

            while (lights[i].intensity < lightIntensities[i])
            {
                lights[i].intensity += 0.1f;
                yield return new WaitForEndOfFrame();
            }
        }

        HibernationTime = 0;
        IsHibernating = false;
    }

}
