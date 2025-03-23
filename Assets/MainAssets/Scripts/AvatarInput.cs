using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AvatarInput : MonoBehaviour
{
    [SerializeField] private float hitsCoolDownTime = .8f;
    
    public GameObject target = null;
    public Button hammer;
    public Button pickaxe;

    private GameObject stackedCube;
    private GameObject cubeProjection;

    public bool blockIsStacked;
    private bool isDigging = false;
    public GameObject cubePrefab;

    private Vector3 spawnPos;
    private float holdTime = 0;
    private RectTransform handlePos;
    private Vector2 startPos;
    private Vector2 posAfterDrag;
    private bool isMoving = false;
    private FloatingJoystick joystick;
    private void Awake()
    {
        joystick = FindFirstObjectByType<FloatingJoystick>();
    }

    private void Start()
    {
        GameObject background = joystick.transform.GetChild(0).gameObject;
        GameObject handle = background.transform.GetChild(0).gameObject;
        handlePos = handle.GetComponent<RectTransform>();
        Transform stackedCubeTransform = transform.Find("Cube");
        stackedCube = stackedCubeTransform.gameObject;
        Transform cubeProjectionTransform = transform.Find("Projection");
        cubeProjection = cubeProjectionTransform.gameObject;
    }

    void Update()
    {
        DetectInput();
        if(!blockIsStacked)
        {
            CheckSurroundings();
            hammer.interactable = false;
        }
        else
        {
            stackedCube.SetActive(true);
            cubeProjection.SetActive(true);
            hammer.interactable = true;
        }
        if(target != null && target.GetComponent<Outline>().enabled)
        {
            pickaxe.interactable = true;
        }
        else { pickaxe.interactable = false; }
    }


    private void DetectInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startPos = touch.position;
                    break;

                case TouchPhase.Stationary:
                    CountHoldTime();

                    break;

                case TouchPhase.Moved:
                    CountHoldTime();
                    posAfterDrag = touch.position;

                    if (handlePos.anchoredPosition != Vector2.zero)
                    {
                        isMoving = true;
                        isDigging = false;
                    }
                    break;

                case TouchPhase.Ended:


                    isMoving = false;
                    isDigging = false;
                    holdTime = 0;
                    break;
            }
        }
    }

    public void BuildBlock()
    {
        if (!isMoving && blockIsStacked && holdTime < 0.3f)
        {
            blockIsStacked = false;
            stackedCube.SetActive(false);
            cubeProjection.SetActive(false);
            gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 50, ForceMode.Impulse);
            spawnPos = new Vector3(cubeProjection.transform.position.x, 0, cubeProjection.transform.position.z);
            StartCoroutine(DelayedSpawn());
        }
    }

    private IEnumerator DelayedSpawn()
    {
        yield return new WaitForSeconds(.3f);
        Instantiate(cubePrefab, spawnPos, Quaternion.identity);
    }

    public void Dig()
    {
        StartCoroutine(Digging());
    }

    private IEnumerator Digging()
    {
        while (!isMoving && !blockIsStacked && !isDigging)
        {
            Cube cube = target.GetComponent<Cube>();
            cube.GetHit();
            isDigging = true;
            yield return new WaitForSeconds(hitsCoolDownTime);
            isDigging = false;
        }
    }

    private void CountHoldTime()
    {
        holdTime += Time.deltaTime;
    }

    private void CheckSurroundings()
    {
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.tag == "Interactable")
            {
                Vector3 directionToTarget = hitCollider.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    target = hitCollider.gameObject;
                }
            }
        }
    }

    void LateUpdate()
    {
        if (cubeProjection.activeInHierarchy)
        {
            // Get the child's current position
            Vector3 childPos = cubeProjection.transform.position;

            // Snap X and Z to the nearest integer, keep Y the same
            cubeProjection.transform.position = new Vector3(
                Mathf.Round(transform.position.x),
                childPos.y,
                Mathf.Round(transform.position.z)
            );
            cubeProjection.transform.rotation = Quaternion.identity;
        }
    }


}
