using UnityEngine;

public interface Interacao {
    public abstract void Interagir(Player jogador);
}

public enum MotivoNaoInteracao { Nenhum, Fraco, Trancado, Cancelar }

public interface InteracaoCondicional : Interacao {
    /// <summary>
    /// Condições para que o jogador possa interagir com o objeto. 
    /// Se falso, será desconsiderada como item interagivel por completo.
    /// </summary>
    /// <param name="jogador">Jogador que interagiu</param>
    /// <returns>Positivo se pode ser interagido</returns>
    public abstract bool PodeInteragir(Player jogador);

    /// <summary>
    /// Motivos pelo qual o jogador não pode interagir, chamado após PodeInteragir retornar true, checa condições que serão informadas para o jogador pelo Indicador.
    /// </summary>
    /// <param name="jogador">Jogador que interagiu</param>
    /// <returns>Se o jogador pode interagir, retorna MotivoNaoInteracao.Nenhum, caso contrário, retorna o motivo pelo qual o jogador não pode interagir com o Interagivel.</returns>
    public abstract MotivoNaoInteracao NaoPodeInteragirPois(Player jogador);
}
