using Sandbox3D;
using UnityEngine;
using UnityEngine.Audio;

[ExecuteAlways]
public class Explosion : MonoBehaviour
{
    public float explosionTime;
    public AudioClip ugh;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = FindFirstObjectByType<AudioSource>();
        if (!Application.isPlaying)
        {       
            DestroyImmediate(gameObject);
        }
    }

    void Start()
    {
        if (!Application.isPlaying)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Destroy(gameObject, explosionTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            //if(other.GetComponent<InputPlayer>())
            //{

            //}
            //if (other.GetComponent<Bomb>() != null)
            //{
            //    other.GetComponent<Bomb>().ExplodeBomb();
            //}
            //if (other.GetComponent<Entity>())
            //{
            //    //other.transform.position = other.GetComponent<GeneralEnemy>().startPos;
            //    Destroy(other);
            //}
            if (!other.GetComponent<InputPlayer>())
            {
                if(other.GetComponent<GeneralEnemy>() != null)
                {
                    other.gameObject.GetComponent<HealthSystem>().GetHit(3);
                }
                if (other.GetComponent<Cube>() != null)
                {
                    other.gameObject.GetComponent<Cube>().GetHit(3);
                }
            }
        }
        else
        {
            if (other.GetComponent<Bomb>() != null)
            {
                other.GetComponent<Bomb>().ExplodeBomb();
            }
            if (other.GetComponent<Entity>())
            {
                //other.transform.position = other.GetComponent<GeneralEnemy>().startPos;
                Destroy(other);
            }
            else if (!other.GetComponent<InputPlayer>())
            {
                Destroy(other.gameObject);
            }
        }
        
    }
}