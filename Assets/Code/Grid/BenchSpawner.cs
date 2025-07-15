using UnityEngine;
using UnityEngine.UIElements;

namespace Code.Grid
{
    public class BenchSpawner : MonoBehaviour
    {
        [SerializeField] Transform originBench;

        [SerializeField] int columns = 10;
        [SerializeField] float padding = 0.05f;

        private void Start()
        {
            BoxCollider collider = originBench.GetComponent<BoxCollider>();
            Vector3 size = Vector3.Scale(collider.size, originBench.lossyScale);

            float usableWidth = size.x;
            float slotSize = (usableWidth - padding * (columns - 1)) / columns;

            Vector3 start = originBench.position - new Vector3(usableWidth, 0, 0) * 0.5f;

            for (int i = 0; i < columns; i++)
            {
                float x = i * (slotSize + padding);
                Vector3 worldPos = start + new Vector3(x + slotSize / 2f, 0.01f, 0);

                GameObject slotObj = new GameObject($"BenchSlot {i}");
                slotObj.transform.position = worldPos;
                slotObj.transform.SetParent(originBench);

                var renderer = slotObj.AddComponent<BenchSlotRenderer>();
                renderer.Initialize(slotSize);

                var col = slotObj.AddComponent<BoxCollider>();
                col.size = new Vector3(slotSize, 0.1f, slotSize);
            }
        }
    }
}

