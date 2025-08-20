using Mirror;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(ObjetoDeJogo))]
public class ObjetoDeJogoEditor : Editor {
    ObjetoDeJogo objeto;

    Dictionary<string, bool> foldoutExpandInfo = new Dictionary<string, bool>();

    private void OnEnable() {
        objeto = target as ObjetoDeJogo;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        EditorGUI.BeginChangeCheck();
        bool houveMudanca = false;


        if (GUILayout.Button("Atualizar componentes")) {
            objeto.AtualizarComponentes();
            houveMudanca = true;
        }

        Sincronizavel sincronizavel = objeto.GetSincronizavel(false);

        if (objeto.carregavel) {
            if (objeto.interagivel == false) 
                houveMudanca = true;

            objeto.interagivel = true;
        }

        if (objeto.sincronizarPosicao) { 
            if (objeto.instanciadoDinamicamente == false) 
                houveMudanca = true;

            objeto.instanciadoDinamicamente = true;
        }

        Collider colisorQualquer = objeto.GetComponent<Collider>();

        if (objeto.interagivel && colisorQualquer == null) {
            ShowMessageInBox("Colisores e objetos interagiveis", "Objetos interagiveis necessitam de ter um colisor para que ocorra a interação! Por colisores serem muito variados, eles não são definidos automaticamente por este componente.", false);
        }

        if (objeto.ganchavel && colisorQualquer == null) {
            ShowMessageInBox("Colisores e objetos ganchaveis", "Objetos ganchaveis necessitam de ter um colisor onde o gancho será preso! Por colisores serem muito variados, eles não são definidos automaticamente por este componente.", false);
        }


        if (!objeto.interagivel && objeto.gameObject.layer == LayerMask.NameToLayer("Interagivel")) {
            ShowMessageInBox("Layer de interagivel?", "Este objeto possui a Layer 'Interagivel' porém não está definido como um objeto interagivel... Defina outra layer para o objeto caso não tenha planos dele ser interagivel.", false);
        }


        if (objeto.instanciadoDinamicamente || objeto.sincronizarPosicao) {
            ShowMessageInBox("Objetos realmente sincronizados", "No caso de objetos realmente sincronizados, é necessário criar duas versões, uma do online e outra do offline, e instanciar o correto dependendo do modo atual. Primeiro certifique que os componentes necessários estão inclusos, então crie uma versão do prefab online, após isso, crie uma variante deste prefab e, apenas na variante, clique no botão abaixo:");
            if (GUILayout.Button("Setar como versão offline")) {
                NetworkIdentity nid = objeto.GetComponent<NetworkIdentity>();
                if (nid != null) DestroyImmediate(nid, true);

                NetworkTransformUnreliable ntu = objeto.GetComponent<NetworkTransformUnreliable>();
                if (ntu != null) DestroyImmediate(ntu, true);

                houveMudanca = true;
            }
            ShowMessageInBox("Observações", "Como a versão offline será uma variante, apenas faça alterações na versão online que elas já irão para a offline. O jogo possui alguns mecanismos para instanciar a versão correta do prefab, como o componente ControladoDeObjeto ou o método GameManager.Instanciar.", false);
        }

        if (EditorGUI.EndChangeCheck() || houveMudanca) {
            EditorUtility.SetDirty(objeto);
        }
    }

    void ShowMessageInBox(string titulo, string mensagem, bool abertoPorPadrao = true) {
        GUIStyle textoGrandeStyle = new GUIStyle(EditorStyles.label);
        textoGrandeStyle.wordWrap = true;
        textoGrandeStyle.stretchHeight = true;

        if (!foldoutExpandInfo.ContainsKey(titulo)) foldoutExpandInfo[titulo] = abertoPorPadrao;

        GUILayout.BeginVertical("box");
        foldoutExpandInfo[titulo] = EditorGUILayout.Foldout(foldoutExpandInfo[titulo], titulo);
        if (foldoutExpandInfo[titulo]) EditorGUILayout.LabelField(mensagem, textoGrandeStyle);
        GUILayout.EndVertical();
    }
}
