#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(Empurravel))]
public class EmpurravelEditor : Editor {
    Empurravel empurravel;

    Dictionary<string, bool> foldoutExpandInfo = new Dictionary<string, bool>();

    private void OnEnable() {
        empurravel = target as Empurravel;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (empurravel.transform.localScale != Vector3.one){
            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Objetos empurraveis não devem ser escalados!");
            GUILayout.EndVertical();

            if (GUILayout.Button("Ajusta escala automaticamente")) {
                AjustarEscala();
            }
        }
    }

    public Vector3 MultVetores(Vector3 a, Vector3 b) {
        a.x *= b.x;
        a.y *= b.y;
        a.z *= b.z;
        return a;
    }

    public void AjustarEscala() {
        Vector3 escala = empurravel.transform.localScale;
        empurravel.transform.localScale = Vector3.one;

        BoxCollider col = empurravel.GetComponent<BoxCollider>();
        col.size = MultVetores(col.size, escala);
        EditorUtility.SetDirty(col);

        foreach (Transform child in empurravel.transform) {
            child.localScale = MultVetores(child.localScale, escala);
            EditorUtility.SetDirty(child);
        }

        EditorUtility.SetDirty(empurravel);
    }
}

#endif