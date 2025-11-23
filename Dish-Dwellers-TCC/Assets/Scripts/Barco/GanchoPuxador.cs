using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GanchoPuxador : IResetavel
{
    Player angler;
    public Ganchavel ganchavel;
    public Vector3 sinkPosition = new Vector3(0, 2, 0);
    public Vector3 targetPosition;
    public GameObject meshBoia;
    Vector3 startPosition;
    public Collider meshCollider;

    bool hasGameManagerCallback = false;
    public void Start(){
        startPosition = meshBoia.transform.position;

        if (GameManager.instance.jogadores.Count == 0) {
            GameManager.instance.OnPlayersInstanciados += SetupAngler;
            hasGameManagerCallback = true;
        }
        else SetupAngler(GameManager.instance.jogadores[0], GameManager.instance.jogadores[1]);
    }

    void OnDestroy() {
        if (hasGameManagerCallback) 
            GameManager.instance.OnPlayersInstanciados -= SetupAngler;
    }

    void SetupAngler(Player p1, Player p2) {
        angler = p1.personagem == QualPersonagem.Angler ? p1 : p2;
    }

    public void HandlePuxada(){
        if (angler == null) SetupAngler(GameManager.instance.jogadores[0], GameManager.instance.jogadores[1]);
        angler?.barcoEmbarcado?.IniciarPuxada(transform.position);
    }

    public void SinkBoia(){
        ganchavel.enabled = false;
        meshCollider.enabled = false;
        targetPosition = meshBoia.transform.position - sinkPosition;
        StartCoroutine(MoveBoia(targetPosition));
    }

    public void UnsinkBoia(){
        ganchavel.enabled = true;
        meshCollider.enabled = true;
        StartCoroutine(MoveBoia(startPosition));
    }

    IEnumerator MoveBoia(Vector3 targetPos){
        Vector3 thisPosition = meshBoia.transform.position;
        float tempo = 0f;
    
        while (tempo <= 1f){
            tempo += Time.deltaTime * 2f;
            meshBoia.transform.position = Vector3.Lerp(thisPosition, targetPos, tempo);
            yield return null;
        }

        meshBoia.transform.position = targetPos;
    }

    public override void OnReset(){
        Debug.Log("ta resertando");
        UnsinkBoia();
        //meshBoia.transform.position = startPosition;
    }
}
