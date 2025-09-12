using System;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class GrudaEmChoes : MonoBehaviour {
    Player jogador;

    Transform chao;
    Vector3 posicao;

    void Awake() {
        jogador = GetComponent<Player>();
    }

    void FixedUpdate() {
        if (chao == null) return;

        Vector3 offset = chao.position - posicao;
        posicao =  chao.position;

        if (offset.magnitude > 0) {
            jogador.Teletransportar(transform.position + offset);
        }
    }

    public void Grudar(Transform target) {
        if (target == null) return;

        chao = target;
        posicao = chao.position;
    }

    public void Desgrudar(Transform target = null) {
        if (chao != target) return;
        
        chao = null;
    }
}