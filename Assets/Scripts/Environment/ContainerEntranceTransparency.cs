using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ContainerEntranceTransparency : MonoBehaviour {

    List<SpriteRenderer> renderers = new List<SpriteRenderer>();

    [Range(0, 100)]
    public float TransparencyLevel;
    public float fadeSpeed;

	// Use this for initialization
	void Start () {
        renderers.Add(GetComponentInParent<SpriteRenderer>());
        renderers.AddRange(GetComponentsInChildren<SpriteRenderer>());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            StartCoroutine(fadeOut(TransparencyLevel));
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            StartCoroutine(fadeIn(100));
        }
    }

    IEnumerator fadeOut(float percent)
    {
        foreach (SpriteRenderer render in renderers)
        {
            //If the door has been destroyed, don't bother.
            if (render == null)
                continue;

            float alpha = render.material.color.a;
            Color color = render.material.color;

            while ((alpha > (TransparencyLevel / 100)))
            {
                alpha -= Time.deltaTime * fadeSpeed;
                render.material.color = new Color(color.r, color.g, color.b, alpha);
                yield return null;
            }
        }
    }

    IEnumerator fadeIn(float percent)
    {
        foreach (SpriteRenderer render in renderers)
        {
            //If the door has been destroyed, don't bother.
            if (render == null)
                continue;

            float alpha = render.material.color.a;
            Color color = render.material.color;

            while ((alpha < percent / 100))
            {
                alpha += Time.deltaTime * fadeSpeed;
                render.material.color = new Color(color.r, color.g, color.b, alpha);
                yield return null;
            }
        }
    }
}
