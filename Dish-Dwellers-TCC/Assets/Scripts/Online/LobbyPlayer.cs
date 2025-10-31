using UnityEngine;
using Mirror;

/// <summary>
/// Classe que representa um jogador no lobby (antes de entrar no jogo)
/// </summary>
public class LobbyPlayer : NetworkBehaviour {

    public float atualizaPingACada = 0.5f; // Tempo em segundos para atualizar o ping

    [SyncVar] public bool isPlayerOne = false;

    void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    void Start(){
        if (ConnectionUI.instance != null) {
            ConnectionUI.instance.EntrouNoLobby();
        }
    }

    public void GerarPlayer() {
        if (isLocalPlayer) {
            CmdGerarPlayer();
        }
    }
    
    [Command]
    public void CmdGerarPlayer() {
        DishNetworkManager manager = (DishNetworkManager)NetworkManager.singleton;
        manager.GerarPlayer(this);
    }


    // Tentar começar o jogo (qualquer um pode fazer desde que os dois estejam prontos)
    public void TentarComecar() {
        if (isLocalPlayer) {
            CmdTentarComecar();
        }
    }

    [Command]
    void CmdTentarComecar() {
        DishNetworkManager manager = (DishNetworkManager)NetworkManager.singleton;
        manager.IniciarJogo();
    }


    public void FoiDesconectado() {
        Debug.Log("Desconectado!");
    }
}
