using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BrokenGlassTransition : ITransicao
{
    [SerializeField] private Image image;
    [SerializeField] private float dellay;
    [SerializeField] private float duracao;
    [SerializeField] private float duracaoFadeOut;
    private static int mainTexID = Shader.PropertyToID("_MainTexture");
    private static int alphaID = Shader.PropertyToID("_Alpha");
    private static int angleOffsetID = Shader.PropertyToID("_AngleOffset");


    private void Start() {
        image = GetComponent<Image>();
    }

    private void OnEnable() => StartCoroutine(PlayTransicao());

    public override float GetDuracao() =>  duracao + duracaoFadeOut;

    public override IEnumerator PlayTransicao() {
        image.materialForRendering.SetTexture(mainTexID, CaptureCameraTexture());
        image.materialForRendering.SetFloat(alphaID, 1);
        float angleOffset = Random.Range(10, 100);
        image.materialForRendering.SetFloat(angleOffsetID, angleOffset);
        yield return new WaitForSecondsRealtime(duracao);
        float timer = duracaoFadeOut;
        while(timer > 0) {
            timer -= Time.deltaTime;
            float alpha = Mathf.Lerp(0.0f, 0.7f, timer / duracaoFadeOut);
            image.materialForRendering.SetFloat(alphaID, alpha);
            yield return null;
        }
        gameObject.SetActive(false);
    }

}
