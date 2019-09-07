using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{

    public Collider boxCollider, targetCollider;
    public float maxDistanceToWin = 2.0f;
    public GameObject box;

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(boxCollider.bounds.center, targetCollider.bounds.center);
        //print("Distance to other: " + dist);
        if (dist < maxDistanceToWin)
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().puzzleDone = true;
            box.transform.parent = null;
            Destroy(this.gameObject);
        }
    }
}
