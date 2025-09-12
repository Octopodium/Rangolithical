using System;
using UnityEngine;

public class GrudavelDois : Grudavel {
    Player jogador;
    Vector3 posicao;

    void Awake() {
        jogador = GetComponent<Player>();
    }

    void FixedUpdate() {
        if (grudavelTransform == null) return;

        Vector3 offset = grudavelTransform.position - posicao;
        posicao =  grudavelTransform.position;

        if (offset.magnitude > 0) {
            jogador.Teletransportar(transform.position + offset);
        }
    }

    public override void Grudar(Transform target, LimitacaoDoGrude limitacao = LimitacaoDoGrude.GrudaTudo, bool manterPosicao = true) {
        if (target == null) return;

        grudavelTransform = target;
        posicao = grudavelTransform.position;
    }

    public override void Desgrudar() {
        if (grudavelTransform == null) return;
        
        grudavelTransform = null;
    }
}