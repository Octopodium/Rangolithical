using UnityEngine;
using Mirror;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyController : IResetavel, SincronizaMetodo {
    public Text lobbyId;
    public Text salaInicialTxt;
    public Portal portal;

    public SalaInfo primeirissimaSala;
    public string proximaSala;


    public System.Action onLobbySetupFinished;
    void Start() {
        if (GameManager.instance.isOnline) {
            if (!NetworkClient.ready) {
                NetworkClient.Ready();
                NetworkClient.AddPlayer();
            }
        }
        
        onLobbySetupFinished?.Invoke();
        ShowID();
    }

    void ShowID() {
        BetterEOSLobby lobby = (NetworkManager.singleton as DishNetworkManager)?.GetLobby();
        if (lobby != null && PartidaInfo.instance.modo != PartidaInfo.Modo.Entrar) {
            lobbyId.gameObject.SetActive(true);
            lobbyId.text = "[ ID: " + lobby.lobbyId + " ]";
        } else {
            lobbyId.gameObject.SetActive(false);
        }
    }

    [Sincronizar]
    public void MudarSala(string sala, string nome) {
        gameObject.Sincronizar(sala, nome);
        proximaSala = sala;
        salaInicialTxt.text = nome;

        portal.salaEscolhidaSemPre = sala;
    }

    public override void OnReset() {
        MudarSala(primeirissimaSala.caminhoParaSala, primeirissimaSala.nomeDaSala);
    }
}
