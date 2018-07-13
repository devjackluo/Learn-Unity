using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCasting : MonoBehaviour {

    public static float DistanceFromTarget = 100;
    public float ToTarget;

	// Update is called once per frame
	void Update () {

        RaycastHit Hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out Hit)) {

            if (Hit.collider.tag == "notice") {
                ToTarget = Hit.distance;
                DistanceFromTarget = ToTarget;
            } else {
                ToTarget = 100;
                DistanceFromTarget = 100;
            }

        }

	}

}
