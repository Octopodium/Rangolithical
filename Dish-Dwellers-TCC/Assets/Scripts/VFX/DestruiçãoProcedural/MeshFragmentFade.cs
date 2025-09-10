using UnityEngine;

public class MeshFragmentFade : MonoBehaviour{ 

    private int opacityHash = Shader.PropertyToID("_Opacity");
    private MeshRenderer meshRenderer;
    private MaterialPropertyBlock materialPropertyBlock;
    private float lifetime = 2.0f;
    private float age = 0.0f;
    private float fadeDuration = 2.0f;
    private float fadeProgression = 0.0f;
    private Vector3 initialScale;

    private void Start(){
        meshRenderer = GetComponent<MeshRenderer>();
        initialScale = transform.localScale;
        materialPropertyBlock = new MaterialPropertyBlock();
    }   

    private void FixedUpdate(){
        if(age < lifetime)
            age += Time.fixedDeltaTime;
        else if (fadeProgression < fadeDuration){
            float progression = fadeProgression / fadeDuration;
            transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, progression);
            materialPropertyBlock.SetFloat(opacityHash, (1f - progression) * 2f); // Precisa ser um valor entre 0 e 2 para funcionar com o Dithering node  
            meshRenderer.SetPropertyBlock(materialPropertyBlock);
            fadeProgression += Time.fixedDeltaTime;
        }
        else
            Destroy(gameObject);
    }

    public void SetLifetimeAndFadeDuration(float lifetime, float fadeDuration){
        this.lifetime = lifetime;
        this.fadeDuration = fadeDuration;
    }

}
