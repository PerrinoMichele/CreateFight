using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InputPlayer : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxPlayerHeight;
    [SerializeField] LineRenderer lineRenderer;

    public FloatingJoystick rightJoystick;
    public AudioClip wooshSound;
    public FloatingJoystick leftJoystick;

    public Button blockButton;
    public GameObject woodBlockPrefab;
    public GameObject rockBlockPrefab;
    public GameObject bombBlockPrefab;

    public LayerMask obstacleLayer;
    public GameObject woodBulletPrefab;
    public GameObject rockBulletPrefab;
    public GameObject bombBulletPrefab;

    //public AudioClip popSound;
    public AudioClip popSound2;
    public GameObject groundImpactVFX;

    private Inventory inventory;
    private Vector3 rightLookDir;
    private Vector3 lastLookDir;
    private Vector3 leftLookDir;
    private float leftJoystickX;
    private float leftJoystickY;
    private Rigidbody rigidbody;
    private GameObject woodAimEffect;
    private GameObject rockAimEffect;
    private GameObject bombAimEffect;
    private UnityEngine.Touch rightTouch;
    private Vector3 blockSpawnPos;
    private Vector3 playerSpawnPos;
    private GameObject nearestInteractable;
    private AudioSource audioSource;
    public bool isPressingButton;
    public bool isAttacking;
    private Quaternion rotation;
    public AudioClip ugh;
    public GameObject mapGen;
    private bool isShooting = false;

    //ACTIVATE FOR LAVA
    private void OnCollisionEnter(Collision collision)
    {
        //ACTIVATE FOR LAVA

        //if(collision.gameObject.tag == "Ground")
        //{
        //    // little jump up
        //    transform.position = transform.position + Vector3.up * 2;

        //    if (GetComponent<HealthSystem>().canGetHit == true)
        //    {
        //        GetComponent<HealthSystem>().GetHit(1);
        //    }
        //}
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
        playerSpawnPos = transform.position;
        isAttacking = false;
        rigidbody = GetComponent<Rigidbody>();
        inventory = GetComponent<Inventory>();
        Transform woodAimTransform = transform.Find("WoodBulletAim");
        woodAimEffect = woodAimTransform.gameObject;
        Transform rockAimTransform = transform.Find("RockBulletAim");
        rockAimEffect = rockAimTransform.gameObject;
        Transform bombAimTransform = transform.Find("BombBulletAim");
        bombAimEffect = bombAimTransform.gameObject;
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
        //print(rightLookDir);
        leftLookDir = rotation * new Vector3(leftJoystickX, 0f, leftJoystickY);

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
            //CalculateSpawnPos();
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

        if (rightLookDir != Vector3.zero && !isShooting)
        {
            Aim();
        }
        else if (rightLookDir == Vector3.zero)
        {
            woodAimEffect.SetActive(false);
            rockAimEffect.SetActive(false);
            StartCoroutine(DisableAfterDelay());
            lineRenderer.enabled = false;
        }

        if (leftLookDir != Vector3.zero)
        {
            Move();
            AutoJumpIfNeeded();
        }
        else if (leftLookDir == Vector3.zero)
        {
            Stop();
        }
    }
    //tempSolution
    IEnumerator DisableAfterDelay()
    {
        yield return new WaitForSeconds(0.1f); // wait 0.1 sec
        bombAimEffect.SetActive(false);
    }

    private void Aim()
    {
        if(inventory.woodButton.transform.localScale == new Vector3(1.3f, 1.3f, 1f))
        {
            woodAimEffect.SetActive(true);
            Quaternion lookRot = Quaternion.LookRotation(rightLookDir);
            transform.rotation = lookRot;
        }

        else if (inventory.rockButton.transform.localScale == new Vector3(1.3f, 1.3f, 1f))
        {
            rockAimEffect.SetActive(true);
            Quaternion lookRot = Quaternion.LookRotation(rightLookDir);
            transform.rotation = lookRot;
        }

        //AIM with BOMB ---
        else if (inventory.bombButton.transform.localScale == new Vector3(1.3f, 1.3f, 1f))
        {
            lineRenderer.enabled = true;
            bombAimEffect.SetActive(true);
            Quaternion lookRot = Quaternion.LookRotation(rightLookDir);
            transform.rotation = lookRot;

            float strength = Mathf.Clamp01(rightLookDir.magnitude);
            float maxDistance = 6f;
            Vector3 localPos = bombAimEffect.transform.localPosition;
            localPos.z = strength * maxDistance;
            bombAimEffect.transform.localPosition = localPos;

            Vector3 desiredLocalPos = Vector3.forward * (strength * maxDistance);
            // Convert to world position
            Vector3 desiredWorldPos = transform.TransformPoint(desiredLocalPos);
            //// Snap to grid
            //desiredWorldPos.x = Mathf.Round(desiredWorldPos.x);
            //desiredWorldPos.y = Mathf.Round(desiredWorldPos.y);
            //desiredWorldPos.z = Mathf.Round(desiredWorldPos.z);
            // Convert back to local
            bombAimEffect.transform.localPosition = transform.InverseTransformPoint(desiredWorldPos);
            // Freeze child rotation
            bombAimEffect.transform.rotation = Quaternion.identity;
            // ===== Check directly below =====
            Vector3 snappedWorldPos = bombAimEffect.transform.position;
            bool hasBlock1Below = Physics.CheckBox(snappedWorldPos + Vector3.down * 1f, Vector3.one * 0.45f, Quaternion.identity, ~0, QueryTriggerInteraction.Ignore);
            bool hasBlock2Below = Physics.CheckBox(snappedWorldPos + Vector3.down * 2f, Vector3.one * 0.45f, Quaternion.identity, ~0, QueryTriggerInteraction.Ignore);

            // Snap to lowest empty spot (-1 or -2)
            if (!hasBlock1Below)
            {
                if (!hasBlock2Below)
                {
                    bombAimEffect.transform.position = snappedWorldPos + Vector3.down * 2f;
                }
                else
                {
                    bombAimEffect.transform.position = snappedWorldPos + Vector3.down * 1f;
                }
            }


            Vector3 start = transform.position;
            Vector3 end = bombAimEffect.transform.position;

            Vector3 mid = (start + end) / 2f;
            mid.y += 2f; // raise midpoint for arc

            // Use 5 points for a smoother arc
            lineRenderer.positionCount = 5;

            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, Vector3.Lerp(start, mid, 0.33f));
            lineRenderer.SetPosition(2, mid);
            lineRenderer.SetPosition(3, Vector3.Lerp(mid, end, 0.66f));
            lineRenderer.SetPosition(4, end);
        }
    }

    private void Move()
    {
        rigidbody.linearVelocity = rotation * new Vector3(leftJoystickX * moveSpeed, rigidbody.linearVelocity.y, leftJoystickY * moveSpeed);
        if (!woodAimEffect.activeInHierarchy && !rockAimEffect.activeInHierarchy && !bombAimEffect.activeInHierarchy && !isShooting) //add bomb & other aimEffects if needed
        {
            Quaternion lookRot = Quaternion.LookRotation(leftLookDir);
            transform.rotation = lookRot;
        }
    }

    void AutoJumpIfNeeded()
    {

        if (transform.position.y <= 2)
        {
            Vector3 pos = transform.position;
            Vector3 forward = transform.forward.normalized;

            Vector3 checkForward = pos + forward * .4f;
            Vector3 checkAbove = checkForward + Vector3.up;
            Vector3 checkAbovePlayer = transform.position + Vector3.up;

            bool isBlockedAhead = Physics.CheckBox(checkForward, Vector3.one * 0.05f, Quaternion.identity, ~0, QueryTriggerInteraction.Ignore);
            bool isBlockAhead = Physics.CheckBox(checkForward, Vector3.one * .25f, Quaternion.identity, ~0, QueryTriggerInteraction.Ignore);
            bool isClearAbove = !Physics.CheckBox(checkAbove, Vector3.one * 0.05f, Quaternion.identity, ~0, QueryTriggerInteraction.Ignore);
            bool isClearAbovePlayer = !Physics.CheckBox(checkAbovePlayer, Vector3.one * 0.05f, Quaternion.identity, ~0, QueryTriggerInteraction.Ignore);

            //WOOD COLLECTION ---
            if (isBlockAhead)
            {
                Collider[] colliders = Physics.OverlapBox(checkForward, Vector3.one * .25f, Quaternion.identity, ~0, QueryTriggerInteraction.Ignore);
                foreach (var col in colliders)
                {
                    if (col.GetComponent<Wood>() != null)
                    {
                        col.GetComponent<Wood>().DestroyWood();
                        break; // no need to check more colliders
                    }
                }
            }

            if (isBlockedAhead && isClearAbove && isClearAbovePlayer)
            {
                gameObject.transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            }
        }
    }

    private void Stop()
    {
        rigidbody.linearVelocity = new Vector3(0, rigidbody.linearVelocity.y, 0);
    }

    //BLOCK BUILDING
    //public void buildBlock()
    //{
    //    if (blockButton.interactable == true)
    //    {
    //        isPressingButton = true;
    //        StartCoroutine(ResettingButton(1f));
    //        if (!Physics.CheckBox(CalculateSpawnPos(), Vector3.one * 0.2f, Quaternion.identity, obstacleLayer) && transform.position.y < maxPlayerHeight)
    //        {
    //            int blockIndex = GetCurrentMaterial();
 
    //            if (blockIndex == 0)
    //            {
    //                gameObject.transform.position = new Vector3(transform.position.x, transform.position.y + 1.05f, transform.position.z);
    //                Instantiate(groundImpactVFX, blockSpawnPos + Vector3.up, Quaternion.Euler(90f, 0f, 0f));
    //                Instantiate(woodBlockPrefab, blockSpawnPos, Quaternion.identity);
    //                //inventory.UpdateBlockText();//pass item number
    //                audioSource.PlayOneShot(popSound2);
    //            }
    //            else
    //            {
    //                if (inventory.currentMaterialAmount > 0)
    //                {
    //                    gameObject.transform.position = new Vector3(transform.position.x, transform.position.y + 1.05f, transform.position.z);
    //                    if(blockIndex == 1)
    //                    {
    //                        Instantiate(groundImpactVFX, blockSpawnPos+ Vector3.up, Quaternion.Euler(90f, 0f, 0f));
    //                        Instantiate(rockBlockPrefab, blockSpawnPos, Quaternion.identity);
    //                    }
    //                    if (blockIndex == 2)
    //                    {
    //                        Instantiate(groundImpactVFX, blockSpawnPos + Vector3.up, Quaternion.Euler(90f, 0f, 0f));
    //                        Instantiate(bombBlockPrefab, blockSpawnPos, Quaternion.identity);
    //                    }

    //                    inventory.itemsAmounts[blockIndex]--;
    //                    inventory.currentMaterialAmount = inventory.itemsAmounts[blockIndex];
    //                    inventory.UpdateBlockText(blockIndex);
    //                    audioSource.PlayOneShot(popSound2);
    //                }
    //                //else { inventory.SwwitchToWood(); }
    //            }
    //        }

    //    }
    //}

    private int GetCurrentMaterial()
    {
        if (inventory.woodButton.transform.localScale == new Vector3(1.3f, 1.3f, 1f))
        {
            return 0;
        }
        else if (inventory.rockButton.transform.localScale == new Vector3(1.3f, 1.3f, 1f))
        {
            return 1;
        }
        else if (inventory.bombButton.transform.localScale == new Vector3(1.3f, 1.3f, 1f))
        {
            return 2;
        }
        else { return 0; }
    }

    public IEnumerator ResettingButton(float resetTime)
    {
        yield return new WaitForSeconds(resetTime);
        isPressingButton = false;
    }

    private Vector3 CalculateSpawnPos()
    {
        blockSpawnPos = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
        if(blockSpawnPos.y > 0) { blockSpawnPos.y = 0; }

        if (Physics.CheckBox(blockSpawnPos, Vector3.one * 0.2f, Quaternion.identity, obstacleLayer) || (transform.position.y >= maxPlayerHeight)
            || Physics.CheckBox(transform.position + Vector3.up * 1f, Vector3.one * 0.1f, Quaternion.identity, obstacleLayer))
        {
            blockButton.interactable = false;
        }
        else
        {
            blockButton.interactable = true;
        }
        return blockSpawnPos;
    }

    //BULLETSHOOTING
    private void Attack(Vector3 lastLookDir)
    {
        int blockIndex = GetCurrentMaterial();

        if (!isPressingButton && !isAttacking && !isShooting)
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
        if (isShooting) return;

        if (inventory.itemsAmounts[blockIndex] > 0)
        {
            if (blockIndex == 0)
            {
                inventory.itemsAmounts[0]--;
                inventory.currentMaterialAmount = inventory.itemsAmounts[0];
                inventory.UpdateBlockText(0);
                StartCoroutine(WoodShooting());
            }
            else if (blockIndex == 1)
            {
                Quaternion baseRot = transform.rotation;
                Instantiate(rockBulletPrefab, transform.position, baseRot);
                Instantiate(rockBulletPrefab, transform.position, baseRot * Quaternion.Euler(0, -30f, 0));
                Instantiate(rockBulletPrefab, transform.position, baseRot * Quaternion.Euler(0, 30f, 0));
                inventory.itemsAmounts[1]--;
                inventory.currentMaterialAmount = inventory.itemsAmounts[1];
                inventory.UpdateBlockText(1);
            }
            else if (blockIndex == 2)
            {
                Instantiate(bombBulletPrefab, transform.position, transform.rotation);
                inventory.itemsAmounts[2]--;
                inventory.currentMaterialAmount = inventory.itemsAmounts[2];
                inventory.UpdateBlockText(2);
            }

            //  NEW: if this shot consumed the last one, auto-switch NOW
            if (inventory.itemsAmounts[blockIndex] == 0)
            {
                AutoSwitchFrom(blockIndex);
            }
        }
        else
        {
            // nothing of this type, try auto-switch immediately
            AutoSwitchFrom(blockIndex);
        }
    }

    private void AutoSwitchFrom(int fromIndex)
    {
        // simple priority order: wood -> rock -> bomb
        if (inventory.itemsAmounts[0] > 0) inventory.SwitchToWood();
        else if (inventory.itemsAmounts[1] > 0) inventory.SwitchToRock();
        else if (inventory.itemsAmounts[2] > 0) inventory.SwitchToBomb();
        else
        {
            // completely empty: hide previews
            inventory.bulletPreviews[0].SetActive(false);
            inventory.bulletPreviews[1].SetActive(false);
            inventory.bulletPreviews[2].SetActive(false);
            inventory.currentMaterialAmount = 0;
        }
    }

    private IEnumerator WoodShooting()
    {
        isShooting = true;

        Instantiate(woodBulletPrefab, transform.position, transform.rotation);
        yield return new WaitForSeconds(.2f);
        Instantiate(woodBulletPrefab, transform.position, transform.rotation);
        yield return new WaitForSeconds(.2f);
        Instantiate(woodBulletPrefab, transform.position, transform.rotation);
        yield return new WaitForSeconds(.2f);
        Instantiate(woodBulletPrefab, transform.position, transform.rotation);

        isShooting = false;
    }

    public GameObject FindNearestInteractable()
    {
        int blockIndex = GetCurrentMaterial();
        float searchRadius = 4f;
        if (blockIndex == 0) searchRadius = 5f;
        if (blockIndex == 1) searchRadius = 4.2f;

        float heightTolerance = 0.5f;

        GameObject nearestEnemy = null;
        float minEnemyDist = searchRadius;

        GameObject nearestInteractable = null;
        float minInteractDist = searchRadius;

        Collider[] colliders = Physics.OverlapSphere(transform.position, searchRadius);
        foreach (Collider col in colliders)
        {
            GameObject obj = col.gameObject;
            if (!(obj.CompareTag("Enemy") || obj.CompareTag("Interactable"))) continue;

            if (Mathf.Abs(obj.transform.position.y - transform.position.y) >= heightTolerance) continue;

            if (obj.GetComponent<Wood>()) continue;

            float distance = Vector3.Distance(transform.position, obj.transform.position);

            if (obj.CompareTag("Enemy"))
            {
                if (distance < minEnemyDist)
                {
                    minEnemyDist = distance;
                    nearestEnemy = obj;
                }
            }
            else // Interactable
            {
                if (distance < minInteractDist)
                {
                    minInteractDist = distance;
                    nearestInteractable = obj;
                }
            }
        }

        // Prioritize enemies if any are in range
        return nearestEnemy != null ? nearestEnemy : nearestInteractable;
    }

    IEnumerator ResetCoolDown()
    {
        yield return new WaitForSeconds(.3f);
        isAttacking = false;
    }


}
