using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Sincronizavel))]
public class Interagivel : InteragivelBase {
    [HideInInspector] public HashSet<PontoInteragivel> pontos = new HashSet<PontoInteragivel>();
    [HideInInspector] public PontoInteragivel ultimoPonto = null;

    public bool podeInteragirDiretamente = true;

    Sincronizavel _sincronizavel;
    [HideInInspector] public Sincronizavel sincronizavel {
        get {
            if (_sincronizavel == null) _sincronizavel = GetComponent<Sincronizavel>();
            return _sincronizavel;
        }
    }

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

    protected override void Start() {
        base.Start();

        if (!podeInteragirDiretamente) enabled = false;
    }
    
    public override void Interagir(Player jogador) {
        if (interacao == null || !PodeInteragir(jogador) || NaoPodeInteragirPois(jogador) != MotivoNaoInteracao.Nenhum) return;
        if (interacao != null) interacao.Interagir(jogador);
    }

    public override bool PodeInteragir(Player jogador) {
        return interacaoCondicional == null || interacaoCondicional.PodeInteragir(jogador);
    }

    public override MotivoNaoInteracao NaoPodeInteragirPois(Player jogador) {
        if (interacaoCondicional == null) return MotivoNaoInteracao.Nenhum;
        return interacaoCondicional.NaoPodeInteragirPois(jogador);
    }

    public void AddPonto(PontoInteragivel ponto) {
        pontos.Add(ponto);
        sincronizavel.AddSub(ponto.subSincronizavel);
    }

    public void RemovePonto(PontoInteragivel ponto) {
        if (!pontos.Contains(ponto)) return;
        pontos.Remove(ponto);
        sincronizavel.RemoveSub(ponto.subSincronizavel);
    }
}
