using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GanchoPuxador : MonoBehaviour
{
    public Barco barco;
    public Ganchavel ganchavel;
    public Vector3 sinkPosition = new Vector3(0, 2, 0);
    public Vector3 targetPosition;
    public GameObject meshBoia;
    Vector3 startPosition;
    public Collider meshCollider;

    public void Start(){
        startPosition = meshBoia.transform.position;
    }

    public void HandlePuxada(){
        barco.IniciarPuxada(transform.position);
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
    
        while (tempo <= 1f)
        {
            tempo += Time.deltaTime * 2f;
            meshBoia.transform.position = Vector3.Lerp(thisPosition, targetPos, tempo);
            yield return null;
        }

        meshBoia.transform.position = targetPos;
    }
}
