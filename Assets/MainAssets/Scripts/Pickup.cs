using UnityEngine;
using UnityEngine.Audio;

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
        Vector3 spawnPos = transform.position;

        // add random offset on X and Z
        spawnPos.x += Random.Range(-0.5f, 0.5f);
        spawnPos.z += Random.Range(-0.5f, 0.5f);

        transform.position = spawnPos;

        // one tile below
        Vector3 below = transform.position + Vector3.down;
        Collider[] hits = Physics.OverlapSphere(below, 0.4f);

        bool hasInteractable = System.Array.Exists(hits, c => c.CompareTag("Interactable"));

        if (!hasInteractable)
        {
            transform.position = below;
            Debug.Log("Moved pickup down by 1");
        }
        else
        {
            Debug.Log("Landed on " + hits[0].name);
        }

        player = FindAnyObjectByType<InputPlayer>().transform;
        startPos = transform.position;

    }

    void Update()
    {
        // Rotate around Y axis
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

        // Float up and down with a sine wave
        float newY = startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        //magnet
        MagnetToPlayer();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if (!other.isTrigger)
            {

                // Play pop sound with random pitch + louder volume
                GameObject tempGO = new GameObject("TempAudio");
                tempGO.transform.position = transform.position;

                AudioSource aSource = tempGO.AddComponent<AudioSource>();
                aSource.clip = popSound;
                aSource.pitch = Random.Range(0.9f, 1.1f); // random pitch
                aSource.volume = 10f; // louder
                aSource.spatialBlend = 1f; // 3D sound
                aSource.Play();

                Destroy(tempGO, popSound.length / aSource.pitch); // cleanup after sound finishes

                player.GetComponent<Inventory>().CollectPickup(materialInventoryNumber);
                Destroy(gameObject);
            }
        }
    }

    void MagnetToPlayer()
    {
        if (player == null) return;

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
