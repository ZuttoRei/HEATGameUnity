using UnityEngine;
using System.Collections.Generic;

public class CargoBeltExitScript : MonoBehaviour {
    SpriteRenderer renderer;
    Light light;

    public float LightIntensity = 3f;

    public State _state;
    public State state
    {
        get { return _state; }
        set
        {
            _state = value;

            switch (value)
            {
                case State.Off:
                    renderer.sprite = machineSprites[0];
                    break;

                case State.Red:
                    renderer.sprite = machineSprites[1];
                    break;

                case State.Orange:
                    renderer.sprite = machineSprites[2];
                    break;

                case State.Green:
                    renderer.sprite = machineSprites[3];
                    break;
            }

        }
    }

    public List<Sprite> machineSprites;

	// Use this for initialization
	void Start () {
        renderer = GetComponentInChildren<SpriteRenderer>();
        light = GetComponentInChildren<Light>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Crate")
        {
            state = State.Green;
            light.intensity = LightIntensity;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Crate")
        {
            state = State.Red;
            light.intensity = 0;
        }
    }

    public enum State
    {
        Off,
        Red,
        Orange,
        Green
    }
}

