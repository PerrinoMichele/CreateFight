using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UI;
using TMPro;

public class InputPlayer : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    public FloatingJoystick rightJoystick;
    public AudioClip wooshSound;
    public DynamicJoystick leftJoystick;
    public int BlocksCollected = 0;
    public Button blockButton;
    public GameObject cubePrefab;
    public LayerMask obstacleLayer;
    public GameObject bulletPrefab;

    private Vector3 rightLookDir;
    private Vector3 lastLookDir;
    private Vector3 leftLookDir;
    private float leftJoystickX;
    private float leftJoystickY;
    private Rigidbody rigidbody;
    private GameObject aimEffect;
    private UnityEngine.Touch rightTouch;
    private Vector3 spawnPos;
    private GameObject nearestInteractable;
    private AudioSource audioSource;
    private bool isPressingButton;
    public bool isAttacking;

    void OnDrawGizmos()
    {
        if(nearestInteractable == null) { return; }
        Gizmos.color = Color.red; // Set color
        Gizmos.DrawWireSphere(nearestInteractable.transform.position, 1f); // Draw a wire sphere with radius 1
    }

    private void Start()
    {
        isAttacking = false;
        rigidbody = GetComponent<Rigidbody>();
        Transform aimTransform = transform.Find("Aim");
        aimEffect = aimTransform.gameObject;
        Transform hitTransform = transform.Find("Hit");
        GameObject background = rightJoystick.transform.GetChild(0).gameObject;
        GameObject handle = background.transform.GetChild(0).gameObject;
        blockButton.interactable = false;
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {

        float rightJoystickX = rightJoystick.Horizontal;
        float rightJoystickY = rightJoystick.Vertical;
        leftJoystickX = leftJoystick.Horizontal;
        leftJoystickY = leftJoystick.Vertical;
        rightLookDir = new Vector3(rightJoystickX, 0f, rightJoystickY);
        leftLookDir = new Vector3(leftJoystickX, 0f, leftJoystickY);

        //Detect right joystick release
        if (Input.touchCount == 2)
        {
            UnityEngine.Touch touch0 = Input.GetTouch(0);
            UnityEngine.Touch touch1 = Input.GetTouch(1);
            if (touch0.position.x > touch1.position.x)
            {
                rightTouch = touch0;
            }
            else
            {
                rightTouch = touch1;
            }
            switch (rightTouch.phase)
            {
                case UnityEngine.TouchPhase.Began:
                    lastLookDir = rightLookDir;
                    break;
                case UnityEngine.TouchPhase.Moved:
                    lastLookDir = rightLookDir;
                    break;
                case UnityEngine.TouchPhase.Ended:
                    Attack(lastLookDir);
                    break;
            }
        }
        else if (Input.touchCount == 1)
        {
            UnityEngine.Touch touch0 = Input.GetTouch(0);
            if (touch0.position.x < Screen.width / 2)
            {
                return;
            }
            else
            {
                switch (touch0.phase)
                {
                    case UnityEngine.TouchPhase.Began:
                        lastLookDir = rightLookDir;
                        break;
                    case UnityEngine.TouchPhase.Moved:
                        lastLookDir = rightLookDir;
                        break;
                    case UnityEngine.TouchPhase.Ended:
                        Attack(lastLookDir);
                        break;
                }
            }
        }
        CalculateSpawnPos();
    }

    //Detect when to aim, not aim, move, not move
    void FixedUpdate()
    {

        if (rightLookDir != Vector3.zero)
        {
            Aim();
        }
        else if (rightLookDir == Vector3.zero)
        {
            aimEffect.SetActive(false);
        }

        if (leftLookDir != Vector3.zero)
        {
            Move();
        }
        else if (leftLookDir == Vector3.zero)
        {
            Stop();
        }

    }

    private void Aim()
    {
        aimEffect.SetActive(true);
        Quaternion lookRot = Quaternion.LookRotation(rightLookDir);
        transform.rotation = lookRot;
    }

    private void Move()
    {
        rigidbody.linearVelocity = new Vector3(leftJoystickX * moveSpeed, rigidbody.linearVelocity.y, leftJoystickY * moveSpeed);
    }

    private void Stop()
    {
        rigidbody.linearVelocity = new Vector3(0, rigidbody.linearVelocity.y, 0);
    }

    public void buildBlock()
    {
        isPressingButton = true;
        if (!Physics.CheckBox(CalculateSpawnPos(), Vector3.one * 0.2f, Quaternion.identity, obstacleLayer) && BlocksCollected > 0)
        {
            gameObject.transform.position = new Vector3(transform.position.x, 1.05f, transform.position.z);
            Instantiate(cubePrefab, spawnPos, Quaternion.identity);
            BlocksCollected--;
            UpdateBlockText();
        }
        StartCoroutine(ResettingButton());
    }

    IEnumerator ResettingButton()
    {
        yield return new WaitForSeconds(1f);
        isPressingButton = false;
    }

    private Vector3 CalculateSpawnPos()
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
        return spawnPos;
    }

    public void UpdateBlockText()
    {
        TextMeshProUGUI buttonText = blockButton.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.text = BlocksCollected.ToString();
        }
    }

    private void Attack(Vector3 lastLookDir)
    {
        if(!isPressingButton && !isAttacking)
        {
            isAttacking = true;
            if (rightLookDir == lastLookDir)
            {
                nearestInteractable = FindNearestInteractable();
                Vector3 direction = (nearestInteractable.transform.position - transform.position).normalized;
                direction.y = 0;
                Quaternion lookRot = Quaternion.LookRotation(direction);
                transform.rotation = lookRot;
                Instantiate(bulletPrefab);
                audioSource.PlayOneShot(wooshSound);
            }
            else
            {
                Instantiate(bulletPrefab);
                audioSource.PlayOneShot(wooshSound);
            }
            StartCoroutine(ResetCoolDown());
        }
    }

    GameObject FindNearestInteractable()
    {
        GameObject[] interactables = GameObject.FindGameObjectsWithTag("Interactable");
        GameObject nearest = null;
        float minDistance = 20f;

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

    IEnumerator ResetCoolDown()
    {
        yield return new WaitForSeconds(.3f);
        isAttacking = false;
    }


}
