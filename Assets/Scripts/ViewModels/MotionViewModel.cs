using System;
using System.Collections;
using Mirror;
using Models;
using UnityEngine;

namespace ViewModels
{
    [RequireComponent(typeof(CharacterController))]
    public class MotionViewModel : NetworkBehaviour
    {
        [SyncVar] [NonSerialized] public bool IsStrafing = false;
        
        [Header("Motion")]
        [SerializeField] private float horizontalRotationSpeed = 5f;
        [SerializeField] private float verticalRotationSpeed = 1f;
        [Tooltip("The first element is minimum, the second element is maximum")]
        [SerializeField] private float[] yRotationRange = new float[2] {4f, 20f};
        [SerializeField] private float motionSpeed = 5f;

        [Header("Strafe")]
        [SerializeField] private float strafeLength = 20f;
        [SerializeField] private float strafeMaxSpeed = 15f;
        [SerializeField] private float strafeCooldown = 3f;
        [SerializeField] private AnimationCurve strafeSpeedCurve;

        private MotionModel _model;
        private CharacterController _characterController;
        private Coroutine _strafeCoroutine;
        private Animator _animator;
        private CinemachineRecomposer _cameraRecomposer;
        private NetworkAnimator _networkAnimator;

        public override void OnStartClient()
        {
            if (isLocalPlayer)
            {
                InitModel();

                _cameraRecomposer = FindObjectOfType<CinemachineRecomposer>();
                _cameraRecomposer.VirtualCamera.Follow = transform;
            }

            _characterController = GetComponent<CharacterController>();
            _animator = GetComponentInChildren<Animator>();
            _networkAnimator = GetComponent<NetworkAnimator>();
        }

        public override void OnStopLocalPlayer()
        {
            ClearModel();
        }

        private void InitModel()
        {
            _model = new MotionModel();

            _model.OnStrafingChange += CmdStrafingStateChanged;
            _model.OnRotate += OnRotated;
            _model.OnMove += OnMoved;
            _model.OnStrafeStart += OnStrafeStarted;
            _model.OnStrafeEnd += OnStrafeEnded;
        }

        private void ClearModel()
        {
            _model.OnStrafingChange -= CmdStrafingStateChanged;
            _model.OnRotate -= OnRotated;
            _model.OnMove -= OnMoved;
            _model.OnStrafeStart -= OnStrafeStarted;
            _model.OnStrafeEnd -= OnStrafeEnded;
        }
        
        private void Update()
        {
            if (!isLocalPlayer)
                return;

            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            if (!GameManager.CanInput)
                mouseX = mouseY = horizontal = vertical = 0f;

            _model.Rotate(horizontalRotationSpeed, verticalRotationSpeed, mouseX, mouseY, _cameraRecomposer.m_Tilt, yRotationRange);
            _model.Move(vertical, horizontal, motionSpeed);

            if (Input.GetMouseButtonDown(0) && GameManager.CanInput)
            {
                _model.StartStrafe(strafeCooldown);
            }
        }
        
        private void OnMoved(Vector3 velocity)
        {
            Vector3 worldVelocity = transform.TransformDirection(velocity);
            
            _characterController.Move(worldVelocity);
            CmdMove(worldVelocity);

            _animator.SetFloat("MovementX", velocity.x);
            _animator.SetFloat("MovementZ", velocity.z);
        }
        
        private void OnRotated(Vector3 characterRotation, float cameraTilt)
        {
            transform.Rotate(characterRotation);
            CmdRotate(characterRotation);

            _cameraRecomposer.m_Tilt = cameraTilt;
        }

        [Command]
        private void CmdStrafingStateChanged(bool isStrafing)
        {
            IsStrafing = isStrafing;
        }
        
        private void OnStrafeStarted()
        {
            _strafeCoroutine = StartCoroutine(StrafeCoroutine());
            
            // _animator.SetTrigger("Jump");
            _networkAnimator.SetTrigger("Jump");
        }

        private void OnStrafeEnded()
        {
            StopCoroutine(_strafeCoroutine);
        }

        private IEnumerator StrafeCoroutine()
        {
            while (true)
            {
                Vector3 velocity = _model.LerpStrafe(strafeLength, strafeMaxSpeed, strafeSpeedCurve, transform.forward);

                _characterController.Move(velocity);
                CmdMove(velocity);

                yield return new WaitForFixedUpdate();
            }
        }

        [Command]
        private void CmdMove(Vector3 velocity)
        {
            if (isLocalPlayer)
                return;
            
            _characterController.Move(velocity);
        }

        [Command]
        private void CmdRotate(Vector3 rotation)
        {
            if (isLocalPlayer)
                return;
            
            transform.Rotate(rotation);
        }
    }
}