using UnityEngine;

public class EscolherSalaNoLobby : MonoBehaviour, InteracaoCondicional, SincronizaMetodo{
    public LobbyController lobby;
    public SeletorDeFase seletor;

    void Start() {
        seletor.salaSelecionada += HandleSalaSecionada;
    }

    public void Interagir(Player jogador) {
        if (jogador.ehJogadorAtual) {
            seletor.Mostrar();
        }
    }

    public bool PodeInteragir(Player jogador) {
        return GameManager.instance.jogadores.Count > 1;
    }

    public MotivoNaoInteracao NaoPodeInteragirPois(Player jogador) {
        return MotivoNaoInteracao.Nenhum;
    }

    void HandleSalaSecionada(SalaInfo sala) {
        SelecionarSala(sala.caminhoParaSala, sala.nomeDaSala);
    }

    [Sincronizar]
    public void SelecionarSala(string sala, string nome) {
        gameObject.Sincronizar(sala, nome);
        lobby.MudarSala(sala, nome);
        
    }
}
