using System.Collections.Generic;
using UnityEngine;

public class SplitFollowTarget : MonoBehaviour {
    [SerializeField] private List<Transform> targetGroup = new List<Transform>(2);

    [Tooltip(@" 
                Pesos de valor 0 representam o comportamento 'padrão' naquele eixo, onde não existe uma preferência por movimento em nenhum sentido,
                valores maiores do que 0 indicam que a camera prefere estar no sentido positiva daquele eixo,
                valores menores do que 0 indicam que a camera prefere estar no sentido negativo daquele eixo.
            ")]
    [SerializeField] private float pesoX = 0.0f, pesoY = 0.25f, pesoZ = -0.25f;


    private void Start() {
        SetupJogadores();
    }

    void SetupJogadores() {
        targetGroup.Clear();
        // esse é o jeito otimizado de atribuir um grupo, nesse caso especifico (eu acho né).
        foreach (var jogador in GameManager.instance.jogadores) {
            targetGroup.Add(jogador.transform);
        }
    }

    private void LateUpdate() {
        transform.position = CalcularPosMedia();
    }

    /// <summary>
    /// Retorna um Vetor que representa a distancia entre os alvos 
    /// </summary>
    /// <returns></returns>
    public Vector3 CalcularDistancia() {
        return targetGroup[0].position - targetGroup[1].position;
    }

    private Vector3 CalcularPosMedia() {
        Vector3 fPos;
        Vector3 dist = Vector3.zero;
        Transform p1, p2;
        bool redefinido = false;

        if (targetGroup.Count < 2 || targetGroup[0] == null || targetGroup[1] == null) {
            if (targetGroup.Count == 2) {
                if (targetGroup[0] != null) targetGroup[1] = targetGroup[0];
                else if (targetGroup[1] != null) targetGroup[0] = targetGroup[1];
                else targetGroup[0] = targetGroup[1] = transform;
            } else {
                if (targetGroup.Count == 0) {
                    targetGroup.Add(transform);
                    targetGroup.Add(transform);
                } else {
                    if (targetGroup[0] == null) targetGroup[0] = transform;
                    targetGroup.Add(targetGroup[0]);
                }
            }

            redefinido = true;
        }

        CalcularDistancia();
        p1 = targetGroup[0];
        p2 = targetGroup[1];
        
        if (redefinido)
            SetupJogadores();

        //Encontra a posição media entre os dois transforms:
        fPos.x = (p1.position.x + p2.position.x) * 0.5f;
        fPos.y = (p1.position.y + p2.position.y) * 0.5f;
        fPos.z = (p1.position.z + p2.position.z) * 0.5f;

        // Aplica o peso à posição média encontrada, alterando a posição final da camera:
        if (pesoX != 0)
            fPos.x += Mathf.Abs(dist.x) * pesoX;
        if (pesoY != 0)
            fPos.y += Mathf.Abs(dist.y) * pesoY;
        if (pesoZ != 0)
            fPos.z += Mathf.Abs(dist.z) * pesoZ;
        return fPos;
    }
}
