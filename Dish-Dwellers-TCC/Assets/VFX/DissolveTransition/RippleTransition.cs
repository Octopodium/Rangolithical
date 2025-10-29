using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RippleTransition : ITransicao {

    [SerializeField] private Image image;
    [SerializeField] private float duracao = 2.0f;
    [SerializeField] private float dellay = 1.0f;
    [SerializeField] private float duracaoFadeIn = 0.75f;
    [SerializeField] private float duracaoFadeOut = 1.0f;
    [SerializeField] private float minAlpha = -0.1f; // Valores de alpha precisam ser deslocados para comportar a transição do smoothstep.
    [SerializeField] private float maxAlpha = 1.1f; // Valores de alpha precisam ser deslocados para comportar a transição do smoothstep.
    [SerializeField] private float minStrength = 0.0f;
    [SerializeField] private float maxStrength = 0.07f; // Valor maximo de força do shader é 0.1
    public static int strengthID = Shader.PropertyToID("_Strength");
    public static int alphaID = Shader.PropertyToID("_Alpha");
    public static int mainTextureID = Shader.PropertyToID("_MainTexture");

    
    private void Start() {
        image = GetComponent<Image>();
    }

    private void OnEnable() {
        StartCoroutine(PlayTransicao());
    }

    public override float GetDuracao() => dellay + duracaoFadeIn + duracao ;

    public override IEnumerator PlayTransicao() {
        float timer = 0.0f;
        image.materialForRendering.SetTexture(mainTextureID, CaptureCameraTexture());
        image.materialForRendering.SetFloat(alphaID, minAlpha);
        image.materialForRendering.SetFloat(strengthID, minStrength);
        yield return new WaitForSecondsRealtime(dellay);
        while(timer < duracaoFadeIn) {
            timer += Time.fixedUnscaledDeltaTime;
            image.materialForRendering.SetFloat(strengthID, Mathf.Lerp(minStrength, maxStrength, timer / duracaoFadeIn));
            yield return null;
        }
        yield return new WaitForSecondsRealtime(duracao);
        timer = 0.0f;
        while(timer < duracaoFadeOut) {
            timer += Time.fixedUnscaledDeltaTime;
            image.materialForRendering.SetFloat(alphaID, Mathf.Lerp(minAlpha, maxAlpha, timer / duracaoFadeOut));
            yield return null;
        }
        gameObject.SetActive(false);
    } 

}
