using UnityEngine;
using Mirror;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyController : MonoBehaviour {
    public Text lobbyId;


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
        if (lobby != null) {
            lobbyId.gameObject.SetActive(true);
            lobbyId.text = "[ ID: " + lobby.lobbyId + " ]";
        } else {
            lobbyId.gameObject.SetActive(false);
        }
    }
}
