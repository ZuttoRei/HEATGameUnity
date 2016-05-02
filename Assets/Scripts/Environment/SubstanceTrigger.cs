using UnityEngine;
using System.Collections.Generic;

public class SubstanceTrigger : MonoBehaviour
{
    public float Duration;
    public float SpeedDebuff;
    public List<string> VulnerableEnemies;
    public List<string> TagsThatExplodeToFire;
    public Material BurnMaterial;

    public FootprintTrailer.PrintType _SubstanceType;
    public FootprintTrailer.PrintType SubstanceType
    {
        get { return _SubstanceType; }
        set
        {
            _SubstanceType = value;
            if (value == FootprintTrailer.PrintType.Fire)
            {
                GetComponent<Renderer>().sortingOrder = 0;
            }
        }
    }


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            //Kill player if it touches fire
            if (SubstanceType == FootprintTrailer.PrintType.Fire)
                other.gameObject.GetComponent<CharControls>().Disintegrate();
            else //Otherwise only slow them
                other.gameObject.GetComponent<FootprintTrailer>().Start(Duration, SubstanceType, SpeedDebuff);
        }
            //If an object that's vulnerable to fire, touches fire, explode it
        else if (SubstanceType == FootprintTrailer.PrintType.Fire && TagsThatExplodeToFire.Contains(other.gameObject.tag))
        {
            Explodable expl = other.gameObject.GetComponent<Explodable>();
            if (expl != null)
                expl.Explode();
        }
        //If oil touches fire... turn it into fire
        if (SubstanceType == FootprintTrailer.PrintType.Fire && other.gameObject.tag == "Oil")
        {
            other.gameObject.GetComponent<Renderer>().material = BurnMaterial;
            other.gameObject.GetComponent<SubstanceTrigger>().SubstanceType = FootprintTrailer.PrintType.Fire;
        }
    }
    void OnTriggerStay2D(Collider2D other)
    {
        //Enemies that are vulnerable to this substance also take a hit
        //but only as long as they are inside of the puddle
        if (VulnerableEnemies.Contains(other.gameObject.tag))
        {
            other.gameObject.GetComponent<AI_SharedVariables>().MovementSpeedPenalty = SpeedDebuff;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        //Enemies that are vulnerable to this substance also take a hit
        //but only as long as they are inside of the puddle
        if (VulnerableEnemies.Contains(other.gameObject.tag))
        {
            other.gameObject.GetComponent<AI_SharedVariables>().MovementSpeedPenalty = 0;
        }
    }
}
