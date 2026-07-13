using System.Collections;
using Code.Bocchi;
using Code.System.Logic;
using UnityEngine;
using UnityEngine.Events;

namespace Code.System.Manager
{
    public class GameManager : MonoBehaviour
    {
        [Header("round")]
        [SerializeField] bool startOnAwake = true;
        [SerializeField] bool autoAdvanceAfterJudgement = true;
        [SerializeField] float roundAdvanceDelay = 1f;

        [Header("progress")]
        [SerializeField] int startExitNumber;
        [SerializeField] int targetExitNumber = 8;

        [Header("scene")]
        [SerializeField] Player player;
        [SerializeField] Transform playerRespawnPoint;
        [SerializeField] bool resetPlayerAfterGate = true;
        [SerializeField] AnomalyDirector anomalyDirector;

        [Header("events")]
        public UnityEvent<GameState> OnStateChanged;
        public UnityEvent<int, bool> OnRoundStarted;
        public UnityEvent<GateKind, bool> OnGateJudged;
        public UnityEvent<int, int> OnProgressChanged;

        public static GameManager Instance { get; private set; }
        public GameState State { get; private set; } = GameState.Boot;
        public int CurrentRound { get; private set; }
        public int CurrentExitNumber { get; private set; }
        public int TargetExitNumber => targetExitNumber;
        public bool AnomalyActive { get; private set; }

        Coroutine _advanceRoutine;
        Vector3 _initialPlayerPosition;
        Quaternion _initialPlayerRotation;
        bool _hasInitialPlayerPose;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            EnsureAnomalyDirector();
            CacheInitialPlayerPose();
        }

        private void Start()
        {
            if (startOnAwake)
            {
                BeginGame();
            }
        }

        public void BeginGame()
        {
            CurrentRound = 0;
            CurrentExitNumber = startExitNumber;
            OnProgressChanged?.Invoke(CurrentExitNumber, targetExitNumber);
            BeginNextRound();
        }

        public void BeginNextRound()
        {
            StopAdvanceRoutine();

            if (State == GameState.GameOver)
            {
                return;
            }

            CurrentRound++;
            RoundAnomalyResult anomalyResult = anomalyDirector != null
                ? anomalyDirector.PrepareRound(CurrentRound)
                : RoundAnomalyResult.None(CurrentRound);
            AnomalyActive = anomalyResult.HasAnomaly;

            SetState(GameState.Playing);
            OnProgressChanged?.Invoke(CurrentExitNumber, targetExitNumber);
            OnRoundStarted?.Invoke(CurrentRound, AnomalyActive);

            Debug.Log($"Round {CurrentRound} started. Exit number: {CurrentExitNumber}/{targetExitNumber}. Anomaly active: {AnomalyActive}. Active anomaly: {anomalyResult.ActiveAnomalyName}");
        }

        public void HandleGateEntered(GateKind gateKind, Player player)
        {
            if (State != GameState.Playing)
            {
                return;
            }

            SetState(GameState.Judging);

            bool playerGuessedChanged = gateKind == GateKind.Exit;
            bool success = playerGuessedChanged == AnomalyActive;
            bool cleared = ApplyProgressResult(success);

            OnGateJudged?.Invoke(gateKind, success);
            SetState(cleared ? GameState.GameOver : success ? GameState.Success : GameState.Failed);

            Debug.Log($"{gateKind} selected by {player.name}. Success: {success}. Exit number: {CurrentExitNumber}/{targetExitNumber}");

            if (resetPlayerAfterGate)
            {
                ResetPlayerToStart(player);
            }

            if (autoAdvanceAfterJudgement && !cleared)
            {
                _advanceRoutine = StartCoroutine(AdvanceAfterDelay());
            }
        }

        private IEnumerator AdvanceAfterDelay()
        {
            yield return new WaitForSeconds(roundAdvanceDelay);
            BeginNextRound();
        }

        private void EnsureAnomalyDirector()
        {
            if (anomalyDirector != null)
            {
                return;
            }

            anomalyDirector = GetComponent<AnomalyDirector>();
            if (anomalyDirector != null)
            {
                return;
            }

            AnomalyDirector[] directors = FindObjectsByType<AnomalyDirector>(FindObjectsSortMode.None);
            if (directors.Length > 0)
            {
                anomalyDirector = directors[0];
                return;
            }

            anomalyDirector = gameObject.AddComponent<AnomalyDirector>();
        }

        private void CacheInitialPlayerPose()
        {
            if (player == null)
            {
                player = FindFirstObjectByType<Player>();
            }

            if (player == null)
            {
                return;
            }

            _initialPlayerPosition = player.transform.position;
            _initialPlayerRotation = player.transform.rotation;
            _hasInitialPlayerPose = true;
        }

        private void ResetPlayerToStart(Player targetPlayer)
        {
            Player playerToReset = targetPlayer != null ? targetPlayer : player;
            if (playerToReset == null)
            {
                return;
            }

            if (!_hasInitialPlayerPose)
            {
                CacheInitialPlayerPose();
            }

            Vector3 targetPosition = playerRespawnPoint != null ? playerRespawnPoint.position : _initialPlayerPosition;
            Quaternion targetRotation = playerRespawnPoint != null ? playerRespawnPoint.rotation : _initialPlayerRotation;

            CharacterController controller = playerToReset.GetComponent<CharacterController>();
            bool wasControllerEnabled = controller != null && controller.enabled;
            if (wasControllerEnabled)
            {
                controller.enabled = false;
            }

            playerToReset.transform.SetPositionAndRotation(targetPosition, targetRotation);

            if (wasControllerEnabled)
            {
                controller.enabled = true;
            }

            CharacterMovement movement = playerToReset.GetComponentInChildren<CharacterMovement>();
            if (movement != null)
            {
                movement.StopImmediately();
            }

            CameraView cameraView = playerToReset.GetComponent<CameraView>();
            if (cameraView != null)
            {
                cameraView.ResetView();
            }
        }

        private bool ApplyProgressResult(bool success)
        {
            if (success)
            {
                CurrentExitNumber = Mathf.Min(CurrentExitNumber + 1, targetExitNumber);
            }
            else
            {
                CurrentExitNumber = startExitNumber;
            }

            OnProgressChanged?.Invoke(CurrentExitNumber, targetExitNumber);
            return success && CurrentExitNumber >= targetExitNumber;
        }

        private void StopAdvanceRoutine()
        {
            if (_advanceRoutine == null)
            {
                return;
            }

            StopCoroutine(_advanceRoutine);
            _advanceRoutine = null;
        }

        private void SetState(GameState nextState)
        {
            if (State == nextState)
            {
                return;
            }

            State = nextState;
            OnStateChanged?.Invoke(State);
        }
    }

    public enum GameState
    {
        Boot,
        Playing,
        Judging,
        Success,
        Failed,
        GameOver
    }
}
