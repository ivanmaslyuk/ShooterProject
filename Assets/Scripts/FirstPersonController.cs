using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (CharacterController))]
    [RequireComponent(typeof (AudioSource))]
    [RequireComponent(typeof(Animator))] // аним
    [RequireComponent(typeof(Rigidbody))] // аним
    public class FirstPersonController : MonoBehaviour
    {
        //[SerializeField] private bool m_IsWalking;
        [SerializeField] private float m_WalkSpeed;
        //[SerializeField] private float m_RunSpeed;
        //[SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
        [SerializeField] private float m_JumpSpeed;
        [SerializeField] private float m_StickToGroundForce;
        [SerializeField] private float m_GravityMultiplier;
        [SerializeField] private MouseLook m_MouseLook;
        //[SerializeField] private bool m_UseFovKick;
        //[SerializeField] private FOVKick m_FovKick = new FOVKick();
        //[SerializeField] private bool m_UseHeadBob;
        //[SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
        //[SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
        [SerializeField] private float m_StepInterval;
        [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
        [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.

        // для анимации
        [SerializeField] float m_RunCycleLegOffset = 0.5f; //specific to the character in sample assets, will need to be modified to work with others
        [SerializeField] float m_AnimSpeedMultiplier = 1f;
        [SerializeField] float m_MoveSpeedMultiplier = 1f;
        [SerializeField] float m_GroundCheckDistance = 0.1f;

        private Camera m_Camera;
        private bool m_Jump;
        private float m_YRotation;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private Vector3 m_OriginalCameraPosition;
        private float m_StepCycle;
        private float m_NextStep;
        private bool m_Jumping;
        private AudioSource m_AudioSource;

        // для анимации
        Animator m_Animator;
        bool m_IsGrounded;
        float m_TurnAmount;
        bool m_Crouching;
        float m_ForwardAmount;
        Rigidbody m_Rigidbody;
        const float k_Half = 0.5f;
        float m_CapsuleHeight;
        Vector3 m_CapsuleCenter;
        Vector3 m_GroundNormal;
        float m_OrigGroundCheckDistance;
        //CapsuleCollider m_Capsule;

        // Use this for initialization
        private void Start()
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_Animator = GetComponent<Animator>();
            m_Camera = Camera.main;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            //m_FovKick.Setup(m_Camera);
            //m_HeadBob.Setup(m_Camera, m_StepInterval);
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle/2f;
            m_Jumping = false;
            m_AudioSource = GetComponent<AudioSource>();
			m_MouseLook.Init(transform , m_Camera.transform); 
            m_Crouching = false;
            //m_Capsule = GetComponent<CapsuleCollider>();
            m_CapsuleCenter = m_CharacterController.center;
            m_CapsuleHeight = m_CharacterController.height;
            m_Rigidbody = GetComponent<Rigidbody>();
            m_OrigGroundCheckDistance = m_GroundCheckDistance;

        }


        // Update is called once per frame
        private void Update()
        {
            RotateView();
            // the jump state needs to read here to make sure it is not missed
            if (!m_Jump && !m_Crouching && !m_Jumping)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }

            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
            {
                //StartCoroutine(m_JumpBob.DoBobCycle());
                PlayLandingSound();
                m_MoveDir.y = 0f;
                m_Jumping = false;
            }
            if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
            {
                m_MoveDir.y = 0f;
            }

            m_PreviouslyGrounded = m_CharacterController.isGrounded;


            //GameObject.Find("UMARenderer").layer = 1; // TODO: сделать это по-другому
        }
        
        void UpdateAnimator(Vector3 move)
        {

            //m_ForwardAmount = move.z;
            m_TurnAmount = Mathf.Atan2(move.x, move.z);

            ////////////////
            
            // update the animator parameters
            m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
            m_Animator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);
            m_Animator.SetBool("Crouch", m_Crouching);
            m_Animator.SetBool("OnGround", m_IsGrounded);
            if (!m_IsGrounded)
            {
                m_Animator.SetFloat("Jump", m_CharacterController.velocity.y);
            }

            // calculate which leg is behind, so as to leave that leg trailing in the jump animation
            // (This code is reliant on the specific run cycle offset in our animations,
            // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
            float runCycle =
                Mathf.Repeat(
                    m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
            float jumpLeg = (runCycle < k_Half ? 1 : -1) * m_ForwardAmount;
            if (m_IsGrounded)
            {
                m_Animator.SetFloat("JumpLeg", jumpLeg);
            }

            // the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
            // which affects the movement speed because of the root motion.
            if (m_IsGrounded && move.magnitude > 0)
            {
                m_Animator.speed = m_AnimSpeedMultiplier;
            }
            else
            {
                // don't use that while airborne
                m_Animator.speed = 1;
            }
        }

        void ScaleCapsuleForCrouching(bool crouch)
        {
            if (m_IsGrounded && crouch)
            {
                if (m_Crouching) return;
                m_CharacterController.height = m_CharacterController.height / 2f;
                m_CharacterController.center = m_CharacterController.center / 2f;
                m_Crouching = true;
                // камера следует за капсулой
                //m_Camera.transform.position -= new Vector3(0, m_CharacterController.height / 2f);
            }
            else
            {
                Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_CharacterController.radius * k_Half, Vector3.up);
                float crouchRayLength = m_CapsuleHeight - m_CharacterController.radius * k_Half;
                if (Physics.SphereCast(crouchRay, m_CharacterController.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                {
                    m_Crouching = true;
                    return;
                }
                m_CharacterController.height = m_CapsuleHeight;
                m_CharacterController.center = m_CapsuleCenter;
                m_Crouching = false;
                // камера следует за капсулой
                //m_Camera.transform.position += new Vector3(0, m_CharacterController.height / 2f);
            }

            m_Camera.transform.position = new Vector3(m_Camera.transform.position.x, m_CharacterController.transform.position.y + m_CharacterController.height, m_Camera.transform.position.z);
        }

        void PreventStandingInLowHeadroom()
        {
            // prevent standing up in crouch-only zones
            if (!m_Crouching)
            {
                Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_CharacterController.radius * k_Half, Vector3.up);
                float crouchRayLength = m_CapsuleHeight - m_CharacterController.radius * k_Half;
                if (Physics.SphereCast(crouchRay, m_CharacterController.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                {
                    m_Crouching = true;
                }
            }
        }


        private void PlayLandingSound()
        {
            m_AudioSource.clip = m_LandSound;
            m_AudioSource.Play();
            m_NextStep = m_StepCycle + .5f;
        }


        private void FixedUpdate()
        {
            float speed;
            GetInput(out speed);
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward*m_Input.y + transform.right*m_Input.x;

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                               m_CharacterController.height/2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            m_MoveDir.x = desiredMove.x*speed;
            m_MoveDir.z = desiredMove.z*speed;


            if (m_CharacterController.isGrounded)
            {
                m_MoveDir.y = -m_StickToGroundForce;

                if (m_Jump && !m_Crouching)
                {
                    m_MoveDir.y = m_JumpSpeed;
                    PlayJumpSound();
                    m_Jump = false;
                    m_Jumping = true;
                }
            }
            else
            {
                m_MoveDir += Physics.gravity*m_GravityMultiplier*Time.fixedDeltaTime;
            }
            m_CollisionFlags = m_CharacterController.Move(m_MoveDir*Time.fixedDeltaTime);

            ProgressStepCycle(speed);
            //UpdateCameraPosition(speed);

            m_MouseLook.UpdateCursorLock();

            // mine
            var move = m_MoveDir;
            if (move.magnitude > 1f) move.Normalize();
            move = m_CharacterController.transform.InverseTransformDirection(move);

            //CheckGroundStatus();
            //move = Vector3.ProjectOnPlane(move, m_GroundNormal);
            m_TurnAmount = Mathf.Atan2(move.x, move.z);
            m_ForwardAmount = move.z;

            bool crouch = Input.GetKey(KeyCode.C);
            ScaleCapsuleForCrouching(crouch);
            m_Crouching = crouch;
            m_IsGrounded = m_CharacterController.isGrounded; // моё
            //m_TurnAmount = Mathf.Atan2(m_MoveDir.x, m_MoveDir.z);

            PreventStandingInLowHeadroom();

            UpdateAnimator(move*Time.fixedDeltaTime);
        }


        private void PlayJumpSound()
        {
            m_AudioSource.clip = m_JumpSound;
            m_AudioSource.Play();
        }


        private void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + speed) * Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }


        private void PlayFootStepAudio()
        {
            if (!m_CharacterController.isGrounded)
            {
                return;
            }
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }


        /*private void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
            if (!m_UseHeadBob)
            {
                return;
            }
            if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
            {
                m_Camera.transform.localPosition =
                    m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                      (speed*(m_IsWalking ? 1f : m_RunstepLenghten)));
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
            }
            else
            {
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
            }
            m_Camera.transform.localPosition = newCameraPosition;
        }*/


        private void GetInput(out float speed)
        {
            // Read input
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");
            //speed = m_WalkSpeed; // TODO: убрать это
            //bool waswalking = m_IsWalking;

            //#if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running
            //m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
            //#endif
            // set the desired speed to be walking or running
            speed = /*m_IsWalking ?*/ m_WalkSpeed; // : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }

            // handle speed change to give an fov kick
            // only if the player is going to a run, is running and the fovkick is to be used
            /*if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
            {
                StopAllCoroutines();
                StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
            }*/
        }


        private void RotateView()
        {
            m_MouseLook.LookRotation (transform, m_Camera.transform);
        }


        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Pickup pickup = hit.gameObject.GetComponent<Pickup>();
            if (pickup != null)
            {
                pickup.HitByCharacterController(gameObject);
            }

            
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity*0.1f, hit.point, ForceMode.Impulse);
        }

        void CheckGroundStatus()
        {
            RaycastHit hitInfo;
#if UNITY_EDITOR
            // helper to visualise the ground check ray in the scene view
            Debug.DrawLine(m_CharacterController.transform.position + (Vector3.up * 0.1f), m_CharacterController.transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
#endif
            // 0.1f is a small offset to start the ray from inside the character
            // it is also good to note that the transform position in the sample assets is at the base of the character
            if (Physics.Raycast(m_CharacterController.transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
            {
                m_GroundNormal = hitInfo.normal;
                m_IsGrounded = true;
                m_Animator.applyRootMotion = true;
            }
            else
            {
                m_IsGrounded = false;
                m_GroundNormal = Vector3.up;
                m_Animator.applyRootMotion = false;
            }
        }

        public Transform lookPos;
        public Transform rightHandTarget;
        public Transform leftHandTarget;
        void OnAnimatorIK()
        {
            // Держим руки у оружия
            //m_Animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
            //m_Animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
            //m_Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            //m_Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);

            // Поворачиваем голову
            Quaternion headRotation = m_Animator.GetBoneTransform(HumanBodyBones.Head).localRotation;
            var cameraTurn = m_MouseLook.m_CameraTargetRot;
            Quaternion headTurn = new Quaternion(0, cameraTurn.x, 0, cameraTurn.w);
            m_Animator.SetBoneLocalRotation(HumanBodyBones.Head, headRotation * headTurn);

            // Крепим камеру между глаз
            Transform eye = m_Animator.GetBoneTransform(HumanBodyBones.RightEye);
            m_Camera.transform.localPosition = transform.InverseTransformPoint(eye.position) - new Vector3(0.03f, 0, -0.03f);
            //print(eye.position);
            // Поворачиваем руки
            //Quaternion rs = m_Animator.GetBoneTransform(HumanBodyBones.RightShoulder).localRotation;
            //Quaternion ls = m_Animator.GetBoneTransform(HumanBodyBones.LeftShoulder).localRotation;

            //float rot = head.localRotation.z;//m_MouseLook.m_CameraTargetRot.x;
            //Quaternion ql = ls * m_MouseLook.m_CameraTargetRot;
            //Quaternion qr = rs * (m_MouseLook.m_CameraTargetRot /** new Quaternion(1, 1, 1, -1)*/);

            //Quaternion headRight = head.localRotation;

            //m_Animator.SetBoneLocalRotation(HumanBodyBones.RightShoulder, qr);
            //m_Animator.SetBoneLocalRotation(HumanBodyBones.LeftShoulder, ql);
        }

    }
}
