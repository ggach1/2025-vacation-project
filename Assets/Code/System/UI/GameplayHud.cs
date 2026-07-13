using Code.System.Logic;
using Code.System.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Code.System.UI
{
    [DisallowMultipleComponent]
    public class GameplayHud : MonoBehaviour
    {
        [Header("scene")]
        [SerializeField] GameManager gameManager;
        [SerializeField] bool showDebugAnomalyState;

        GameObject _root;
        GameObject _resultPanel;
        GameObject _debugPanel;
        Image _resultBackground;
        Text _progressText;
        Text _stateText;
        Text _attemptText;
        Text _resultTitleText;
        Text _resultDetailText;
        Text _debugText;
        Font _font;

        private void Awake()
        {
            EnsureGameManager();
            BuildHud();
            Subscribe();
            RefreshFromGameManager();
        }

        private void OnDestroy()
        {
            if (gameManager == null)
            {
                return;
            }

            gameManager.OnRoundStarted.RemoveListener(HandleRoundStarted);
            gameManager.OnGateJudged.RemoveListener(HandleGateJudged);
            gameManager.OnStateChanged.RemoveListener(HandleStateChanged);
            gameManager.OnProgressChanged.RemoveListener(HandleProgressChanged);
        }

        private void EnsureGameManager()
        {
            if (gameManager != null)
            {
                return;
            }

            gameManager = GameManager.Instance != null
                ? GameManager.Instance
                : FindFirstObjectByType<GameManager>();
        }

        private void Subscribe()
        {
            if (gameManager == null)
            {
                return;
            }

            gameManager.OnRoundStarted.AddListener(HandleRoundStarted);
            gameManager.OnGateJudged.AddListener(HandleGateJudged);
            gameManager.OnStateChanged.AddListener(HandleStateChanged);
            gameManager.OnProgressChanged.AddListener(HandleProgressChanged);
        }

        private void RefreshFromGameManager()
        {
            if (gameManager == null)
            {
                SetProgress(0, 8);
                SetAttempt(0);
                SetStateText("WAITING", new Color(0.78f, 0.82f, 0.88f));
                SetResultVisible(false);
                return;
            }

            SetProgress(gameManager.CurrentExitNumber, gameManager.TargetExitNumber);
            SetAttempt(gameManager.CurrentRound);
            SetStateFromGameState(gameManager.State);
            SetDebugText(gameManager.AnomalyActive);
            SetResultVisible(false);
        }

        private void HandleRoundStarted(int roundNumber, bool anomalyActive)
        {
            SetAttempt(roundNumber);
            SetStateText("OBSERVE", Color.white);
            SetDebugText(anomalyActive);
            SetResultVisible(false);
        }

        private void HandleProgressChanged(int currentExitNumber, int targetExitNumber)
        {
            SetProgress(currentExitNumber, targetExitNumber);
        }

        private void HandleGateJudged(GateKind gateKind, bool success)
        {
            string choice = gateKind == GateKind.Exit ? "EXIT" : "ENTRANCE";
            _resultTitleText.text = success ? "CORRECT" : "WRONG";
            _resultDetailText.text = $"{choice} selected";
            _resultBackground.color = success
                ? new Color(0.08f, 0.36f, 0.2f, 0.88f)
                : new Color(0.42f, 0.1f, 0.1f, 0.88f);

            SetResultVisible(true);
        }

        private void HandleStateChanged(GameState state)
        {
            SetStateFromGameState(state);
        }

        private void SetStateFromGameState(GameState state)
        {
            switch (state)
            {
                case GameState.Playing:
                    SetStateText("OBSERVE", Color.white);
                    break;
                case GameState.Judging:
                    SetStateText("JUDGING", new Color(1f, 0.85f, 0.36f));
                    break;
                case GameState.Success:
                    SetStateText("CORRECT", new Color(0.55f, 1f, 0.68f));
                    break;
                case GameState.Failed:
                    SetStateText("WRONG", new Color(1f, 0.55f, 0.55f));
                    break;
                case GameState.GameOver:
                    SetStateText("CLEAR", new Color(0.55f, 0.85f, 1f));
                    _resultTitleText.text = "CLEAR";
                    _resultDetailText.text = "Exit reached";
                    _resultBackground.color = new Color(0.08f, 0.2f, 0.42f, 0.9f);
                    SetResultVisible(true);
                    break;
                default:
                    SetStateText("WAITING", new Color(0.78f, 0.82f, 0.88f));
                    break;
            }
        }

        private void BuildHud()
        {
            _font = CreateHudFont();

            _root = new GameObject("Gameplay HUD", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            _root.transform.SetParent(transform, false);

            Canvas canvas = _root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            CanvasScaler scaler = _root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            GameObject statusPanel = CreatePanel(
                "Status Panel",
                _root.transform,
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(24f, -24f),
                new Vector2(390f, 166f),
                new Color(0.05f, 0.06f, 0.07f, 0.78f));

            _progressText = CreateText(
                "Progress Text",
                statusPanel.transform,
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(18f, -14f),
                new Vector2(350f, 34f),
                "EXIT 0 / 8",
                26,
                FontStyle.Bold,
                Color.white,
                TextAnchor.MiddleLeft);

            _stateText = CreateText(
                "State Text",
                statusPanel.transform,
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(18f, -50f),
                new Vector2(350f, 24f),
                "WAITING",
                18,
                FontStyle.Normal,
                new Color(0.78f, 0.82f, 0.88f),
                TextAnchor.MiddleLeft);

            _attemptText = CreateText(
                "Attempt Text",
                statusPanel.transform,
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(18f, -76f),
                new Vector2(350f, 22f),
                "ATTEMPT --",
                15,
                FontStyle.Normal,
                new Color(0.68f, 0.72f, 0.78f),
                TextAnchor.MiddleLeft);

            CreateText(
                "Rule Text",
                statusPanel.transform,
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(18f, -106f),
                new Vector2(350f, 48f),
                "No anomaly: ENTRANCE\nAnomaly found: EXIT",
                15,
                FontStyle.Normal,
                new Color(0.76f, 0.8f, 0.86f),
                TextAnchor.UpperLeft);

            _resultPanel = CreatePanel(
                "Result Panel",
                _root.transform,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, -130f),
                new Vector2(320f, 92f),
                new Color(0.08f, 0.36f, 0.2f, 0.88f));
            _resultBackground = _resultPanel.GetComponent<Image>();

            _resultTitleText = CreateText(
                "Result Title",
                _resultPanel.transform,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -14f),
                new Vector2(280f, 38f),
                "CORRECT",
                28,
                FontStyle.Bold,
                Color.white,
                TextAnchor.MiddleCenter);

            _resultDetailText = CreateText(
                "Result Detail",
                _resultPanel.transform,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -54f),
                new Vector2(280f, 24f),
                "EXIT selected",
                15,
                FontStyle.Normal,
                new Color(0.88f, 0.92f, 0.96f),
                TextAnchor.MiddleCenter);

            _debugPanel = CreatePanel(
                "Debug Panel",
                _root.transform,
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-24f, -24f),
                new Vector2(270f, 36f),
                new Color(0.05f, 0.06f, 0.07f, 0.68f));

            _debugText = CreateText(
                "Debug Text",
                _debugPanel.transform,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(244f, 24f),
                "DEBUG ANOMALY: OFF",
                14,
                FontStyle.Bold,
                new Color(1f, 0.85f, 0.36f),
                TextAnchor.MiddleCenter);

            _debugPanel.SetActive(showDebugAnomalyState);
            SetResultVisible(false);
        }

        private static GameObject CreatePanel(
            string name,
            Transform parent,
            Vector2 anchor,
            Vector2 pivot,
            Vector2 anchoredPosition,
            Vector2 size,
            Color color)
        {
            GameObject panel = new GameObject(name, typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);

            RectTransform rectTransform = panel.GetComponent<RectTransform>();
            rectTransform.anchorMin = anchor;
            rectTransform.anchorMax = anchor;
            rectTransform.pivot = pivot;
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = size;

            Image image = panel.GetComponent<Image>();
            image.color = color;

            return panel;
        }

        private Text CreateText(
            string name,
            Transform parent,
            Vector2 anchor,
            Vector2 pivot,
            Vector2 anchoredPosition,
            Vector2 size,
            string text,
            int fontSize,
            FontStyle fontStyle,
            Color color,
            TextAnchor alignment)
        {
            GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(parent, false);

            RectTransform rectTransform = textObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = anchor;
            rectTransform.anchorMax = anchor;
            rectTransform.pivot = pivot;
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = size;

            Text label = textObject.GetComponent<Text>();
            label.text = text;
            label.font = _font;
            label.fontSize = fontSize;
            label.fontStyle = fontStyle;
            label.color = color;
            label.alignment = alignment;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Truncate;

            return label;
        }

        private static Font CreateHudFont()
        {
            Font font = Font.CreateDynamicFontFromOSFont(
                new[] { "Malgun Gothic", "Segoe UI", "Arial" },
                18);

            return font != null ? font : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }

        private void SetProgress(int currentExitNumber, int targetExitNumber)
        {
            _progressText.text = $"EXIT {currentExitNumber} / {targetExitNumber}";
        }

        private void SetAttempt(int roundNumber)
        {
            _attemptText.text = roundNumber > 0 ? $"ATTEMPT {roundNumber:00}" : "ATTEMPT --";
        }

        private void SetStateText(string text, Color color)
        {
            _stateText.text = text;
            _stateText.color = color;
        }

        private void SetDebugText(bool anomalyActive)
        {
            if (_debugText == null)
            {
                return;
            }

            _debugText.text = anomalyActive ? "DEBUG ANOMALY: ON" : "DEBUG ANOMALY: OFF";
        }

        private void SetResultVisible(bool visible)
        {
            if (_resultPanel != null)
            {
                _resultPanel.SetActive(visible);
            }
        }
    }
}
