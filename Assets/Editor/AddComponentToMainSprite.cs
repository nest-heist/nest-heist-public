using UnityEngine;
using UnityEditor;

public class AddComponentToMainSpritePrefab : EditorWindow
{
    // 게임 오브젝트 리스트를 담는 변수
    public GameObject[] prefabs;

    // 에디터 창 열기
    [MenuItem("Tools/Add Component To MainSprite in Prefabs")]
    static void Init()
    {
        AddComponentToMainSpritePrefab window = (AddComponentToMainSpritePrefab)EditorWindow.GetWindow(typeof(AddComponentToMainSpritePrefab));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Add Component to MainSprite in Prefabs", EditorStyles.boldLabel);

        // 프리팹 배열 입력 받기
        SerializedObject serializedObject = new SerializedObject(this);
        SerializedProperty serializedProperty = serializedObject.FindProperty("prefabs");
        EditorGUILayout.PropertyField(serializedProperty, true);
        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Add Component"))
        {
            AddComponentToPrefabs();
        }
    }

    void AddComponentToPrefabs()
    {
        foreach (GameObject prefab in prefabs)
        {
            if (prefab == null) continue;

            // 프리팹 경로 가져오기
            string prefabPath = AssetDatabase.GetAssetPath(prefab);
            if (string.IsNullOrEmpty(prefabPath))
            {
                Debug.LogWarning(prefab.name + " is not a valid prefab.");
                continue;
            }

            // 프리팹 에셋을 수정 가능하도록 로드
            GameObject prefabInstance = PrefabUtility.LoadPrefabContents(prefabPath);
            if (prefabInstance != null)
            {
                // 하위에서 MainSprite 오브젝트 찾기
                Transform mainSpriteTransform = prefabInstance.transform.Find("MainSprite");
                if (mainSpriteTransform != null)
                {
                    GameObject mainSprite = mainSpriteTransform.gameObject;

                    // 원하는 컴포넌트를 추가
                    // 예: mainSprite.AddComponent<YourComponent>();

                    // 예제로 SpriteRenderer 컴포넌트를 추가
                    if (mainSprite.GetComponent<Attack>() == null)
                    {
                        mainSprite.AddComponent<Attack>();
                    }

                    Debug.Log("Component added to " + mainSprite.name + " in prefab " + prefab.name);

                    // 프리팹 변경사항 저장
                    PrefabUtility.SaveAsPrefabAsset(prefabInstance, prefabPath);
                }
                else
                {
                    Debug.LogWarning("MainSprite not found in " + prefab.name);
                }

                // 프리팹 에셋을 언로드하여 메모리 해제
                PrefabUtility.UnloadPrefabContents(prefabInstance);
            }
        }
    }
}