using UnityEngine;
using System.Collections;

public class LockObjectPosition : MonoBehaviour {

    Vector3 position;
    void Start()
    {
        position = transform.localPosition;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.localPosition = position;
    }
}
