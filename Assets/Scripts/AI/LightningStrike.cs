using UnityEngine;
using System.Collections;

public class LightningStrike : MonoBehaviour
{
    GameObject target;
    public Transform impactEffect;
    public GameObject BurnedAshesEffect;
    public LineRenderer lineRend;
    public float arcLength = 1.0f;
    public float arcVariation = 1.0f;
    public float inaccuracy = 0.5f;
    public float timeOfZap = 0.25f;
    public bool Zap;

    bool Started = false;

    Transform _impactEffect;

    void Start()
    {
        lineRend = gameObject.GetComponent<LineRenderer>();
        lineRend.SetVertexCount(1);
        lineRend.enabled = true;
        target = GameObject.Find("Player");
    }

    void LateUpdate()
    {
        if (Zap)
        {
            if (!Started)
            {
                Started = true;
                _impactEffect = Instantiate(impactEffect, target.transform.position + new Vector3(0, 0, -1f), Quaternion.identity) as Transform;
            }

            Vector3 lastPoint = transform.position;
            int i = 1;
            lineRend.SetPosition(0, transform.position);//make the origin of the LR the same as the transform
            while (Vector3.Distance(target.transform.position, lastPoint) > 0.5f)
            {//was the last arc not touching the target?
                lineRend.SetVertexCount(i + 1);//then we need a new vertex in our line renderer
                Vector3 fwd = target.transform.position - lastPoint;//gives the direction to our target from the end of the last arc
                fwd.Normalize();//makes the direction to scale
                fwd = Randomize(fwd, inaccuracy);//we don't want a straight line to the target thoaugh
                fwd *= Random.Range(arcLength * arcVariation, arcLength);//nature is never too uniform
                fwd += lastPoint;//point + distance * direction = new point. this is where our new arc ends
                lineRend.SetPosition(i, fwd);//this tells the line renderer where to draw to
                i++;
                lastPoint = fwd;//so we know where we are starting from for the next arc
            }
            lineRend.SetVertexCount(i + 1);
            lineRend.SetPosition(i, target.transform.position);
        }
        else
        {
            lineRend.SetVertexCount(1);
        }
    }

    public void DestroyThis()
    {
        Destroy(_impactEffect.gameObject);
        lineRend.enabled = false;
        this.enabled = false;
    }

    private Vector3 Randomize(Vector3 newVector, float devation)
    {
        newVector += new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(0f, 1.0f)) * devation;
        newVector.Normalize();
        return newVector;
    }

}