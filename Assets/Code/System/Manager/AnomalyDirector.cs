using System.Collections.Generic;
using Code.System.Anomaly;
using UnityEngine;
using UnityEngine.Events;

namespace Code.System.Manager
{
    [DisallowMultipleComponent]
    public class AnomalyDirector : MonoBehaviour
    {
        [Header("round chance")]
        [SerializeField] bool randomizeAnomalyEachRound = true;
        [SerializeField] bool useForcedAnomaly;
        [SerializeField] bool forcedAnomalyActive;
        [SerializeField, Range(0f, 1f)] float anomalyChance = 0.5f;
        [SerializeField] bool avoidImmediateRepeat = true;

        [Header("scene")]
        [SerializeField] AnomalyBehaviour[] anomalies;

        [Header("events")]
        public UnityEvent<AnomalyBehaviour> OnAnomalyApplied;
        public UnityEvent OnAnomalyCleared;

        readonly List<AnomalyBehaviour> _candidates = new List<AnomalyBehaviour>();
        AnomalyBehaviour _lastAnomaly;

        public bool HasActiveAnomaly { get; private set; }
        public AnomalyBehaviour ActiveAnomaly { get; private set; }

        private void Awake()
        {
            EnsureAnomalyCache();
        }

        public RoundAnomalyResult PrepareRound(int roundNumber)
        {
            EnsureAnomalyCache();
            ResetAllAnomalies();

            if (!ShouldCreateAnomaly())
            {
                OnAnomalyCleared?.Invoke();
                return RoundAnomalyResult.None(roundNumber);
            }

            AnomalyBehaviour selected = PickAnomaly(roundNumber);
            if (selected == null)
            {
                Debug.LogWarning($"Round {roundNumber} wanted an anomaly, but no valid anomaly candidate exists.");
                OnAnomalyCleared?.Invoke();
                return RoundAnomalyResult.None(roundNumber);
            }

            if (!selected.TryApply(roundNumber))
            {
                Debug.LogWarning($"{selected.name} could not be applied for round {roundNumber}.");
                OnAnomalyCleared?.Invoke();
                return RoundAnomalyResult.None(roundNumber);
            }

            ActiveAnomaly = selected;
            HasActiveAnomaly = true;
            _lastAnomaly = selected;
            OnAnomalyApplied?.Invoke(selected);

            return RoundAnomalyResult.WithAnomaly(roundNumber, selected);
        }

        public void ResetAllAnomalies()
        {
            if (anomalies != null)
            {
                foreach (AnomalyBehaviour anomaly in anomalies)
                {
                    if (anomaly != null)
                    {
                        anomaly.ResetAnomaly();
                    }
                }
            }

            ActiveAnomaly = null;
            HasActiveAnomaly = false;
        }

        private void EnsureAnomalyCache()
        {
            if (anomalies != null && anomalies.Length > 0)
            {
                return;
            }

            anomalies = FindObjectsByType<AnomalyBehaviour>(FindObjectsSortMode.None);
        }

        private bool ShouldCreateAnomaly()
        {
            if (useForcedAnomaly)
            {
                return forcedAnomalyActive;
            }

            return randomizeAnomalyEachRound && Random.value <= anomalyChance;
        }

        private AnomalyBehaviour PickAnomaly(int roundNumber)
        {
            _candidates.Clear();

            if (anomalies == null)
            {
                return null;
            }

            foreach (AnomalyBehaviour anomaly in anomalies)
            {
                if (anomaly != null && anomaly.CanAppear(roundNumber))
                {
                    _candidates.Add(anomaly);
                }
            }

            if (_candidates.Count > 1 && avoidImmediateRepeat && _lastAnomaly != null && !_lastAnomaly.CanRepeatImmediately)
            {
                _candidates.Remove(_lastAnomaly);
            }

            return _candidates.Count == 0 ? null : _candidates[Random.Range(0, _candidates.Count)];
        }
    }

    public readonly struct RoundAnomalyResult
    {
        public int RoundNumber { get; }
        public bool HasAnomaly { get; }
        public AnomalyBehaviour ActiveAnomaly { get; }
        public string ActiveAnomalyName => ActiveAnomaly != null ? ActiveAnomaly.DisplayName : string.Empty;

        private RoundAnomalyResult(int roundNumber, bool hasAnomaly, AnomalyBehaviour activeAnomaly)
        {
            RoundNumber = roundNumber;
            HasAnomaly = hasAnomaly;
            ActiveAnomaly = activeAnomaly;
        }

        public static RoundAnomalyResult None(int roundNumber)
        {
            return new RoundAnomalyResult(roundNumber, false, null);
        }

        public static RoundAnomalyResult WithAnomaly(int roundNumber, AnomalyBehaviour anomaly)
        {
            return new RoundAnomalyResult(roundNumber, anomaly != null, anomaly);
        }
    }
}
