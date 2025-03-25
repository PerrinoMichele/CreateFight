using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RightStick : MonoBehaviour
{
    public FloatingJoystick joystick;
    public AudioClip digSound;
    public int BlocksCollected = 0;
    public GameObject cubePrefab;
    public LayerMask obstacleLayer;
    public Button blockButton;

    private Vector3 spawnPos;
    private AudioSource audioSource;
    private float joystickX;
    private float joystickY;
    private Vector3 lookDir;
    private GameObject hitPreview;
    private bool isHitting;
    private bool hittingCoroutineIsPlaying = false;
    private GameObject hit;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Transform hitTransform = transform.Find("Hit");
        hit = hitTransform.gameObject;
        blockButton.interactable = false;
        UpdateBlockText();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Interactable")
        {
            other.gameObject.GetComponent<Cube>().GetHit();

        }

    }

    void Update()
    {
        joystickX = joystick.Horizontal;
        joystickY = joystick.Vertical;
        lookDir = new Vector3(joystickX, 0f, joystickY);

        if(lookDir == Vector3.zero)
        {
            isHitting = false;
            hittingCoroutineIsPlaying = false;
        }

        else if (lookDir != Vector3.zero)
        {
            Rotate();
            isHitting = true;
        }

        else if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Ended:
                    isHitting = false;
                    
                    break;
            }
        }

        if(BlocksCollected == 0)
        {
            blockButton.interactable = false;
        }
        else
        {
            blockButton.interactable = true;
        }

        if(isHitting && !hittingCoroutineIsPlaying)
        {
            StartCoroutine(SpawnTrigger());
        }
    }

    IEnumerator SpawnTrigger()
    {
        while(isHitting)
        {
            hittingCoroutineIsPlaying = true;
            hit.SetActive(true);
            yield return new WaitForSeconds(.05f);
            hit.SetActive(false);
            yield return new WaitForSeconds(.4f);
        }

    }

    private void Rotate()
    {
        Quaternion lookRot = Quaternion.LookRotation(lookDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 40f);
    }


    public void buildBlock()
    {
        spawnPos = new Vector3(Mathf.Round(transform.position.x), 0, Mathf.Round(transform.position.z));

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
