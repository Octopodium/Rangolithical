using UnityEngine;

public class TrocarPersonagemNoLobby : MonoBehaviour, InteracaoCondicional {
    public void Interagir(Player jogador) {
        if (!PodeInteragir(jogador)) return;

        GameManager.instance.Despausar();

        if (GameManager.instance.modoDeJogo == ModoDeJogo.MULTIPLAYER_ONLINE) GameManager.instance.RedefinirControlesMultiplayerOnline();
        else GameManager.instance.RedefinirControlesMultiplayerLocal();
    }

    public bool PodeInteragir(Player jogador) {
        return GameManager.instance.jogadores.Count > 1;
    }

    public MotivoNaoInteracao NaoPodeInteragirPois(Player jogador) {
        return MotivoNaoInteracao.Nenhum;
    }
}
