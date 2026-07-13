using Code.System.Logic;
using UnityEngine;

namespace Code.System.Manager
{
    [DisallowMultipleComponent]
    public class ScenePlaytestSetup : MonoBehaviour
    {
        [Header("gates")]
        [SerializeField] bool createGateMarkers = true;
        [SerializeField] Vector3 entrancePosition = new Vector3(0f, 1.5f, -4.75f);
        [SerializeField] Vector3 exitPosition = new Vector3(0f, 1.5f, 4.75f);
        [SerializeField] Vector3 gateSize = new Vector3(4f, 3f, 0.15f);

        [Header("change prop")]
        [SerializeField] bool createChangePropVisuals = true;
        [SerializeField] ChangeableProp changeableProp;
        [SerializeField] Vector3 propBasePosition = new Vector3(-1.25f, 0f, 0.5f);

        private const float GateFrameThickness = 0.15f;

        private void Awake()
        {
            if (createGateMarkers)
            {
                CreateGateFrame("Entrance Gate Visual", entrancePosition, gateSize, new Color(0.2f, 0.55f, 1f));
                CreateGateFrame("Exit Gate Visual", exitPosition, gateSize, new Color(1f, 0.35f, 0.2f));
            }

            if (createChangePropVisuals)
            {
                EnsureChangePropVisuals();
            }
        }

        private void EnsureChangePropVisuals()
        {
            if (changeableProp == null)
            {
                ChangeableProp[] props = FindObjectsByType<ChangeableProp>(FindObjectsSortMode.None);
                changeableProp = props.Length > 0 ? props[0] : null;
            }

            if (changeableProp == null || changeableProp.HasConfiguredObjects)
            {
                return;
            }

            GameObject normal = CreatePrimitive(
                "Normal Prop Visual",
                propBasePosition + new Vector3(0f, 0.4f, 0f),
                new Vector3(0.8f, 0.8f, 0.8f),
                new Color(0.8f, 0.82f, 0.78f),
                changeableProp.transform);

            GameObject changed = CreatePrimitive(
                "Changed Prop Visual",
                propBasePosition + new Vector3(0f, 0.7f, 0f),
                new Vector3(0.8f, 1.4f, 0.8f),
                new Color(1f, 0.78f, 0.2f),
                changeableProp.transform);

            changeableProp.Configure(normal, changed);
        }

        private void CreateGateFrame(string rootName, Vector3 center, Vector3 size, Color color)
        {
            if (transform.Find(rootName) != null)
            {
                return;
            }

            GameObject root = new GameObject(rootName);
            root.transform.SetParent(transform, true);
            root.transform.position = center;

            float halfWidth = size.x * 0.5f;
            float halfHeight = size.y * 0.5f;

            CreatePrimitive(
                $"{rootName} Left",
                center + new Vector3(-halfWidth, 0f, 0f),
                new Vector3(GateFrameThickness, size.y, size.z),
                color,
                root.transform);

            CreatePrimitive(
                $"{rootName} Right",
                center + new Vector3(halfWidth, 0f, 0f),
                new Vector3(GateFrameThickness, size.y, size.z),
                color,
                root.transform);

            CreatePrimitive(
                $"{rootName} Top",
                center + new Vector3(0f, halfHeight, 0f),
                new Vector3(size.x + GateFrameThickness, GateFrameThickness, size.z),
                color,
                root.transform);
        }

        private static GameObject CreatePrimitive(string name, Vector3 position, Vector3 scale, Color color, Transform parent)
        {
            GameObject instance = GameObject.CreatePrimitive(PrimitiveType.Cube);
            instance.name = name;
            instance.transform.SetParent(parent, true);
            instance.transform.position = position;
            instance.transform.localScale = scale;

            Collider collider = instance.GetComponent<Collider>();
            if (collider != null)
            {
                Destroy(collider);
            }

            Renderer renderer = instance.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material material = new Material(renderer.sharedMaterial)
                {
                    name = $"{name} Material",
                    color = color
                };

                renderer.material = material;
            }

            return instance;
        }
    }
}
