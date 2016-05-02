using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class SpawnBlood : MonoBehaviour {


    [Range(0, 3)]
    public float Radius;
    [Range(0, 200)]
    public int BloodSpots;
    public int LayerOrder = 0;
    [Range(0f, 1f)]
    public float MinSize;
    [Range(0f, 1f)]
    public float MaxSize;
    [Range(0f, 1f)]
    public float DelayPerBloodCloth = 0.01f;
    public List<Transform> bloodPrefabs;

    public bool MakeChild = true;
    public bool SpawnOnStart = false;
    public bool DisableLight = true;
    public float ZDepth;


    // Use this for initialization
    void Start()
    {
        if (SpawnOnStart)
        {
            MakeBlood();
        }
    }


    public void MakeBlood()
    {
        StartCoroutine(GenerateBlood());
    }

    IEnumerator GenerateBlood()
    {
        if (DisableLight)
        {
            var children = new List<GameObject>();
            foreach (Transform child in transform) children.Add(child.gameObject);
            children.Where(f => f.tag != "PlayerLight").ToList().ForEach(child => Destroy(child));
        }

        for (int i = 0; i < BloodSpots; i++)
        {
            Quaternion rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
            Transform t = Instantiate(GetRandomBloodPrefab(), GetRandomPosition(), Quaternion.identity) as Transform;
            if (MakeChild)
            {
                t.parent = transform;
            }
            t.GetComponent<SpriteRenderer>().sortingOrder = LayerOrder;
            t.transform.rotation = rotation;
            t.position = new Vector3(t.position.x, t.position.y, ZDepth);
            t.transform.localScale = new Vector3(Random.Range(MinSize, MaxSize), Random.Range(MaxSize, MaxSize), 1);
            yield return new WaitForSeconds(DelayPerBloodCloth);
        }
    }

    Transform GetRandomBloodPrefab()
    {
        return bloodPrefabs[Random.Range(0, bloodPrefabs.Count - 1)];
    }

    Vector3 GetRandomPosition()
    {
        //return new Vector3(Random.Range(transform.position.x - Radius, transform.position.x + Radius), Random.Range(transform.position.y - Radius, transform.position.y + Radius));
        return transform.position + (Vector3)Random.insideUnitCircle * Radius;
    }


	// Update is called once per frame
	void Update () {
        
	}
}
