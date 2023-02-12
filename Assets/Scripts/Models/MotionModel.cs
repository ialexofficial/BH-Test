using System;
using Mirror;
using UnityEngine;

namespace Models
{
    public class MotionModel
    {
        public event Action<bool> OnStrafingChange;
        public event Action<Vector3> OnMove;
        public event Action<Vector3, float> OnRotate;
        public event Action OnStrafeStart;
        public event Action OnStrafeEnd;

        private bool _isStrafing = false;
        private float _strafedLength = 0f;
        private double _lastStrafeTime = double.NegativeInfinity;

        public bool IsStrafing
        {
            get => _isStrafing;
            set
            {
                _isStrafing = value;
                OnStrafingChange?.Invoke(_isStrafing);
            }
        }
        
        public void Move(
            float verticalAxis, 
            float horizontalAxis, 
            float motionSpeed
        )
        {
            if (IsStrafing)
                return;

            Vector3 velocity = new Vector3(horizontalAxis, 0, verticalAxis).normalized * motionSpeed;
            velocity.y = -7f;

            OnMove?.Invoke(velocity * Time.deltaTime);
        }

        public void Rotate(
            float horizontalRotationSpeed,
            float verticalRotationSpeed,
            float mouseX, 
            float mouseY,
            float currentTilt,
            float[] tiltRange
        )
        {
            Vector3 characterRotation = new Vector3(0, mouseX, 0) *
                                        (horizontalRotationSpeed * Time.deltaTime);

            float cameraTilt = currentTilt - mouseY * verticalRotationSpeed * Time.deltaTime;

            if (cameraTilt < tiltRange[0])
                cameraTilt = tiltRange[0];

            if (cameraTilt > tiltRange[1])
                cameraTilt = tiltRange[1];
            
            OnRotate?.Invoke(characterRotation, cameraTilt);
        }

        public void StartStrafe(float strafeCooldown)
        {
            if (IsStrafing || NetworkTime.time < _lastStrafeTime + strafeCooldown)
                return;

            _lastStrafeTime = NetworkTime.time;
            IsStrafing = true;
            _strafedLength = 0f;
            OnStrafeStart?.Invoke();
        }

        public Vector3 LerpStrafe(
            float strafeLength,
            float strafeMaxSpeed,
            AnimationCurve strafeSpeedCurve,
            Vector3 forward
        )
        {
            if (!IsStrafing)
                return Vector3.zero;

            Vector3 velocity = forward;

            velocity.y = 0;
            velocity *= strafeSpeedCurve.Evaluate(_strafedLength / strafeLength) *
                           strafeMaxSpeed *
                           Time.deltaTime;
            _strafedLength += velocity.magnitude;

            if (_strafedLength >= strafeLength)
            {
                IsStrafing = false;   
                OnStrafeEnd?.Invoke();
            }

            return velocity;
        }
    }
}