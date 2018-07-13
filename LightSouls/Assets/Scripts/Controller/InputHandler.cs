using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA {

    public class InputHandler : MonoBehaviour {

        float vertical;
        float horizontal;
        bool b_input;
        bool run_input;
        bool a_input;
        bool x_input;
        bool y_input;

        bool rb_input;
        float rt_axis;
        bool rt_input;
        bool lb_input;
        float lt_axis;
        bool lt_input;

        bool leftAxis_down;
        bool rightAxis_down;

        StateManager states;
        CameraManager camManager;

        float delta;

        //#############################################
        //#############################################
        //WHERE IT ALL STARTS
        //#############################################
        //#############################################

        void Start() {

            //THIS IS ATTACHED TO BOXMAN controller, therefore we can get states manager.
            states = GetComponent<StateManager>();
            states.Init();

            //Get the static Camera manager in itself. passing in boxmancontroller transforms.
            camManager = CameraManager.singleton;
            camManager.Init(states);
        }

        //FANCY UPDATE UNITY FUNCTION THAT ALWAYS CALLED AT SAME INTERVAL, USED FOR RIGID BODIES
        void FixedUpdate() {
            
            //get time since last fixupdate call.
            delta = Time.fixedDeltaTime;
            //get values for all buttons.
            GetInput();

            //get all the current states. mhmm
            UpdateStates();
            //pass in delta
            states.FixedTick(delta);
            //pass delta 
            camManager.Tick(delta);

        }

        //UPDATE 
        void Update() {
            //get delta since last update
            delta = Time.deltaTime;
            //pass delta. checks to see if on ground and stuff.
            states.Tick(delta);
        }

        void GetInput() {

            vertical = Input.GetAxis("Vertical");
            horizontal = Input.GetAxis("Horizontal");
            b_input = Input.GetButton("B");
            run_input = Input.GetButton("Fire2");
            a_input = Input.GetButton("A");
            y_input = Input.GetButtonUp("Y");
            x_input = Input.GetButton("X");


            rt_input = Input.GetButton("RT");
            rt_axis = Input.GetAxis("RT");
            if (rt_axis != 0) {
                rt_input = true;
            }

            lt_input = Input.GetButton("LT");
            lt_axis = Input.GetAxis("LT");
            if (lt_axis != 0) {
                lt_input = true;
            }


            rb_input = Input.GetButton("RB");


            lb_input = Input.GetButton("LB");

            rightAxis_down = Input.GetButtonUp("L");
            //leftAxis_down = Input.GetButton("L");

            //Debug.Log(lt_input);

            

        }

        void UpdateStates() {

            //set the states manager's hori and vert as input values
            states.horizontal = horizontal;
            states.vertical = vertical;

            //take the vert and horizontal values and multiply it by camera's transforms
            //camManager is attached to camera holder.
            Vector3 v = vertical * camManager.transform.forward;
            Vector3 h = horizontal * camManager.transform.right;
            //Debug.Log(camManager.transform.right);

            //add the two vectors and normalized it. (13.21, 10.21, 7.5) - > (1, 0.8, 0.5) etc.
            states.moveDir = (v + h).normalized;
            //take absolute value of how much you moved the inputs together
            float m = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
            //set state man's moveamount as previous value clamped. make it no more/less than 0 to 1
            states.moveAmount = Mathf.Clamp01(m);

            states.rollInput = b_input;
            //if b, set states run.
            if (run_input) {
                states.run = (states.moveAmount > 0);
            } else {
                //else run false.
                states.run = false;
            }

            //set state manager's locals as inputs
            //states.b = b_input;
            states.rt = rt_input;
            states.rb = rb_input;
            states.lt = lt_input;
            states.lb = lb_input;

            //if y, twohand is opposite
            if (y_input) {
                states.isTwoHanded = !states.isTwoHanded;
                //handle it in state manager.
                states.HandleTwoHanded();
            }

            if (rightAxis_down) {
                states.lockOn = !states.lockOn;
                if (states.lockOnTarget == null) {
                    states.lockOn = false;
                }
                camManager.lockonTarget = states.lockOnTarget;
                states.lockOnTransform = camManager.lockonTransform;
                camManager.lockon = states.lockOn;


            }


        }

    }

}
