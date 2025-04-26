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
            Debug.Log("Destroyed VFX in edit mode: " + name);
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
            if(other.GetComponent<Bomb>() != null)
            {
                other.GetComponent<Bomb>().ExplodeBomb();
            }
            if(other.GetComponent<InputPlayer>())
            {
                audioSource.PlayOneShot(ugh);
                other.transform.position = new Vector3(0, 4, 0);
            }
            if(other.GetComponent<Entity>())
            {
                //other.transform.position = other.GetComponent<GeneralEnemy>().startPos;
                Destroy(other);
            }
            else
            {
                Destroy(other.gameObject);
            }
                
        }
        
    }
}