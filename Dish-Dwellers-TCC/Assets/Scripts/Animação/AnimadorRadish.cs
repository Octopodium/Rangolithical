using UnityEngine;

public class AnimadorRadish : MonoBehaviour{

    [SerializeField] private Animator animator;
    public static readonly int hooked = Animator.StringToHash("Hooked");


    private void Awake() => animator = GetComponent<Animator>();

    public void Ganchado(bool estaGanchado) => animator.SetBool(hooked, estaGanchado);

}
