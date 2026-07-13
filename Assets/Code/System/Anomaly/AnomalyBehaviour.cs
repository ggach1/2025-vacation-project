using UnityEngine;

namespace Code.System.Anomaly
{
    public abstract class AnomalyBehaviour : MonoBehaviour
    {
        [Header("anomaly")]
        [SerializeField] string displayName;
        [SerializeField, Min(1)] int firstRound = 1;
        [SerializeField] bool canRepeatImmediately = true;

        public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;
        public int FirstRound => firstRound;
        public bool CanRepeatImmediately => canRepeatImmediately;
        public bool IsApplied { get; private set; }

        public bool CanAppear(int roundNumber)
        {
            return isActiveAndEnabled && roundNumber >= firstRound && CanApply(roundNumber);
        }

        public bool TryApply(int roundNumber)
        {
            if (!CanAppear(roundNumber))
            {
                return false;
            }

            OnApply(roundNumber);
            IsApplied = true;
            return true;
        }

        public void ResetAnomaly()
        {
            OnReset();
            IsApplied = false;
        }

        protected virtual bool CanApply(int roundNumber)
        {
            return true;
        }

        protected abstract void OnApply(int roundNumber);
        protected abstract void OnReset();
    }
}
