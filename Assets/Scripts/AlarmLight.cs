using UnityEngine;
using System.Collections;

public class AlarmLight : MonoBehaviour {

	// Use this for initialization
	void Start () {
        light = GetComponent<Light>();
	}

    public bool Enabled = true;
    public float Speed;
    Light light;
    float elapsed;

    float minIntensity = 0;
    public float maxIntensity = 0.3f;

	// Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime * Speed;

        if (Enabled)
        {
            if ((light.intensity >= maxIntensity) || (light.intensity <= minIntensity)) // Hit an edge?
                Speed = -Speed;            // Reverse direction.

            light.intensity += Speed;
        }

    }
}
