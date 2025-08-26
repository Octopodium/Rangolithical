using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Algoritmo simples de vagueio que pode ser usado nos inimigos e futuros NPC's
/// </summary>
public class WayPointGenerico : MonoBehaviour {
    public Transform[] waypoints;
    private int IndexPosicaoAtual = 0;
    private NavMeshAgent agent;

    void Awake() {
        agent = GetComponent<NavMeshAgent>();
        if (waypoints.Length > 0) {
            agent.SetDestination(waypoints[IndexPosicaoAtual].position);
        }
    }
    
    void Update() {
        if (!agent.pathPending && agent.remainingDistance < 1) {
            IndexPosicaoAtual = (IndexPosicaoAtual + 1) % waypoints.Length;
            agent.SetDestination(waypoints[IndexPosicaoAtual].position);
        }
    }
}
