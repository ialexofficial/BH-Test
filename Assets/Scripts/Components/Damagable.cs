using System;
using System.Collections;
using Managers;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using ViewModels;

namespace Components
{
    [RequireComponent(typeof(MeshRenderer), typeof(MotionViewModel))]
    public class Damagable : NetworkBehaviour
    {
        public UnityEvent<uint> OnDamage = new UnityEvent<uint>();
        [SerializeField] private LayerMask damagableLayer;
        [SerializeField] private float damagingCooldoown = 3f;
        [SerializeField] private Color damagedColor = Color.red;
        [SerializeField] private SkinnedMeshRenderer materialMeshRenderer;

        [SyncVar] [NonSerialized] private bool _canBeDamaged = true;
        
        private MotionViewModel _motionComponent;
        private Material _material;
        private Color _defaultColor;
        
        public void Damage(uint damager)
        {
            if (!_canBeDamaged)
                return;

            _canBeDamaged = false;
            OnDamage.Invoke(damager);
            RpcChangeColor(damagedColor);

            StartCoroutine(TurnOnDamage());
        }

        public override void OnStartClient()
        {
            _motionComponent = GetComponent<MotionViewModel>();

            _material = materialMeshRenderer.material;
            _defaultColor = _material.color;
        }

        public override void OnStartServer()
        {
            FindObjectOfType<PlayerScoreManager>().AddPlayer(this);
        }

        [Server]
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            GameObject other = hit.gameObject;

            if ((damagableLayer.value & (1 << other.layer)) != 0 && _motionComponent.IsStrafing)
            {
                other.GetComponent<Damagable>().Damage(netId);
            }
        }
        
        [ClientRpc]
        private void RpcChangeColor(Color color)
        {
            _material.color = color;
        }
        
        private IEnumerator TurnOnDamage()
        {
            yield return new WaitForSeconds(damagingCooldoown);

            RpcChangeColor(_defaultColor);
            _canBeDamaged = true;
        }
    }
}