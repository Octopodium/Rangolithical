using UnityEngine;
using System.Collections;

public class SpritePisca : MonoBehaviour {
    [SerializeField] private Material piscaMat;
    Material defaultMat;
    Renderer render;

    [Tooltip("Para resultados melhores, recomenda-se o uso de numeros impares")]
    public int piscadas;


    private void Awake() {
        render = GetComponent<Renderer>();
        defaultMat = render.material;
    }

    public void Piscar(float duracao) {
        Debug.Log($"<color=green> piscou!");
        StartCoroutine(PiscaSprite(duracao));
    }

    IEnumerator PiscaSprite(float duracao) {
        float step = duracao / piscadas;

        for (int i = 0; i < piscadas; i++) {
            if (i % 2 == 0) {
                render.material = piscaMat;
            }
            else {
                render.material = defaultMat;
            }
            float timer = step;

            while (timer > 0) {
                timer -= Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
        }

        render.material = defaultMat;
    }
}
