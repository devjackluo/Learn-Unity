using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NecroWalkAI : MonoBehaviour {

    public int Xpos;
    public int Zpos;
    public GameObject NPCDest;

	// Use this for initialization
	void Start () {
        Xpos = Random.Range(105, 140);
        Zpos = Random.Range(140, 160);
        NPCDest.transform.position = new Vector3(Xpos, 0f, Zpos);
        StartCoroutine(RunRandomWalk());
    }

    // Update is called once per frame
    void Update () {
        transform.LookAt(NPCDest.transform);
        transform.position = Vector3.MoveTowards(transform.position, NPCDest.transform.position, 0.02f);
	}

    IEnumerator RunRandomWalk() {
        yield return new WaitForSeconds(10);
        Xpos = Random.Range(50, 90);
        Zpos = Random.Range(35, 70);
        NPCDest.transform.position = new Vector3(Xpos, NPCDest.transform.position.y, -Zpos);
        StartCoroutine(RunRandomWalk());
    }



}
