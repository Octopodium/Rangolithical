using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Checkpoint)), CanEditMultipleObjects]
public class CheckpointEditor : Editor {
    #region SerializedProperties

    SerializedProperty spawnPoints;
    SerializedProperty layerMask;
    SerializedProperty playerColliders;
    SerializedProperty coresPorQuantidadeDePlayers;
    SerializedProperty renderers;
    SerializedProperty particulas;
    SerializedProperty explosao;

    #endregion

    Checkpoint checkpoint;
    bool sizeHandle = false;


    private void OnEnable() {
        checkpoint = target as Checkpoint;

        spawnPoints = serializedObject.FindProperty("spawnPoints");
        layerMask = serializedObject.FindProperty(nameof(layerMask));
        playerColliders = serializedObject.FindProperty(nameof(playerColliders));
        coresPorQuantidadeDePlayers = serializedObject.FindProperty(nameof(coresPorQuantidadeDePlayers));
        renderers = serializedObject.FindProperty(nameof(renderers));
        particulas = serializedObject.FindProperty(nameof(particulas));
        explosao = serializedObject.FindProperty(nameof(explosao));
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        sizeHandle = EditorGUILayout.Toggle("Enable Size Editing",sizeHandle);

        EditorGUI.BeginChangeCheck();

        Vector3 size = EditorGUILayout.Vector3Field("Checkpoint Size", checkpoint.col.size);

        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(checkpoint.col, "Changed collider size");
            checkpoint.col.size = size;
            checkpoint.col.center = new Vector3(0, size.y / 2, 0);
            EditorUtility.SetDirty(target);
        }

        EditorGUILayout.Space(15);

        EditorGUILayout.PropertyField(spawnPoints);
        EditorGUILayout.PropertyField(layerMask);
        EditorGUILayout.PropertyField(playerColliders);
        EditorGUILayout.PropertyField(renderers);
        EditorGUILayout.PropertyField(coresPorQuantidadeDePlayers);
        EditorGUILayout.PropertyField(particulas);
        EditorGUILayout.PropertyField(explosao);

        serializedObject.ApplyModifiedProperties();
    }

    public void OnSceneGUI() {
        Handles.color = Color.green;

        if (sizeHandle) {
            EditorGUI.BeginChangeCheck();

            Vector3 size = Handles.ScaleHandle(checkpoint.col.size, checkpoint.transform.position + new Vector3(0, checkpoint.col.size.y / 2, 0), checkpoint.transform.rotation);

            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(checkpoint.col, "Changed collider size");
                checkpoint.col.size = size;
                checkpoint.col.center = new Vector3(0, size.y / 2, 0);
                EditorUtility.SetDirty(target);
            }
        }

        foreach (var point in checkpoint.spawnPoints) {
            EditorGUI.BeginChangeCheck();

            Handles.DrawDottedLine(checkpoint.transform.position + checkpoint.col.center, point.position, 7f);
            Handles.DrawWireDisc(point.position, Vector3.up, 0.75f);
            Vector3 newPos = Handles.DoPositionHandle(point.position, point.rotation);

            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(point, "Checkpoint spawn position changed.");
                point.position = newPos;
                EditorUtility.SetDirty(point);
            }
        }
    }

}
