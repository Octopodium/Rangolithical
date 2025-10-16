using UnityEngine;

public class CollisionActivator : MonoBehaviour
{
    public GameObject objectToActivate;
    public string triggerObjectTag = "Player";

    void Start()
    {
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(triggerObjectTag))
        {
            if (objectToActivate != null)
            {
                objectToActivate.SetActive(true);
                Debug.Log(objectToActivate.name + " foi ativado pela colisão com " + collision.gameObject.name);
            }
            else
            {
                Debug.LogWarning("A variável 'objectToActivate' não foi definida no Inspector.", this.gameObject);
            }
        }
    }
}