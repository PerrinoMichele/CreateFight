using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class PickupFloat : MonoBehaviour
{
    [Header("Rotation")]
    public float rotationSpeed = 50f;

    [Header("Floating")]
    public float floatAmplitude = 0.25f; // How far up and down it moves
    public float floatFrequency = 1f;    // Speed of the up/down motion

    public int materialInventoryNumber;
    private Transform player;        // assign player transform in inspector
    public float magnetRange = .5f;  // distance where magnet starts
    public float magnetSpeed = 2f;  // how fast it flies to player
    public AudioClip popSound;   // assign in inspector
    private AudioSource audioSource;

    private Vector3 startPos;
    private Vector3 targetPos;

    void Start()
    {
        //if(transform.position.y > 0)
        //{
        //    transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        //}
        Vector3 spawnPos = transform.position;

        // add random offset on X and Z
        spawnPos.x += Random.Range(-0.5f, 0.5f);
        spawnPos.z += Random.Range(-0.5f, 0.5f);

        transform.position = spawnPos;

        player = FindAnyObjectByType<InputPlayer>().transform;
        startPos = transform.position;
        audioSource = FindFirstObjectByType<AudioSource>();

        StartCoroutine(BlinkAndDie());
    }

    IEnumerator BlinkAndDie()
    {
        yield return new WaitForSeconds(30f);   // wait before blinking

        MeshRenderer rend = GetComponent<MeshRenderer>();
        if (rend == null) yield break;

        int blinks = 3;
        float blinkDuration = 0.15f;

        for (int i = 0; i < blinks; i++)
        {
            rend.enabled = false;
            yield return new WaitForSeconds(blinkDuration);
            rend.enabled = true;
            yield return new WaitForSeconds(blinkDuration);
        }

        Destroy(gameObject);
    }

    void Update()
    {
        // Rotate around Y axis
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

        // Float up and down with a sine wave
        //float newY = startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        //transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        //magnet
        MagnetToPlayer();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && !other.isTrigger)
        {
            
            if (player.GetComponent<Inventory>().itemsAmounts[materialInventoryNumber] < 50)
            {
                audioSource.pitch = Random.Range(0.9f, 1.2f);
                audioSource.PlayOneShot(popSound);
                player.GetComponent<Inventory>().CollectPickup(materialInventoryNumber);
                Destroy(gameObject);
            }

        }
    }

    void MagnetToPlayer()
    {
        if (player == null) return;
        if (player.GetComponent<Inventory>().itemsAmounts[materialInventoryNumber] >= 50) { return; }

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist < magnetRange)
        {
            // Move toward player
            transform.position = Vector3.MoveTowards(
                transform.position,
                player.position,
                magnetSpeed * Time.deltaTime
            );
        }
    }
}
