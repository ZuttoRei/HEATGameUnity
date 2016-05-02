using UnityEngine;
using System.Collections;

public class DestroyOnEnter : MonoBehaviour {

    public bool RequireRigidbody = true;
    public bool DestroyPlayer = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D body = other.gameObject.GetComponent<Rigidbody2D>();

        //Only destroy objects with a rigidbody
        if (body == null && RequireRigidbody)
            return;

        if (!DestroyPlayer && other.gameObject.tag == "Player")
            return;

        Destroy(other.gameObject);
    }
}
