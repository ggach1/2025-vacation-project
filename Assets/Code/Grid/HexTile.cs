using System;
using System.Collections;
using UnityEngine;

namespace Code.Grid
{
    [RequireComponent(typeof(LineRenderer))]
    public class HexTile : MonoBehaviour
    {
        public Vector3Int CubeCoord;
        public bool IsWalkable = true;

        LineRenderer _renderer;
        Color _defaultColor = Color.white;
        Color _highlightColor = Color.green;

        private void Awake()
        {
            _renderer = GetComponent<LineRenderer>();
        }

        public void HighlightTemporary(float duration = 0.3f)
        {
            StopAllCoroutines();
            StartCoroutine(FlashHighlight(duration));
        }

        // 나중에 awate를 사용해서 바꿔보자
        private IEnumerator FlashHighlight(float duration)
        {
            _renderer.material.color = _highlightColor;
            yield return new WaitForSeconds(duration);
            _renderer.material.color = _defaultColor;
        }
    }
}

