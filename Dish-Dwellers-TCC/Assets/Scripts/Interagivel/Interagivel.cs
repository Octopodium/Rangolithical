using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Sincronizavel))]
public class Interagivel : InteragivelBase {
    [HideInInspector] public HashSet<PontoInteragivel> pontos = new HashSet<PontoInteragivel>();
    [HideInInspector] public PontoInteragivel ultimoPonto = null;

    // [Tooltip("A não ser que esteja utilizando pelo menos um PontoInteragivel, você sempre irá querer esta opção ligada!")]
    // public bool incluirProprioColisor = true;

    Interacao _interacao;
    public Interacao interacao {
        get {
            if (_interacao == null) _interacao = GetComponent<Interacao>();
            return _interacao;
        }
    }

    InteracaoCondicional _interacaoCondicional;
    public InteracaoCondicional interacaoCondicional {
        get {
            if (_interacaoCondicional == null) _interacaoCondicional = GetComponent<InteracaoCondicional>();
            return _interacaoCondicional;
        }
    }
    
    public override void Interagir(Player jogador) {
        if (interacao != null) interacao.Interagir(jogador);
    }

    public override bool PodeInteragir(Player jogador) {
        return interacaoCondicional == null || interacaoCondicional.PodeInteragir(jogador);
    }


}
