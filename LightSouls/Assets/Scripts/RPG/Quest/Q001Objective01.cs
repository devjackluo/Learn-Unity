using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Q001Objective01 : MonoBehaviour {

    public GameObject TheObjective;
    public int CloseObjective;
	
	// Update is called once per frame
	void Update () {
        if (CloseObjective == 1) {
            if (TheObjective.transform.localScale.y <= 0.0f)
            {
                CloseObjective = 0;
                TheObjective.SetActive(false);
            }
            else {
                TheObjective.transform.localScale -= new Vector3(0.0f, 0.01f, 0.0f);
                
            }
        }

        if (CloseObjective == 2) {
            TheObjective.GetComponent<Text>().color -= new Color(0.0F, 0.01F, 0.01F);
        }

	}

    void OnTriggerEnter(){
        if (QuestManager.Quest001Taken) {
            GetComponent<BoxCollider>().enabled = false;
            StartCoroutine(FinishObjective());
        }

    }

    IEnumerator FinishObjective() {
        TheObjective.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(TransitionObjective());
    }

    IEnumerator TransitionObjective()
    {
        CloseObjective = 2;
        yield return new WaitForSeconds(3.0f);
        CloseObjective = 1;
    }


}
