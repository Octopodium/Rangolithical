using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MaybeWobble : MonoBehaviour
{
    TMP_Text textMesh;
    Mesh mesh;
    Vector3[] vertices;
    
    private bool isWobbling = false;
    private float wobbleTime = 0f;

    void Start(){
        textMesh = GetComponent<TMP_Text>();
    }

    void FixedUpdate(){
        if (isWobbling){
            textMesh.ForceMeshUpdate();
            mesh = textMesh.mesh;
            vertices = mesh.vertices;

            if(textMesh.textInfo.characterCount > 0){ //consertar, a primeira letra fica brava...
                TMP_CharacterInfo c = textMesh.textInfo.characterInfo[textMesh.textInfo.characterCount - 1]; //pega o ultimo caracter digitado
                int index = c.vertexIndex;

                Vector3 offset = Wobble(wobbleTime);
                vertices[index] += offset;
                vertices[index + 1] += offset;
                vertices[index + 2] += offset;
                vertices[index + 3] += offset;

                wobbleTime += Time.deltaTime;
            }

            mesh.vertices = vertices;
            textMesh.canvasRenderer.SetMesh(mesh);
        }
    }

    public IEnumerator AnimateWobbleChar(TMP_Text oficialText){
        isWobbling = true;
        wobbleTime = 0f;
        
        float elapsedTime = 0f;

        while (elapsedTime < 2f){
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isWobbling = false;
        
        textMesh.ForceMeshUpdate();
        mesh = textMesh.mesh;
        vertices = mesh.vertices;

        if(textMesh.textInfo.characterCount > 0){
            TMP_CharacterInfo c = textMesh.textInfo.characterInfo[textMesh.textInfo.characterCount - 1];
            int index = c.vertexIndex;

            Vector3[] originalVertices = textMesh.textInfo.meshInfo[0].vertices; //vertices originais da mesh
            
            vertices[index] = originalVertices[index];
            vertices[index + 1] = originalVertices[index + 1];
            vertices[index + 2] = originalVertices[index + 2];
            vertices[index + 3] = originalVertices[index + 3];
        }

        mesh.vertices = vertices;
        textMesh.canvasRenderer.SetMesh(mesh);
    }

    Vector2 Wobble(float time){
        return new Vector2(Mathf.Sin(time * 1f), Mathf.Cos(time * 1f)) * 5f;
    }
}