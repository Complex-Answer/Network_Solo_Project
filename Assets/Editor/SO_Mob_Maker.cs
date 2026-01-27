using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SO_Mob_Maker : EditorWindow
{
    private List<MonsterSO> _monsterSO = new();

    private Vector2 _scrollPos;

    [MenuItem("SO/Monster")]
    public static void ShowWindow() => GetWindow<SO_Mob_Maker>("Mob Maker");

    private void OnGUI()
    {
        GUILayout.Label("new Mob Data", EditorStyles.boldLabel);

        if (GUILayout.Button("Add New Monster",GUILayout.Height(25)))
        {
            _monsterSO.Add(CreateInstance<MonsterSO>());
        }

        EditorGUILayout.Space(10); //간격 벌리기

        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

        EditorGUILayout.BeginVertical("box");

        for (int i =0; i<_monsterSO.Count; i++)
        {
            GUILayout.Label($"{i+1}.Mob", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical("helpbox");
            GUILayout.Label($"Mob Data", EditorStyles.boldLabel);
            _monsterSO[i]._enemyName = EditorGUILayout.TextField("Name", _monsterSO[i]._enemyName);
            _monsterSO[i]._enemyModelPrefab = (GameObject)EditorGUILayout.ObjectField
                ("PreFab", 
                _monsterSO[i]._enemyModelPrefab, 
                typeof(GameObject),
                false); //강제 형변환 시킴 근데 이게 맞나?

            EditorGUILayout.Space(5);

            GUILayout.Label($"Mob Stats", EditorStyles.boldLabel);
            _monsterSO[i]._moveSpeed = EditorGUILayout.IntField("MoveSpeed", _monsterSO[i]._moveSpeed);
            _monsterSO[i]._attackDamage = EditorGUILayout.IntField("Damage", _monsterSO[i]._attackDamage);
            _monsterSO[i]._maxHp = EditorGUILayout.IntField("Hp", _monsterSO[i]._maxHp);
            _monsterSO[i]._attackRange = EditorGUILayout.FloatField("Range", _monsterSO[i]._attackRange);

            if (GUILayout.Button("Delete", GUILayout.Width(50)))
            {
                _monsterSO.RemoveAt(i);
            }

            EditorGUILayout.EndHorizontal(); //가로배치 끝

            EditorGUILayout.Space(5);
        }
        EditorGUILayout.EndVertical();

        GUILayout.EndScrollView();

        if (GUILayout.Button("Save all SOData", GUILayout.Height(30))) //저장 버튼
        {
            SaveSO();
        }
    }

    private void SaveSO()
    {
        string folderPath = "Assets/0. Prefab/SO/Monster";
        if (!System.IO.Directory.Exists(folderPath))
        {
            Debug.Log("지정된 경로의 폴더가 없습니다");
            return;
        }
        foreach (var monster in _monsterSO)
        {
            if (string.IsNullOrEmpty(monster._enemyName))
            {
                Debug.LogError("저장할 이름을 확인해주세요");
                continue;
            }
            string path = $"{folderPath}/{monster._enemyName}.asset";

            if (System.IO.File.Exists(path))
            {
                EditorUtility.DisplayDialog("저장 실패",
                $"'{monster._enemyName}'(이)라는 이름의 파일이 이미 존재합니다.\n이름을 수정해주세요.", "확인");

                Debug.LogError("같은 이름의 파일이 있습니다");
                continue;
            }


            MonsterSO newAsset = Instantiate(monster);
            AssetDatabase.CreateAsset(newAsset, path);

        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        _monsterSO.Clear();
        Debug.Log($"저장 완료! 경로: {folderPath}");
    }
}
