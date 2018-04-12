using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartCollect : MonoBehaviour {

    // Use this for initialization
    //void Start () {

    //}

    public int RotateSpeed = 2;
    public AudioSource CollectSound;
    public GameObject ThisHeart;
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(0, RotateSpeed, 0, Space.World);
	}

    private void OnTriggerEnter(Collider other){
        CollectSound.Play();
        HealthMonitor.HealthValue += 1;
        this.ThisHeart.SetActive(false);
    }

}
