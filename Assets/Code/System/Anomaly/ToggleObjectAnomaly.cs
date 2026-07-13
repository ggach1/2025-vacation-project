using UnityEngine;

namespace Code.System.Anomaly
{
    public class ToggleObjectAnomaly : AnomalyBehaviour
    {
        [Header("toggle objects")]
        [SerializeField] GameObject normalObject;
        [SerializeField] GameObject changedObject;
        [SerializeField] bool startChanged;

        public bool IsChanged { get; private set; }
        public bool HasConfiguredObjects => normalObject != null || changedObject != null;

        protected virtual void Awake()
        {
            AutoAssignChildObjects();
            SetChanged(startChanged);
        }

        public void Configure(GameObject normal, GameObject changed)
        {
            normalObject = normal;
            changedObject = changed;
            SetChanged(startChanged);
        }

        public void ApplyChange()
        {
            TryApply(1);
        }

        public void ResetChange()
        {
            ResetAnomaly();
        }

        protected override bool CanApply(int roundNumber)
        {
            return HasConfiguredObjects;
        }

        protected override void OnApply(int roundNumber)
        {
            SetChanged(true);
        }

        protected override void OnReset()
        {
            SetChanged(false);
        }

        private void SetChanged(bool changed)
        {
            IsChanged = changed;

            if (normalObject != null)
            {
                normalObject.SetActive(!changed);
            }

            if (changedObject != null)
            {
                changedObject.SetActive(changed);
            }
        }

        private void AutoAssignChildObjects()
        {
            if (normalObject == null && transform.childCount > 0)
            {
                normalObject = transform.GetChild(0).gameObject;
            }

            if (changedObject == null && transform.childCount > 1)
            {
                changedObject = transform.GetChild(1).gameObject;
            }
        }
    }
}
