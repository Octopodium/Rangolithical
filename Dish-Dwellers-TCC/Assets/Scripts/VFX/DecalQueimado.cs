using UnityEngine;

public class DecalQueimado : MonoBehaviour {
    
    private Renderer render;
    private MaterialPropertyBlock materialPropertyBlock;
    private static readonly int alphaID = Shader.PropertyToID("_Alpha");
    [SerializeField] private float lifetime;
    private float fadeTimer;
    [SerializeField] private float fadeDuration;
 

    private void Start() {
        render = GetComponent<Renderer>();
        materialPropertyBlock = new MaterialPropertyBlock();
        fadeTimer = fadeDuration;
    }

    private void LateUpdate() {
        if(lifetime <= 0) {
            fadeTimer -= Time.deltaTime;
            materialPropertyBlock.SetFloat(alphaID, fadeTimer / fadeDuration);
            render.SetPropertyBlock(materialPropertyBlock);
            if(fadeTimer <= 0) Destroy(gameObject);
        }
        else {
            lifetime -= Time.deltaTime;
        }
    }

}
