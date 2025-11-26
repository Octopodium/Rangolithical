using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(Sincronizavel))]
public class Projectile : MonoBehaviour, SincronizaMetodo {
    [SerializeField] private float projectileSpeed = 7f;
    [SerializeField] private float lifeTime = 4.0f;
    [SerializeField] private float currentLifeTime;
    [SerializeField] private Player player;
    [SerializeField] private GameObject splashDeFogo; // Particula que é instanciada quando a bola explode.
    [SerializeField] private GameObject trail;
    [SerializeField] private VisualEffect trailFx;
    [SerializeField] private GameObject decalQueimado;
    public GameObject owner;
    private Vector3 direction;
    
    public AudioClip knockBackSom;
    public AudioClip vinhaQueimandoSom;
    private bool isReflected = false;


    [Header("<color=green> Lima coisas :")]
    [SerializeField] private bool refletirNormal;

    void Start() {
        direction = transform.forward; //Usa a direção inicial do disparo
        currentLifeTime = lifeTime;
    }

    void FixedUpdate() {
        transform.Translate(direction * projectileSpeed * Time.deltaTime, Space.World);

        if (currentLifeTime <= 0) {
            Destroy(gameObject);
        }
        currentLifeTime -= Time.fixedDeltaTime;
    }

    private void OnDestroy() {
        GameObject splash = Instantiate(splashDeFogo, transform.position, transform.rotation);
        Destroy(splash, 2.0f);

    }

    private void DeixarMarcaDeQueimado(ContactPoint contactPoint) {
        GameObject decal = Instantiate(decalQueimado);
        decal.transform.position = contactPoint.point;
        Debug.Log(contactPoint.normal);
        decal.transform.forward = contactPoint.normal;
    }

    [Sincronizar]
    public void Refletir(Vector3 pos, Quaternion rot) {
        gameObject.Sincronizar(pos, rot);

        //CODIGO DO PEDRO DE LIMA:

        //Reseta o lifetime:
        currentLifeTime = lifeTime;

        transform.SetPositionAndRotation(pos, rot);
        direction = transform.forward;

        //FIM DO CÓDIGO DO PEDRO DE LIMA

        AudioManager.PlaySounds(TiposDeSons.SHIELDHIT);
        isReflected = true;
    }

    private void OnCollisionEnter(Collision other) {
        Debug.Log("Colidiu com: " + other.gameObject.tag);

        if (other.gameObject.CompareTag("Escudo") && !isReflected) {

            //Tenta pegar o centro da proteção (protecao) do escudo para refletir 

            Escudo escudo = other.transform.GetComponentInParent<Escudo>();

            Refletir(escudo.pontoDeReflexao.position, escudo.pontoDeReflexao.rotation);
        }

        else if (isReflected && other.gameObject == owner) {
            Debug.Log("Colidiu");

            //Quando acerta o proprietário do projetil(ou seja, a torreta) coloca o mesmo no estado de stunado
            InimigoTorreta torreta = owner.GetComponent<InimigoTorreta>();
            if (torreta != null) {
                torreta.GetStunned();
            }
            Destroy(gameObject);
        }

        else if (other.gameObject.CompareTag("Torreta") && !isReflected) {
            return;
        }

        else if (other.transform.CompareTag("Queimavel")) {
            other.transform.GetComponent<ParedeDeVinhas>().ReduzirIntegridade();
            Destroy(gameObject);
        }

        else if (other.gameObject.CompareTag("Player") && !isReflected) {
            Player player = other.transform.GetComponent<Player>();
            if (player != null) {
                player.MudarVida(-1, AnimadorPlayer.fonteDeDano.FOGO);
                player.AplicarKnockback(transform);
                AudioManager.PlaySounds(TiposDeSons.KNOCKBACK);
            }
            Destroy(gameObject);
        }
        
        else if (other.transform.CompareTag("Chao") || other.transform.CompareTag("Parede")) {
            DeixarMarcaDeQueimado(other.GetContact(0));
            Destroy(gameObject);
        }

        //previsão pra caso houver colisão com outros obstáculos
        else {
            DeixarMarcaDeQueimado(other.GetContact(0));
            Destroy(gameObject);
        }
    }
}