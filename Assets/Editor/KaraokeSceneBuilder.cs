using System.Collections.Generic;
using System.IO;
using Code.System.Logic;
using Code.System.Manager;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Code.Editor
{
    public static class KaraokeSceneBuilder
    {
        const string ScenePath = "Assets/Scenes/8Exit.unity";
        const string EnvironmentRootName = "Karaoke Environment";
        const string RespawnRootName = "Player Respawn Point";
        const string AutoBuildRequestPath = "Temp/KaraokeSceneBuilder.request";

        static string AutoBuildRequestFullPath => Path.Combine(Directory.GetParent(Application.dataPath).FullName, AutoBuildRequestPath);

        const string Booth04 = "Assets/GameModule/Nimikko_Karaoke/Prefabs/Booths/KaraokeBooth_04.prefab";
        const string Booth05 = "Assets/GameModule/Nimikko_Karaoke/Prefabs/Booths/KaraokeBooth_05.prefab";
        const string CeilingLight = "Assets/GameModule/Nimikko_Karaoke/Prefabs/SM_CeilingLightSunk_01.prefab";
        const string WallLamp = "Assets/GameModule/Nimikko_Karaoke/Prefabs/SM_WallLamp_01.prefab";
        const string Floor = "Assets/GameModule/Nimikko_Karaoke/Prefabs/SM_Floor_01.prefab";
        const string KaraokeDoor = "Assets/GameModule/Nimikko_Karaoke/Prefabs/SM_KaraokeDoor_01.prefab";
        const string Monitor = "Assets/GameModule/Nimikko_Karaoke/Prefabs/SM_Monitor_01_Angled.prefab";
        const string Pad = "Assets/GameModule/Nimikko_Karaoke/Prefabs/SM_KaraokePad_01.prefab";
        const string Table = "Assets/GameModule/Nimikko_Karaoke/Prefabs/SM_KaraokeTable_01.prefab";
        const string MicStand = "Assets/GameModule/Nimikko_Karaoke/Prefabs/SM_MicrophoneStand_01.prefab";
        const string Microphone = "Assets/GameModule/Nimikko_Karaoke/Prefabs/SM_KaraokeMicrophone_01.prefab";

        [MenuItem("Tools/8Exit/Build Karaoke Scene")]
        public static void Build()
        {
            Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            GameObject environmentRoot = FindOrCreateRoot(EnvironmentRootName, scene);
            ClearChildren(environmentRoot.transform);
            BuildEnvironment(environmentRoot.transform);

            ChangeableProp changeableProp = ConfigureChangeableProp(scene);
            ConfigureSystems(changeableProp, scene);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();

            Debug.Log($"Built karaoke layout in {ScenePath}.");
        }

        [InitializeOnLoadMethod]
        static void AutoBuildWhenRequested()
        {
            string requestPath = AutoBuildRequestFullPath;
            if (!File.Exists(requestPath))
            {
                return;
            }

            EditorApplication.delayCall += () =>
            {
                if (!File.Exists(requestPath))
                {
                    return;
                }

                File.Delete(requestPath);
                Build();
            };
        }

        static void BuildEnvironment(Transform root)
        {
            CreatePrefab(Booth04, root, "Right Karaoke Booth", new Vector3(3.45f, 0f, 0f), new Vector3(0f, 180f, 0f), Vector3.one);
            CreatePrefab(Booth05, root, "Left Karaoke Booth", new Vector3(-3.45f, 0f, 0f), Vector3.zero, Vector3.one);

            CreatePrefab(Floor, root, "Walkway Floor Center", new Vector3(0f, 0.01f, 0f), Vector3.zero, new Vector3(1.6f, 1f, 3f));
            CreatePrefab(Floor, root, "Walkway Floor Entrance", new Vector3(0f, 0.01f, -3.2f), Vector3.zero, new Vector3(1.6f, 1f, 1.6f));
            CreatePrefab(Floor, root, "Walkway Floor Exit", new Vector3(0f, 0.01f, 3.2f), Vector3.zero, new Vector3(1.6f, 1f, 1.6f));

            CreatePrefab(KaraokeDoor, root, "Left Room Door", new Vector3(-1.85f, 0f, -2.15f), new Vector3(0f, 90f, 0f), Vector3.one);
            CreatePrefab(KaraokeDoor, root, "Right Room Door", new Vector3(1.85f, 0f, 2.15f), new Vector3(0f, -90f, 0f), Vector3.one);

            CreatePrefab(CeilingLight, root, "Ceiling Light Entrance", new Vector3(0f, 2.85f, -2.7f), Vector3.zero, Vector3.one);
            CreatePrefab(CeilingLight, root, "Ceiling Light Center", new Vector3(0f, 2.85f, 0f), Vector3.zero, Vector3.one);
            CreatePrefab(CeilingLight, root, "Ceiling Light Exit", new Vector3(0f, 2.85f, 2.7f), Vector3.zero, Vector3.one);

            CreatePrefab(WallLamp, root, "Left Wall Lamp Entrance", new Vector3(-1.95f, 1.55f, -3.25f), new Vector3(0f, 90f, 0f), Vector3.one);
            CreatePrefab(WallLamp, root, "Left Wall Lamp Exit", new Vector3(-1.95f, 1.55f, 3.25f), new Vector3(0f, 90f, 0f), Vector3.one);
            CreatePrefab(WallLamp, root, "Right Wall Lamp Entrance", new Vector3(1.95f, 1.55f, -3.25f), new Vector3(0f, -90f, 0f), Vector3.one);
            CreatePrefab(WallLamp, root, "Right Wall Lamp Exit", new Vector3(1.95f, 1.55f, 3.25f), new Vector3(0f, -90f, 0f), Vector3.one);

            CreatePrefab(Table, root, "Observation Table", new Vector3(-1.15f, 0f, 1.2f), new Vector3(0f, 35f, 0f), Vector3.one);
            CreatePrefab(Monitor, root, "Observation Monitor", new Vector3(-1.45f, 1.12f, 1.72f), new Vector3(0f, 205f, 0f), Vector3.one);
            CreatePrefab(Pad, root, "Observation Karaoke Pad", new Vector3(-1.08f, 0.72f, 1.02f), new Vector3(0f, 35f, 0f), Vector3.one);
        }

        static ChangeableProp ConfigureChangeableProp(Scene scene)
        {
            GameObject changeable = GameObject.Find("Changeable");
            if (changeable == null)
            {
                changeable = new GameObject("Changeable");
                SceneManager.MoveGameObjectToScene(changeable, scene);
            }

            ChangeableProp prop = changeable.GetComponent<ChangeableProp>();
            if (prop == null)
            {
                prop = changeable.AddComponent<ChangeableProp>();
            }

            changeable.transform.position = new Vector3(-1.15f, 0f, 1.2f);
            changeable.transform.rotation = Quaternion.Euler(0f, 35f, 0f);
            changeable.transform.localScale = Vector3.one;

            ClearChildren(changeable.transform);

            GameObject normal = new GameObject("Normal Microphone Stand");
            normal.transform.SetParent(changeable.transform, false);
            CreatePrefab(MicStand, normal.transform, "Stand", Vector3.zero, Vector3.zero, Vector3.one);

            GameObject changed = new GameObject("Changed Microphone Stand");
            changed.transform.SetParent(changeable.transform, false);
            CreatePrefab(MicStand, changed.transform, "Stand", Vector3.zero, Vector3.zero, Vector3.one);
            CreatePrefab(Microphone, changed.transform, "Added Handheld Microphone", new Vector3(0f, 1.15f, 0.08f), new Vector3(0f, 0f, 78f), Vector3.one);

            normal.SetActive(true);
            changed.SetActive(false);

            SerializedObject propObject = new SerializedObject(prop);
            SetString(propObject, "displayName", "Handheld microphone appears");
            SetInt(propObject, "firstRound", 1);
            SetBool(propObject, "canRepeatImmediately", true);
            SetObject(propObject, "normalObject", normal);
            SetObject(propObject, "changedObject", changed);
            SetBool(propObject, "startChanged", false);
            propObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(prop);

            return prop;
        }

        static void ConfigureSystems(ChangeableProp changeableProp, Scene scene)
        {
            AnomalyDirector director = FindSceneComponent<AnomalyDirector>();
            if (director != null && changeableProp != null)
            {
                SerializedObject directorObject = new SerializedObject(director);
                SerializedProperty anomalies = directorObject.FindProperty("anomalies");
                if (anomalies != null)
                {
                    List<Object> configured = new List<Object>();
                    for (int i = 0; i < anomalies.arraySize; i++)
                    {
                        Object existing = anomalies.GetArrayElementAtIndex(i).objectReferenceValue;
                        if (existing != null && !configured.Contains(existing))
                        {
                            configured.Add(existing);
                        }
                    }

                    if (!configured.Contains(changeableProp))
                    {
                        configured.Add(changeableProp);
                    }

                    anomalies.arraySize = configured.Count;
                    for (int i = 0; i < configured.Count; i++)
                    {
                        anomalies.GetArrayElementAtIndex(i).objectReferenceValue = configured[i];
                    }
                }

                directorObject.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(director);
            }

            GameManager gameManager = FindSceneComponent<GameManager>();
            if (gameManager != null)
            {
                GameObject respawn = FindOrCreateRoot(RespawnRootName, scene);
                respawn.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

                SerializedObject gameManagerObject = new SerializedObject(gameManager);
                SetObject(gameManagerObject, "playerRespawnPoint", respawn.transform);
                if (director != null)
                {
                    SetObject(gameManagerObject, "anomalyDirector", director);
                }

                gameManagerObject.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(gameManager);
            }

            ScenePlaytestSetup setup = FindSceneComponent<ScenePlaytestSetup>();
            if (setup != null)
            {
                SerializedObject setupObject = new SerializedObject(setup);
                SetBool(setupObject, "createGateMarkers", true);
                SetBool(setupObject, "createChangePropVisuals", false);
                setupObject.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(setup);
            }
        }

        static GameObject CreatePrefab(string path, Transform parent, string name, Vector3 localPosition, Vector3 localEuler, Vector3 localScale)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
            {
                throw new global::System.InvalidOperationException($"Missing prefab: {path}");
            }

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            instance.name = name;
            instance.transform.localPosition = localPosition;
            instance.transform.localRotation = Quaternion.Euler(localEuler);
            instance.transform.localScale = localScale;
            return instance;
        }

        static GameObject FindOrCreateRoot(string name, Scene scene)
        {
            GameObject root = GameObject.Find(name);
            if (root != null)
            {
                return root;
            }

            root = new GameObject(name);
            SceneManager.MoveGameObjectToScene(root, scene);
            return root;
        }

        static void ClearChildren(Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(parent.GetChild(i).gameObject);
            }
        }

        static T FindSceneComponent<T>() where T : Component
        {
            T[] components = Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] != null && components[i].gameObject.scene.IsValid())
                {
                    return components[i];
                }
            }

            return null;
        }

        static void SetString(SerializedObject serializedObject, string propertyName, string value)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property != null)
            {
                property.stringValue = value;
            }
        }

        static void SetInt(SerializedObject serializedObject, string propertyName, int value)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property != null)
            {
                property.intValue = value;
            }
        }

        static void SetBool(SerializedObject serializedObject, string propertyName, bool value)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property != null)
            {
                property.boolValue = value;
            }
        }

        static void SetObject(SerializedObject serializedObject, string propertyName, Object value)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property != null)
            {
                property.objectReferenceValue = value;
            }
        }
    }
}
