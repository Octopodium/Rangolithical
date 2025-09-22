using UnityEngine;

public class Desativador : MonoBehaviour {
    public GameObject objetoDesativavel;

    public void DesativarObjeto() {
        objetoDesativavel.SetActive(false);
    }
}
