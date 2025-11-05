using Mirror;
using UnityEngine;


public class SincronizadorSpawner : MonoBehaviour {
    public GameObject sincronizadorOnline, sincronizadorOffline;

    void Awake() {
        if (Sincronizador.instance != null) return;
        if (NetworkClient.connection is LocalConnectionToServer)
            Spawn();
    }

    void Spawn() {
        if (Sincronizador.instance != null) return;

        if (GameManager.instance.isOnline) {
            GameObject objeto = Instantiate(sincronizadorOnline);
            NetworkServer.Spawn(objeto);
        } else {
            Instantiate(sincronizadorOffline);
        }
    }
}
