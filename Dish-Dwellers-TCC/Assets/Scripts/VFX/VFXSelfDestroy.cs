using UnityEngine;
using UnityEngine.VFX;

public class VFXSelfDestroy : MonoBehaviour{

    private VisualEffect visualEffect;
    public static int maxLifetimeID = Shader.PropertyToID("MaxLifetime");


    private void Awake(){
        visualEffect = GetComponent<VisualEffect>();
        Destroy(gameObject, visualEffect.GetFloat(maxLifetimeID));
    }

}
