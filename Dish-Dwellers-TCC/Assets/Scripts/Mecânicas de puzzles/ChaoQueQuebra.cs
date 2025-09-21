using System.Collections;
using UnityEngine;

public class ChaoQueDestroi : MonoBehaviour {
    [Header("Configs do Chão Quebrável")]
    [SerializeField] private float tempoAntesDeCair = 2f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float distanciaDoChao = 0.1f; 

    [Header("Tamanho da Zona de Checagem")]
    public Vector3 extensaoDoCubo = new Vector3(1.0f, 1.0f, 1.0f);

    [Header("Rotação da Zona")]
    public Vector3 rotacaoDoCubo = Vector3.zero; 

    private bool _playerNoChao;
    private bool estaDestruido = false;

    private void FixedUpdate() {
        Vector3 boxCenter = transform.position - (Vector3.up * distanciaDoChao);
        Quaternion boxRotation = Quaternion.Euler(rotacaoDoCubo);
        _playerNoChao = Physics.CheckBox(boxCenter, extensaoDoCubo * 0.5f, boxRotation, playerLayer);

        if (_playerNoChao && !estaDestruido) {
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
        Quaternion boxRotation = Quaternion.Euler(rotacaoDoCubo);
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(boxCenter, boxRotation, Vector3.one);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(Vector3.zero, extensaoDoCubo);
        Gizmos.matrix = Matrix4x4.identity;
    }
}
