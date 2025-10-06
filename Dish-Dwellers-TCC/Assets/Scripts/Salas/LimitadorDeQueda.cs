using UnityEngine;
using System.Collections;

public class LimitadorDeQueda : MonoBehaviour
{
    public bool checarGlobalmente = false;

    void Awake() {
        if (checarGlobalmente) {
            if (GameManager.instance.jogadores.Count == 0) GameManager.instance.OnPlayersInstanciados += SetarCorotina;
            else SetarCorotina(null, null);
        }
    }

    void SetarCorotina(Player _p1, Player _p2) {
        GameManager.instance.OnPlayersInstanciados -= SetarCorotina;
        StartCoroutine(ChecarPosicaoPlayers());
    }

    IEnumerator ChecarPosicaoPlayers() {
        while (gameObject.activeSelf) {
            foreach (Player player in GameManager.instance.jogadores) {
                if (player.transform.position.y < transform.position.y) {
                    player.MudarVida(-99, "Queda");
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }

    private void OnTriggerEnter(Collider other){
        if (other.CompareTag("Player")) {
            Player player = other.GetComponent<Player>();
            if (player != null) player.MudarVida(-99, "Queda");
        }

        Destrutivel destrutivel = other.GetComponent<Destrutivel>();
        if(destrutivel){
            destrutivel.OnDestruido.Invoke();
        }
    }
}
