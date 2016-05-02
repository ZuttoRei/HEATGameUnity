using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class ConveyerBeltScript : MonoBehaviour {


    public bool IsRunning;
    public float ForcePerSpeed;
    public float BeltAnimationSpeed;

    List<Animator> BeltAnims;
    Animator MainBeltAnim;
    AreaEffector2D effector;


    //Physical properties for items on the belt
    Rigidbody2D physicalProperties;


    //List that keeps track of the items on the list. Items on the belt must all have the same
    //physical properties, so this list stores the items and reverts the values once they leave the belt.
    Dictionary<int, ImportantRigidbodyValues> ItemsOnBelt = new Dictionary<int, ImportantRigidbodyValues>();

	// Use this for initialization
	void Start () {
        effector = GetComponent<AreaEffector2D>();
        BeltAnims = GetComponentsInChildren<Animator>().ToList();
        BeltAnims.RemoveAt(0); //Exclude own animator, we only want the children
        MainBeltAnim = GetComponent<Animator>();
        physicalProperties = GetComponent<Rigidbody2D>();
	}

	void FixedUpdate () {

        MainBeltAnim.speed = BeltAnimationSpeed;

        if (IsRunning)
            RunBelt();
        else
            StopBelt();
	}

    void RunBelt()
    {
        effector.forceMagnitude = ForcePerSpeed * BeltAnimationSpeed;
        BeltAnims.ForEach(f => f.speed = BeltAnimationSpeed);
    }

    void StopBelt()
    {
        effector.forceMagnitude = 0;
        BeltAnims.ForEach(f => f.speed = 0);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Store others' objects properties
        Rigidbody2D otherBody = other.gameObject.GetComponent<Rigidbody2D>();

        //Only allow items with a rigidbody
        if (otherBody == null)
            return;

        //Precaution to make sure it doesn't get added twice
        if (ItemsOnBelt.ContainsKey(other.gameObject.GetInstanceID()))
            return;

        ImportantRigidbodyValues irv = new ImportantRigidbodyValues();
        irv.Mass = otherBody.mass;
        irv.Drag = otherBody.drag;
        irv.AngularDrag = otherBody.angularDrag;

        ItemsOnBelt.Add(other.gameObject.GetInstanceID(), irv);

        //Reassign them
        otherBody.mass = physicalProperties.mass;
        otherBody.angularDrag = physicalProperties.angularDrag;
        otherBody.drag = physicalProperties.drag;
    }
    void OnTriggerExit2D(Collider2D other)
    {
        Rigidbody2D otherBody = other.gameObject.GetComponent<Rigidbody2D>();

        //Only allow items with a rigidbody
        if (otherBody == null)
            return;

        ImportantRigidbodyValues originalBody = ItemsOnBelt.Where(f => f.Key == other.gameObject.GetInstanceID()).FirstOrDefault().Value;

        //Revert back to original values
        otherBody.mass = originalBody.Mass;
        otherBody.angularDrag = originalBody.AngularDrag;
        otherBody.drag = originalBody.Drag;

        //Discard from list
        ItemsOnBelt.Remove(other.gameObject.GetInstanceID());
    }
}

//Cheaper to only store the vital information
struct ImportantRigidbodyValues
{
    public float Mass;
    public float AngularDrag;
    public float Drag;
}