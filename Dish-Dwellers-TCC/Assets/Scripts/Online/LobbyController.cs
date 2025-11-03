using UnityEngine;
using Mirror;
using System.Collections;

public class LobbyController : MonoBehaviour {
    public System.Action onLobbySetupFinished;
    void Start() {
        if (!NetworkClient.ready) {
            NetworkClient.Ready();
            NetworkClient.AddPlayer();
        }

        onLobbySetupFinished?.Invoke();
    }
}
