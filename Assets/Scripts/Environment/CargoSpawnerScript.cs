using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class CargoSpawnerScript : MonoBehaviour {


    //List of all items to be spawned
    public List<Transform> SpawnableObjects;
    public bool SwitchedOn;
    public State _state;
    public State state
    {
        get { return _state; }
        set 
        {
            _state = value; 

            switch(value)
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
    
    //A collection, int stands for the item in the SpawnableObjects list, float for the
    //delay between the current and next index
    public ItemSpawnSet[] SpawnOrder;

    public List<Sprite> machineSprites;

    SpriteRenderer renderer;
    int CurrentItem = 0;
    float elapsed;

	void Start () {
        renderer = transform.Find("ConveyerBoxGraphic").GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        if (SwitchedOn)
            ItemSpawner();
        else
            state = State.Orange;
	}


    void ItemSpawner()
    {
        //Avoid errors if there's nothing in the list
        if (SpawnableObjects.Count == 0)
            return;

        elapsed += Time.deltaTime;

        //If current item's spawn time has passed
        if (elapsed >= SpawnOrder[CurrentItem].ItemDelaySeconds)
        {
            //Set timer back to zero
            elapsed = 0;

            //Spawn the item
            SpawnItem();
        }
    }

    public void SpawnItem()
    {
        //Create the item
        Instantiate(SpawnableObjects[SpawnOrder[CurrentItem].ItemIndex], new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
        StartCoroutine(SetStates(new State[] { State.Green, State.Red }, SpawnOrder[CurrentItem].ItemDelaySeconds / 4)); //Always switch the lights on for a quarter the duration of the spawn delay

        //Calculate next item to use, if the last item is reached, go back to the first
        CurrentItem = CurrentItem + 1== SpawnOrder.Length ? 0 : CurrentItem + 1;
    }

    public IEnumerator SetStates(State[] states, float seconds)
    {
        foreach(State state_ in states)
        {
            state = state_;
            yield return new WaitForSeconds(seconds);
        }
    }
}

public enum State
{
    Off,
    Red,
    Orange,
    Green
}

[Serializable]
public struct ItemSpawnSet
{
    public int ItemIndex;
    [Range(0.1f, 100)]
    public float ItemDelaySeconds;
}
