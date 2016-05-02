using UnityEngine;
using System.Collections;

public class LightFlicker : MonoBehaviour {

    Light _light;

	// Use this for initialization
	void Start () {
        _light = GetComponent<Light>();
	}
	
	// Update is called once per frame
	void Update () {
        try
        {
            _light.intensity = Random.Range(4, 8);
        }
        catch
        {
            Destroy(this);
        }
	}
}
