using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA {

    public class StateManager : MonoBehaviour {

        [Header("Init")]
        public GameObject activeModel;

        [Header("Inputs")]
        public float vertical;
        public float horizontal;
        public float moveAmount;
        public Vector3 moveDir;
        public bool rt, rb, lt, lb; //, b, a, y, x;
        public bool rollInput;



        [Header("Stats")]
        public float moveSpeed = 3;
        public float runSpeed = 8.0f;
        public float rotateSpeed = 5;
        public float toGround = 0.5f;
        public float rollSpeed = 1;

        [Header("States")]
        public bool onGround;
        public bool run;
        public bool lockOn;
        public bool inAction;
        public bool canMove;
        public bool isTwoHanded;

        [Header("Other")]
        public EnemyTarget lockOnTarget;
        public Transform lockOnTransform;
        public AnimationCurve roll_curve;


        [HideInInspector]
        public Animator anim;
        [HideInInspector]
        public Rigidbody rigid;
        [HideInInspector]
        public AnimatorHook a_hook;

        [HideInInspector]
        public float delta;
        [HideInInspector]
        public LayerMask ignoreLayers;

        float _actionDelay;

        public void Init() {


            SetupAnimator();
            //rigid body is attached to controller, gets it. set some variables.
            rigid = GetComponent<Rigidbody>();
            rigid.angularDrag = 999;
            rigid.drag = 4;
            rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            //gets the animation hook from boxman, init it with states manager
            a_hook = activeModel.AddComponent<AnimatorHook>();
            a_hook.Init(this);

            //sets some variables for layering
            gameObject.layer = 8;
            ignoreLayers = ~(1 << 9);

            //default onground starts true
            anim.SetBool("onGround", true);
            //anim.SetBool("run", false);


        }

        void SetupAnimator() {

            if (activeModel == null) {
                //states manager belongs to controller so children is boxman.
                anim = GetComponentInChildren<Animator>();
                if (anim == null) {
                    Debug.Log("No Model Found");
                } else {
                    //animator is attached to boxman so it returns boxman
                    activeModel = anim.gameObject;
                }
            }

            //seems reducdant.
            if (anim == null) {
                anim = activeModel.GetComponent<Animator>();
            }

            //make it so animations dont move character.
            anim.applyRootMotion = false;

        }

        public void FixedTick(float d) {

            //set local delta as passed delta.
            delta = d;

            //play any action if you have any.
            DetectAction();

            //if you did an action.
            if (inAction) {

                //you can move with action.
                anim.applyRootMotion = true;

                //action delay add the delta. 
                _actionDelay += delta;
                if (_actionDelay > 1.1f) {
                    // when delay reached, set inaction back false. reset delay
                    inAction = false;
                    _actionDelay = 0;
                } else {
                    //if in action but delay not reached, return.
                    return;
                }
            }

            //if not in action, get canMove from animator.
            canMove = anim.GetBool("canMove");

            //if you can't move, return
            if (!canMove) {
                return;
            }

            //a_hook.rm_multi = 1;
            a_hook.CloseRoll();
            HandleRolls();


            //if you can move and not in action, animation can't move u.
            anim.applyRootMotion = false;
            // if you're falling or moving, you have no rigidbody drag, otherwise 4
            rigid.drag = (moveAmount > 0 || onGround == false) ? 0 : 4;
            //rigid.drag = 0;

            //conditional for movement speeds.
            float targetSpeed = moveSpeed;
            if (run) {
                targetSpeed = runSpeed;
            }

            //if on ground, rigid body gets the moveDir vector multipled by (speed+amount)
            if(onGround)
                rigid.velocity = moveDir * (targetSpeed * moveAmount);

            //if run, lockon false (not used yet)
            if (run)
                lockOn = false;

            //new direction variable
            Vector3 targetDir = (lockOn == false) ? moveDir 
                : (lockOnTransform != null) ? lockOnTransform.transform.position - transform.position 
                : moveDir;

            //set the y as 0. in case.
            targetDir.y = 0;
            if (targetDir == Vector3.zero) {
                //Debug.Log(targetDir);
                targetDir = transform.forward;
            }

            //get Rotational direction of target direction.
            Quaternion tr = Quaternion.LookRotation(targetDir);
            //create a new target rotation using slerp(current, goal, speed)
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, delta * moveAmount * rotateSpeed);
            //set the boxman controller's rotation to slerp over there.
            transform.rotation = targetRotation;


            anim.SetBool("lockon", lockOn);

            if (lockOn == false) {
                //do animations.
                HandleMovementAnimations();
            } else {
                HandleLockOnAnimations(moveDir);
            }
        }

        public void DetectAction() {

            //if can't move return.
            if (canMove == false)
                return;

            //if no actions return.
            if (rb == false && rt == false && lb == false && lt == false)
                return;

            //for now.
            string targetAnim = null;

            //whatever button, set it as that.
            if (rb)
                targetAnim = "oh_attack_2";

            if (rt) {

                if (!isTwoHanded) {
                    targetAnim = "oh_attack_1";
                } else {
                    targetAnim = "th_attack_1";

                }


            }


            if (lb)
                targetAnim = "oh_attack_3";

            if (lt) {

                if (!isTwoHanded) {
                    targetAnim = "oh_attack_2";
                } else {
                    targetAnim = "th_attack_2";

                }

            }

            //incase.
            if (string.IsNullOrEmpty(targetAnim)) {
                return;
            }

            //if going to play action, you now cant move and you are in action.
            canMove = false;
            inAction = true;
            //play animation of action.
            anim.CrossFade(targetAnim, 0.3f);
            //rigid.velocity = Vector3.zero;


        }

        public void Tick(float d) {
            //localize delta
            delta = d;
            //
            onGround = OnGround();

            //if (!onGround) {
            //    run = false;
            //}

            anim.SetBool("onGround", onGround);
            //anim.SetBool("run", run);


        }

        public void HandleRolls() {
            if (!rollInput)
                return;
            Debug.Log(rollInput);
            float v = vertical;
            float h = horizontal;
            v = (moveAmount > 0.3f) ? 1 : 0;
            h = 0;

            /*if (lockOn == false) {

                v = (moveAmount > 0.3f) ? 1 : 0;
                h = 0;

            } else {
                if (Mathf.Abs(v) < 0.3f)
                    v = 0;
                if (Mathf.Abs(h) < 0.3f)
                    h = 0;
            }
            */
            if (v != 0) {
                if (moveDir == Vector3.zero)
                    moveDir = transform.forward;

                Quaternion targetRot = Quaternion.LookRotation(moveDir);
                transform.rotation = targetRot;
                a_hook.InitForRoll();
                a_hook.rm_multi = rollSpeed;


            } else {

                a_hook.rm_multi = 2f;

            }


            anim.SetFloat("vertical", v);
            anim.SetFloat("horizontal", h);

            canMove = false;
            inAction = true;
            anim.CrossFade("Rolls", 0.3f);


        }

        void HandleMovementAnimations() {
            //run as run.
            anim.SetBool("run", run);
            //don't imediately set the amount to amount but over 0.4f duration.
            //takes 0.4f to start animation.
            anim.SetFloat("vertical", moveAmount, 0.4f, delta);
        }

        public void HandleLockOnAnimations(Vector3 moveDir) {
            Vector3 relativeDir = transform.InverseTransformDirection(moveDir);
            float h = relativeDir.x;
            float v = relativeDir.z;

            anim.SetFloat("vertical", v, 0.2f, delta);
            anim.SetFloat("horizontal", h, 0.2f, delta);

        }


        public bool OnGround() {
            //default is false.
            bool r = false;
            //the boxman controller's positon plus a little higher up.
            Vector3 origin = transform.position + (Vector3.up * toGround);
            //dir is down?
            Vector3 dir = -Vector3.up;
            //length of ray
            float dis = toGround + 0.1f;
            RaycastHit hit;
            Debug.DrawRay(origin, dir * dis);
            //projects a ray from origin downwards and if it doesnt it anything, if statement doesnt run
            if (Physics.Raycast(origin, dir, out hit, dis, ignoreLayers)) {
                //if projected ray hits something, 
                r = true;
                //if it hitsm we want our position to be right same as hit point. else we sink due to some params we have.
                Vector3 targetPosition = hit.point;
                transform.position = targetPosition;
            }

            return r;
        }

        public void HandleTwoHanded() {
            //set animations twohand as whatever it is.
            anim.SetBool("two_handed", isTwoHanded);
        }


    }

}
