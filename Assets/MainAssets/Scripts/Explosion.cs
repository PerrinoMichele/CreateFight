using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float explosionTime;
    void Start()
    {
        Destroy(gameObject, explosionTime); // destroy after 1 second
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            if(other.GetComponent<Bomb>() != null)
            {
                other.GetComponent<Bomb>().ExplodeBomb();
            }
            Destroy(other.gameObject);
        }
        
    }
}