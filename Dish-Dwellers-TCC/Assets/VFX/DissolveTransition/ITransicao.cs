using UnityEngine;
using System.Collections;

public abstract class ITransicao : MonoBehaviour {
    
    abstract public float GetDuracao();
    abstract public IEnumerator PlayTransicao();

}
