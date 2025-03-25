using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RightStick : MonoBehaviour
{
    public FloatingJoystick joystick;
    public AudioClip popSound;
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
    private GameObject hitEffect;
    private GameObject aimEffect;

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
        joystickX = joystick.Horizontal;
        joystickY = joystick.Vertical;
        lookDir = new Vector3(joystickX, 0f, joystickY);


        if (lookDir != Vector3.zero)
        {
            Rotate();
            aimEffect.SetActive(true);
            //isHitting = true;
        }

        if(lookDir == Vector3.zero && aimEffect.activeInHierarchy && !hitEffect.activeInHierarchy)
        {
            aimEffect.SetActive(false);
            StartCoroutine(SpawnHitTrigger());
        }
        else if(lookDir == Vector3.zero && aimEffect.activeInHierarchy)
        {
            aimEffect.SetActive(false);
        }

            CalculateSpawnPos();

        //if(isHitting && !hittingCoroutineIsPlaying)
        //{
        //    StartCoroutine(SpawnTrigger());
        //}
    }

    IEnumerator SpawnHitTrigger()
    {
            hitEffect.SetActive(true);
            yield return new WaitForSeconds(.05f);
            hitEffect.SetActive(false);
    }

    private void Rotate()
    {
        Quaternion lookRot = Quaternion.LookRotation(lookDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 40f);
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
