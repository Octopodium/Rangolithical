using UnityEngine;

public class LobbyController : MonoBehaviour {
    void Start() {
        LobbyPlayer[] players = FindObjectsByType<LobbyPlayer>(FindObjectsSortMode.None);
        foreach (LobbyPlayer player in players) {
            player.GerarPlayer();
        }
    }
}
