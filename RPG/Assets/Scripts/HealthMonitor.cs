using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthMonitor : MonoBehaviour {

    public static int HealthValue;
    public int InternalHealth;
    public GameObject Heart1;
    public GameObject Heart2;
    public GameObject Heart3;


    // Use this for initialization
    void Start () {
        HealthValue = 1;
	}
	
	// Update is called once per frame
	void Update () {
        InternalHealth = HealthValue;

        if (HealthValue == 1) {
            Heart1.SetActive(true);
        }
        else if (HealthValue == 2){
            Heart2.SetActive(true);
        }
        else if (HealthValue == 3){
            Heart3.SetActive(true);
        }

    }
}
