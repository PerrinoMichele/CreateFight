using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InputPlayer : MonoBehaviour
{
    public int cacapupu = 0;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxPlayerHeight;

    public FloatingJoystick rightJoystick;
    public AudioClip wooshSound;
    public DynamicJoystick leftJoystick;

    public Button blockButton;
    public GameObject woodBlockPrefab;
    public GameObject rockBlockPrefab;

    public LayerMask obstacleLayer;
    public GameObject woodBulletPrefab;
    public GameObject rockBulletPrefab;
    public GameObject bombBlockPrefab;
    public AudioClip popSound;
    public AudioClip popSound2;
    public GameObject smokeVFX;

    private Inventory inventory;
    private Vector3 rightLookDir;
    private Vector3 lastLookDir;
    private Vector3 leftLookDir;
    private float leftJoystickX;
    private float leftJoystickY;
    private Rigidbody rigidbody;
    private GameObject woodAimEffect;
    private GameObject rockAimEffect;
    private UnityEngine.Touch rightTouch;
    private Vector3 spawnPos;
    private GameObject nearestInteractable;
    private AudioSource audioSource;
    public bool isPressingButton;
    public bool isAttacking;
    private Quaternion rotation;
    public AudioClip ugh;
    public GameObject mapGen;


    void OnDrawGizmos()
    {
        if(nearestInteractable == null) { return; }
        Gizmos.color = Color.red; // Set color
        Gizmos.DrawWireSphere(nearestInteractable.transform.position, 1f); // Draw a wire sphere with radius 1
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Cube>() != null)
        {
            other.GetComponent<Cube>().enabled = true;
            //other.GetComponent<HealthSystem>().enabled = true;
            other.GetComponent<Collider>().isTrigger = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Cube>() != null)
        {
            other.GetComponent<Cube>().enabled = false;
            //other.GetComponent<HealthSystem>().enabled = false;
            other.GetComponent<Collider>().isTrigger = true;
        }
    }

    private void Start()
    {
        
        isAttacking = false;
        rigidbody = GetComponent<Rigidbody>();
        inventory = GetComponent<Inventory>();
        Transform woodAimTransform = transform.Find("WoodBulletAim");
        woodAimEffect = woodAimTransform.gameObject;
        Transform rockAimTransform = transform.Find("RockBulletAim");
        rockAimEffect = rockAimTransform.gameObject;
        //Transform hitTransform = transform.Find("Hit");
        GameObject background = rightJoystick.transform.GetChild(0).gameObject;
        GameObject handle = background.transform.GetChild(0).gameObject;
        audioSource = FindFirstObjectByType<AudioSource>();
        rotation = Quaternion.Euler(0, 45, 0);
    }

    public void RespawnPlayer()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //rigidbody.linearVelocity = Vector3.zero;
        //rigidbody.angularVelocity = Vector3.zero;
        //transform.position = new Vector3(0, 4, 0);
        //audioSource.PlayOneShot(ugh);
        //mapGen.GetComponent<mapGenerator>().GenerateMap();
    }

    private void Update()
    {
        if(transform.position.y < -2)
        {
            RespawnPlayer();
        }

        float rightJoystickX = rightJoystick.Horizontal;
        float rightJoystickY = rightJoystick.Vertical;
        leftJoystickX = leftJoystick.Horizontal;
        leftJoystickY = leftJoystick.Vertical;
        rightLookDir = rotation * new Vector3(rightJoystickX, 0f, rightJoystickY);
        leftLookDir = new Vector3(leftJoystickX, 0f, leftJoystickY);

        CalculateSpawnPos();
        //Detect right joystick release
        if (Input.touchCount == 2)
        {
            CalculateSpawnPos();
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
            CalculateSpawnPos();
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
            woodAimEffect.SetActive(false);
            rockAimEffect.SetActive(false);
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
        if(inventory.woodButton.image.color == Color.white)
        {
            woodAimEffect.SetActive(true);
            Quaternion lookRot = Quaternion.LookRotation(rightLookDir);
            transform.rotation = lookRot;
        }
        else if (inventory.rockButton.image.color == Color.white)
        {
            rockAimEffect.SetActive(true);
            Quaternion lookRot = Quaternion.LookRotation(rightLookDir);
            transform.rotation = lookRot;
        }
    }

    private void Move()
    {
        rigidbody.linearVelocity = rotation * new Vector3(leftJoystickX * moveSpeed, rigidbody.linearVelocity.y, leftJoystickY * moveSpeed);
    }

    private void Stop()
    {
        rigidbody.linearVelocity = new Vector3(0, rigidbody.linearVelocity.y, 0);
    }

    //BLOCK BUILDING
    public void buildBlock()
    {
        if (blockButton.interactable == true)
        {
            isPressingButton = true;
            StartCoroutine(ResettingButton());
            if (!Physics.CheckBox(CalculateSpawnPos(), Vector3.one * 0.2f, Quaternion.identity, obstacleLayer) && transform.position.y < maxPlayerHeight)
            {
                int blockIndex = GetCurrentMaterial();
 
                if (blockIndex == 0)
                {
                    gameObject.transform.position = new Vector3(transform.position.x, transform.position.y + 1.05f, transform.position.z);
                    Instantiate(woodBlockPrefab, spawnPos, Quaternion.identity);
                    //inventory.UpdateBlockText();//pass item number
                    audioSource.PlayOneShot(popSound);
                }
                else
                {
                    if (inventory.currentMaterialAmount > 0)
                    {
                        gameObject.transform.position = new Vector3(transform.position.x, transform.position.y + 1.05f, transform.position.z);
                        if(blockIndex == 1)
                        {
                            Instantiate(smokeVFX, spawnPos, Quaternion.identity);
                            Instantiate(rockBlockPrefab, spawnPos, Quaternion.identity);
                        }
                        if (blockIndex == 2)
                        {
                            Instantiate(bombBlockPrefab, spawnPos, Quaternion.identity);
                        }

                        inventory.itemsAmounts[blockIndex]--;
                        inventory.currentMaterialAmount = inventory.itemsAmounts[blockIndex];
                        inventory.UpdateBlockText(blockIndex);
                        audioSource.PlayOneShot(popSound2);
                    }
                    else { inventory.SwitchToWood(); }
                }
            }

        }
    }

    private int GetCurrentMaterial()
    {
        if (inventory.woodButton.image.color == Color.white)
        {
            return 0;
        }
        else if (inventory.rockButton.image.color == Color.white)
        {
            return 1;
        }
        else if (inventory.bombButton.image.color == Color.white)
        {
            return 2;
        }
        else { return 0; }
    }

    public IEnumerator ResettingButton()
    {
        yield return new WaitForSeconds(1f);
        isPressingButton = false;
    }

    private Vector3 CalculateSpawnPos()
    {
        spawnPos = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
        if(spawnPos.y > 0) { spawnPos.y = 0; }

        if (Physics.CheckBox(spawnPos, Vector3.one * 0.2f, Quaternion.identity, obstacleLayer) || (transform.position.y >= maxPlayerHeight)
            || Physics.CheckBox(transform.position + Vector3.up * 1f, Vector3.one * 0.1f, Quaternion.identity, obstacleLayer))
        {
            blockButton.interactable = false;
        }
        else
        {
            blockButton.interactable = true;
        }
        return spawnPos;
    }

    //BULLETSHOOTING
    private void Attack(Vector3 lastLookDir)
    {
        int blockIndex = GetCurrentMaterial();

        if (!isPressingButton && !isAttacking)
        {
            isAttacking = true;
            if (rightLookDir == lastLookDir)
            {
                nearestInteractable = FindNearestInteractable();
                
                if(nearestInteractable != null)
                {
                    Vector3 direction = (nearestInteractable.transform.position - transform.position).normalized;
                    direction.y = 0;
                    Quaternion lookRot = Quaternion.LookRotation(direction);
                    transform.rotation = lookRot;
                    SpawnBullet(blockIndex);
                }
            }
            else
            {
                SpawnBullet(blockIndex);
            }

            StartCoroutine(ResetCoolDown());
        }
    }

    private void SpawnBullet(int blockIndex)
    {
        if (blockIndex == 0)
        {
            Instantiate(woodBulletPrefab);
            //inventory.UpdateBlockText();//pass item number
        }
        else if (inventory.currentMaterialAmount > 0)
        {
            if (blockIndex == 1)
            {
                Instantiate(rockBulletPrefab);
                inventory.itemsAmounts[1]--;
                inventory.currentMaterialAmount = inventory.itemsAmounts[blockIndex];
                inventory.UpdateBlockText(1);
            }
            else if (blockIndex == 2)
            {
                gameObject.transform.position = new Vector3(transform.position.x, transform.position.y + 1.05f, transform.position.z);
                Instantiate(bombBlockPrefab, spawnPos, Quaternion.identity);
                inventory.itemsAmounts[2]--;
                inventory.currentMaterialAmount = inventory.itemsAmounts[blockIndex];
                inventory.UpdateBlockText(2);
                audioSource.PlayOneShot(popSound);
            }
        }
        else { inventory.SwitchToWood(); }
    }

    GameObject FindNearestInteractable()
{
    float searchRadius = 5f; // Adjust this radius as needed
    float heightTolerance = .2f; // Allow slight height differences
    GameObject nearest = null;
    float minDistance = searchRadius;

    Collider[] colliders = Physics.OverlapSphere(transform.position, searchRadius);
    foreach (Collider col in colliders)
    {
        GameObject obj = col.gameObject;
        
        // Check if it has the correct tag
        if (obj.CompareTag("Interactable") || obj.CompareTag("Enemy"))
        {
            // Ensure it's on the same height level
            if (Mathf.Abs(obj.transform.position.y - transform.position.y) < heightTolerance)
            {
                float distance = Vector3.Distance(transform.position, obj.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = obj;
                }
            }
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
