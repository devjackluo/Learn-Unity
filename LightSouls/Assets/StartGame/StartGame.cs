using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour {

    public GameObject Camera1;
    public GameObject Camera2;
    public GameObject ThePlayer;
    public GameObject PlayerCam;

    public GameObject Skip;
    public bool Skipped;
    public GameObject GameTitle;
    public GameObject Info;

    private Coroutine routine;


    // Use this for initialization
    void Start () {
        //Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
        routine = StartCoroutine(CutSceneStart());
    }

    // Update is called once per frame
    void Update () {

        if (Input.GetButtonDown("Jump") && !Skipped) {
            ThePlayer.SetActive(true);
            PlayerCam.SetActive(true);
            StopCoroutine(routine);
            EndCut();
            //EndScene = true;
        }

    }

    IEnumerator HowToPlay() {
        Skipped = true;
        Info.SetActive(true);
        yield return new WaitForSeconds(25f);
        Info.SetActive(false);

    }

    IEnumerator CutSceneStart() {
        Skip.SetActive(true);
        GameTitle.SetActive(true);
        yield return new WaitForSeconds(5f);
        Camera2.SetActive(true);
        Camera1.SetActive(false);
        yield return new WaitForSeconds(5f);
        Skip.SetActive(false);
        GameTitle.SetActive(false);
        ThePlayer.SetActive(true);
        PlayerCam.SetActive(true);
        Camera2.SetActive(false);
        StartCoroutine(HowToPlay());
    }

    void EndCut() {
        ThePlayer.SetActive(true);
        PlayerCam.SetActive(true);
        Camera1.SetActive(false);
        Camera2.SetActive(false);
        Skip.SetActive(false);
        GameTitle.SetActive(false);
        StartCoroutine(HowToPlay());
    }


}
