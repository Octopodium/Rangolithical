using System.Collections;
using UnityEngine;

public class ChaoQueDestroi : MonoBehaviour {
    [Header("Configs do Chão Quebrável")]
    [SerializeField] private float tempoAntesDeCair = 2f;
    [SerializeField] private float distanciaPlayer = 3f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float distanciaDoChao = 0.1f; 

    public Vector3 extensaoDoCubo = new Vector3(1.0f, 1.0f, 1.0f);
    private bool _playerNoChao;
    private bool estaDestruido = false;

    private void FixedUpdate() {
        Vector3 boxCenter = transform.position - (Vector3.up * distanciaDoChao);
        _playerNoChao = Physics.CheckBox(boxCenter, extensaoDoCubo, Quaternion.identity, playerLayer);

        if (_playerNoChao && !estaDestruido ) {
            StartCoroutine(DestruirChao());
        }
    }

    private IEnumerator DestruirChao() {
        yield return new WaitForSeconds(tempoAntesDeCair);

        gameObject.SetActive(false);
        estaDestruido = true;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Vector3 boxCenter = transform.position - (Vector3.up * distanciaDoChao);
        Gizmos.DrawWireCube(boxCenter, extensaoDoCubo * 2);
    }
}
