using Code.Bocchi;
using Code.System.Manager;
using UnityEngine;

namespace Code.System.Logic
{
    [RequireComponent(typeof(Collider))]
    public class GateTrigger : MonoBehaviour
    {
        [SerializeField] GateKind gateKind;
        [SerializeField] bool configureAsTrigger = true;

        Collider _collider;

        private void Reset()
        {
            _collider = GetComponent<Collider>();
            if (_collider != null)
            {
                _collider.isTrigger = true;
            }
        }

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            if (configureAsTrigger && _collider != null)
            {
                _collider.isTrigger = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Player player = other.GetComponentInParent<Player>();
            if (player == null || GameManager.Instance == null)
            {
                return;
            }

            GameManager.Instance.HandleGateEntered(gateKind, player);
        }
    }
}
