
using UnityEngine;
using UnityEngine.Events;

namespace Code.Entities
{
    public class EntityAnimator : MonoBehaviour
    {
        [SerializeField] Animator animator;
        public UnityEvent<Vector3, Quaternion> OnAnimationMoveEvt;

        Entity _entity;

        public bool ApplyRootMotion
        {
            get => animator.applyRootMotion;
            set => animator.applyRootMotion = value;
        }

        public void Initialize(Entity entity)
        {
            _entity = entity;
        }

        public void SetParam(int hash, float value, float dampTime) => animator.SetFloat(hash, value, dampTime, Time.deltaTime);

        public void SetParam(int hash, float value) => animator.SetFloat(hash, value);
        public void SetParam(int hash, int value) => animator.SetInteger(hash, value);
        public void SetParam(int hash, bool value) => animator.SetBool(hash, value);
        public void SetParam(int hash) => animator.SetTrigger(hash);

        public void OffAnimator()
        {
            animator.enabled = false;
        }

        private void OnAnimatorMove()
        {
            OnAnimationMoveEvt?.Invoke(animator.deltaPosition, animator.deltaRotation);
        }
    }
}