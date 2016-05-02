using UnityEngine;
using System.Collections.Generic;

public class Visibility : MonoBehaviour {

    private bool _Visible;

    public bool Visible
    {
        get { return _Visible; }
        set 
        {
            _Visible = value; 

            if(value == true)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }
    }


    private void Hide()
    {
        foreach(Transform t in transform)
        {
            t.GetComponent<Renderer>().enabled = false;
            Light[] lights = t.GetComponentsInChildren<Light>();
            foreach(Light light in lights)
                light.enabled = false;
        }
    }

    private void Show()
    {
        foreach (Transform t in transform)
        {
            t.GetComponent<Renderer>().enabled = true;
            Light[] lights = t.GetComponentsInChildren<Light>();
            foreach (Light light in lights)
                light.enabled = true;
        }
    }

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
