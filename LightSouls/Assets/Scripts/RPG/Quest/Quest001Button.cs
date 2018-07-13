using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Quest001Button : MonoBehaviour {

    public GameObject ThePlayer;
    public GameObject NoticeCam;
    public GameObject UIQuest;
    //public GameObject ActiveQuestBox;
    public GameObject Objective01;
    public GameObject ExMark;
    public GameObject Necro;


    public void AcceptQuest() {
        QuestManager.Quest001Taken = true;
        Cursor.visible = false;
        ThePlayer.SetActive(true);
        NoticeCam.SetActive(false);
        UIQuest.SetActive(false);
        ExMark.SetActive(false);
        Necro.SetActive(true);
        StartCoroutine(SetQuestUI());
    }

    private IEnumerator SetQuestUI() {
        Objective01.GetComponent<Text>().text = "Find the house in the clearing.";
        yield return new WaitForSeconds(0.5f);
        Objective01.SetActive(true);
        yield return new WaitForSeconds(5f);
        Objective01.SetActive(false);
    }

}
