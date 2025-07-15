using UnityEngine;

namespace Code.Grid
{
    [RequireComponent(typeof(LineRenderer))]
    public class BenchSlotRenderer : MonoBehaviour, IRenderer
    {
        public void Initialize(float size)
        {
            var line = GetComponent<LineRenderer>();
            line.positionCount = 5;
            line.useWorldSpace = false;
            line.loop = true;
            line.widthMultiplier = 0.05f;

            Vector3[] points = new Vector3[5]
            {
                new Vector3(-size/2, 0, -size/2),
                new Vector3(-size/2, 0,  size/2),
                new Vector3( size/2, 0,  size/2),
                new Vector3( size/2, 0, -size/2),
                new Vector3(-size/2, 0, -size/2)
            };

            line.SetPositions(points);
            line.material = new Material(Shader.Find("Sprites/Default")) { color = Color.white};
        }
    }
}

