using UnityEngine;

/// <summary>
/// Algoritmo simples de vagueio que pode ser usado nos inimigos e futuros NPC's
/// </summary>
public class WayPointGenerico : MonoBehaviour {
    public GameObject[] waypoints;
    private int IndexPosicaoAtual = 0;
    public float velocidade; //Velocidade de caminhamento entre os waypoints

    void Update() {
        if (Vector3.Distance(this.transform.position, waypoints[IndexPosicaoAtual].transform.position) < 1) {
            IndexPosicaoAtual++;
        }

        if (IndexPosicaoAtual >= waypoints.Length) {
            IndexPosicaoAtual = 0;
        }

        this.transform.LookAt(waypoints[IndexPosicaoAtual].transform);
        this.transform.Translate(0, 0, velocidade * Time.deltaTime);
    }
}
