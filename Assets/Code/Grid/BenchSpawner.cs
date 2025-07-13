using UnityEngine;

namespace Code.Grid
{
    public class BenchSpawner : MonoBehaviour
    {
        [SerializeField] Transform originBench;

        [SerializeField] int colums = 5;
        [SerializeField] float slotSize = 1f;

        private void Start()
        {
            for (int i = 0; i < colums; i++)
            {
                Vector3 position = new Vector3(i * slotSize, 0, 0) + originBench.position;

                GameObject slot = new GameObject($"BenchSlot {i}");
                slot.transform.position = position;
                slot.transform.SetParent(transform);

                var renderer = slot.AddComponent<BenchSlotRenderer>();
                renderer.Initialize(slotSize);
            }
        }
    }
}

