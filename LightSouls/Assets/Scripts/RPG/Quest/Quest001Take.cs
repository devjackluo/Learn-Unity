using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest001Take : MonoBehaviour {

    public float TheDistance;
    public GameObject ActionDisplay;
    public GameObject ActionText;
    public GameObject UIQuest;
    public GameObject ThePlayer;
    public GameObject NoticeCam;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        TheDistance = PlayerCasting.DistanceFromTarget;

        //TheDistance = Vector3.Distance(ThePlayer.transform.position, transform.position);

        if (!QuestManager.Quest001Taken) {

            if (TheDistance <= 2) {

                ActionDisplay.SetActive(true);
                ActionText.SetActive(true);

                if (Input.GetButtonDown("A")) {
                    if (TheDistance <= 2) {

                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;

                        ActionText.SetActive(false);
                        ActionDisplay.SetActive(false);
                        UIQuest.SetActive(true);
                        NoticeCam.SetActive(true);
                        ThePlayer.SetActive(false);
                    }
                }

            } else {

                ActionDisplay.SetActive(false);
                ActionText.SetActive(false);

            }

        } else {

            ActionDisplay.SetActive(false);
            ActionText.SetActive(false);

        }



    }

    /*
     void OnMouseOver() {

        

    }

     void OnMouseExit() {
        ActionText.SetActive(false);
        ActionDisplay.SetActive(false);
    }*/


}
