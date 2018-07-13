using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA {

    public class CameraManager : MonoBehaviour {

        public static CameraManager singleton;

        public bool lockon;
        public float followSpeed = 9;
        public float mouseSpeed = 3;
        public float controllerSpeed = 3;

        public Transform target;
        public EnemyTarget lockonTarget;
        public Transform lockonTransform;

        [HideInInspector]
        public Transform pivot;
        [HideInInspector]
        public Transform camTrans;

        StateManager states;

        float turnSmoothing = .1f;
        public float minAngle = -35;
        public float maxAngle = 35;

        float smoothX;
        float smoothY;
        float smoothXVelocity;
        float smoothYVelocity;

        public float lookAngle;
        public float tiltAngle;

        bool usedRightAxis;


        public void Init(StateManager st) {

            states = st;

            //localize boxman controller transform. 
            target = st.transform;
            //Unity lets us get main camera transform
            camTrans = Camera.main.transform;
            //the main camera's parent which is pivot (emptiest folder)
            pivot = camTrans.parent;
        }

        public void Tick(float d) {

            //get input for mosue x and y axis
            float h = Input.GetAxis("Mouse X");
            float v = Input.GetAxis("Mouse Y");
            
            //get x and y axis for xbox controller
            float c_h = Input.GetAxis("RightAxis X");
            float c_v = Input.GetAxis("RightAxis Y");

            //speed default mouse.
            float targetSpeed = mouseSpeed;

            if (lockonTarget != null) {

                if (lockonTransform == null) {
                    lockonTransform = lockonTarget.GetTarget();
                    states.lockOnTransform = lockonTransform;

                }

                if (Mathf.Abs(c_h) > 0.6f) {
                    if (!usedRightAxis) {
                        lockonTransform = lockonTarget.GetTarget((c_h > 0));
                        states.lockOnTransform = lockonTransform;
                        usedRightAxis = true;
                    } 

                }

            }

            if (usedRightAxis) {
                if (Mathf.Abs(c_h) < 0.6f) {
                    usedRightAxis = false;
                }
            }


            //if input values for xbc not 0, then hori and vert is set to xbc's and speed
            if (c_h != 0 || c_v != 0) {
                h = c_h;
                v = c_v;
                targetSpeed = controllerSpeed;
            }

            //pass delta 
            FollowTarget(d);
            //pass delta, vert, hori, speed
            HandleRotations(d, v, h, targetSpeed);

        }

        void FollowTarget(float d) {
            //speed of camera follow relative to delta.
            float speed = d * followSpeed;
            //slerp the vector from current position(cam holder), to boxman controller with speed.
            Vector3 targetPosition = Vector3.Lerp(transform.position, target.position, speed);
            //new position based on slerp(increment) amount.
            transform.position = targetPosition;
        }

        void HandleRotations(float d, float v, float h, float targetSpeed) {
            //something smoothing, seems always first if atm... how smooth to move rotations.
            //Debug.Log(smoothXVelocity);
            //smoothVelocity getting ref changed somehow..
            if (turnSmoothing > 0) {
                smoothX = Mathf.SmoothDamp(smoothX, h, ref smoothXVelocity, turnSmoothing);
                smoothY = Mathf.SmoothDamp(smoothY, v, ref smoothYVelocity, turnSmoothing);

            } else {
                smoothX = h;
                smoothY = v;
            }

            //rotate x-axis for pivot
            tiltAngle -= smoothY * targetSpeed;
            //clamp the x-rotate to not go pass a certain point.
            tiltAngle = Mathf.Clamp(tiltAngle, minAngle, maxAngle);
            pivot.localRotation = Quaternion.Euler(tiltAngle, 0, 0);




            if (lockon && lockonTarget != null) {

                Vector3 targetDir = lockonTransform.position - transform.position;
                targetDir.Normalize();
                //targetDir.y = 0;



                if (targetDir == Vector3.zero)
                    targetDir = transform.forward;
                Quaternion targetRot = Quaternion.LookRotation(targetDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, d * 9);

                lookAngle = transform.eulerAngles.y;

                return;

            } 

            //float of a direction...
            lookAngle += smoothX * targetSpeed;

            //rotate only around the y-axis for holder
            transform.rotation = Quaternion.Euler(0, lookAngle, 0);


        }

        

        void Awake() {

            singleton = this;

        }

    }

}
