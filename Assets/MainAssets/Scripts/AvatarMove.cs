using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class AvatarMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5;

    public FloatingJoystick joystick;
    private float joystickX;
    private float joystickY;
    private Vector3 movementDir;
    private new Rigidbody rigidbody;
    private RightStick rightStick;

    private void Awake()
    {
        rightStick = GetComponent<RightStick>();
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (joystick.Vertical != 0 && joystick.Horizontal != 0)
        {
            DetectDirection();
        }
        else
        {
            Stop();
        }
    }

    private void DetectDirection()
    {
        joystickX = joystick.Horizontal;
        joystickY = joystick.Vertical;
        movementDir = new Vector3(joystickX, 0f, joystickY);
    }

    private void Stop()
    {
        joystickX = 0;
        joystickY = 0;
        movementDir = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        rigidbody.linearVelocity = new Vector3(0, rigidbody.linearVelocity.y, 0);
    }

    private void FixedUpdate()
    {
        Move();

        if (movementDir != Vector3.zero && !rightStick.hitEffect.activeInHierarchy)
        {
            Rotate();
        }
    }
    private void Move()
    {
        rigidbody.linearVelocity = new Vector3(joystickX * moveSpeed, rigidbody.linearVelocity.y, joystickY * moveSpeed);
    }


    private void Rotate()
    {
        if(rightStick.lookDir == Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(movementDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 40f);
        }

    }
}

