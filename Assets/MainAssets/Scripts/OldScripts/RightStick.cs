using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RightStick : MonoBehaviour
{
    public FloatingJoystick joystick;
    public AudioClip wooshSound;
    public int BlocksCollected = 0;
    public GameObject cubePrefab;
    public LayerMask obstacleLayer;
    public Button blockButton;

    private Vector3 spawnPos;
    private AudioSource audioSource;
    private float joystickX;
    private float joystickY;
    public Vector3 lookDir;
    private GameObject hitPreview;
    private bool isHitting;
    private bool hittingCoroutineIsPlaying = false;
    public GameObject hitEffect;
    private GameObject aimEffect;
    private GameObject nearestInteractable;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Transform hitTransform = transform.Find("Hit");
        hitEffect = hitTransform.gameObject;
        Transform aimTransform = transform.Find("Aim");
        aimEffect = aimTransform.gameObject;
        blockButton.interactable = false;
        UpdateBlockText();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Interactable")
        {
            other.gameObject.GetComponent<Cube>().GetHit();
        }

    }

    void Update()
    {
        nearestInteractable = FindNearestInteractable();
        joystickX = joystick.Horizontal;
        joystickY = joystick.Vertical;

        if (nearestInteractable != null && joystick.Horizontal == 0 && joystick.Vertical == 0)
        {
            lookDir = (nearestInteractable.transform.position - transform.position).normalized;
            Rotate();
        }

        lookDir = new Vector3(joystickX, 0f, joystickY);

        if (lookDir != Vector3.zero)
        {
            Rotate();
            aimEffect.SetActive(true);
            //isHitting = true;
        }



        else if(lookDir == Vector3.zero && aimEffect.activeInHierarchy)
        {
            aimEffect.SetActive(false);
        }

        if(hitEffect.activeInHierarchy) { GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation; }

        CalculateSpawnPos();

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.position.x > Screen.width / 2 && !hitEffect.activeInHierarchy)
            {
                switch (touch.phase)
                {
                    case TouchPhase.Ended:
                        aimEffect.SetActive(false);
                        StartCoroutine(SpawnHitTrigger());
                        break;
                }
            }

        }
    }



    GameObject FindNearestInteractable()
    {
        GameObject[] interactables = GameObject.FindGameObjectsWithTag("Interactable");
        GameObject nearest = null;
        float minDistance = 1.5f; 

        foreach (GameObject obj in interactables)
        {
            float distance = Vector3.Distance(transform.position, obj.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = obj;
            }
        }
        return nearest;
    }

    IEnumerator SpawnHitTrigger()
    {
            hitEffect.SetActive(true);
            audioSource.PlayOneShot(wooshSound);
            yield return new WaitForSeconds(.2f);
            hitEffect.SetActive(false);
    }

    private void Rotate()
    {
        if(!hitEffect.activeInHierarchy)
        {
            Quaternion lookRot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 40f);
        }

    }

















    private void CalculateSpawnPos()
    {
        spawnPos = new Vector3(Mathf.Round(transform.position.x), 0, Mathf.Round(transform.position.z));
        if (BlocksCollected == 0 || Physics.CheckBox(spawnPos, Vector3.one * 0.2f, Quaternion.identity, obstacleLayer))
        {
            blockButton.interactable = false;
        }
        else
        {
            blockButton.interactable = true;
        }
    }

    public void buildBlock()
    {
        if (!Physics.CheckBox(spawnPos, Vector3.one * 0.2f, Quaternion.identity, obstacleLayer) && BlocksCollected > 0)
        {
            gameObject.transform.position = new Vector3(transform.position.x, 1.05f, transform.position.z);
            Instantiate(cubePrefab, spawnPos, Quaternion.identity);
            BlocksCollected--;
            UpdateBlockText();
        }

    }
    public void UpdateBlockText()
    {
        TextMeshProUGUI buttonText = blockButton.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.text = BlocksCollected.ToString();
        }
    }

}
