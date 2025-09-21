using UnityEngine;

/// <summary>
/// Um PontoInteragivel serve apenas como um colisor para outro Interagivel. Quando o jogador se aproxima/interage com o ponto, a interação é redirecionada para o Interagivel referenciado.
/// </summary>
public class PontoInteragivel : InteragivelBase {
    public Interagivel interagivelParaRedirecionar { get; protected set; }
    public bool utilizarIndicadorProprio = true;

    void Awake() {
        if (interagivelParaRedirecionar != null) {
            interagivelParaRedirecionar.pontos.Add(this);
        }
    }

    public void SetInteragivelParaRedirecionar(Interagivel interagivel) {
        if (interagivelParaRedirecionar != null) {
            interagivelParaRedirecionar.pontos.Remove(this);
        }

        interagivelParaRedirecionar = interagivel;
        interagivelParaRedirecionar.pontos.Add(this);
    }

    public override void Interagir(Player jogador) {
        interagivelParaRedirecionar.ultimoPonto = this;
        interagivelParaRedirecionar.Interagir(jogador);
    }

    public override bool PodeInteragir(Player jogador) {
        return interagivelParaRedirecionar.PodeInteragir(jogador);
    }

    public override MotivoNaoInteracao NaoPodeInteragirPois(Player jogador) {
        return interagivelParaRedirecionar.NaoPodeInteragirPois(jogador);
    }
    
    public override void MostrarIndicador(bool mostrar, Indicador indicador, MotivoNaoInteracao motivo = MotivoNaoInteracao.Nenhum) {
        if (utilizarIndicadorProprio) base.MostrarIndicador(mostrar, indicador, motivo);
        else interagivelParaRedirecionar.MostrarIndicador(mostrar, indicador, motivo);
    }

    protected override void OnDrawGizmosSelected() {
        if (utilizarIndicadorProprio) base.OnDrawGizmosSelected();
    }
}
