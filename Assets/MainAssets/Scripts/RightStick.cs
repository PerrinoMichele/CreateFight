using System.Collections;
using UnityEditor;
using UnityEngine;

public class RightStick : MonoBehaviour
{
    public FloatingJoystick joystick;

    private float joystickX;
    private float joystickY;
    private Vector3 lookDir;
    private GameObject hitPreview;
    private GameObject hit;

    private void Start()
    {
        Transform hitPreviewTransform = transform.Find("Aim");
        Transform hitTransform = transform.Find("Hit");
        hitPreview = hitPreviewTransform.gameObject;
        hit = hitTransform.gameObject;

    }

    void Update()
    {
        joystickX = joystick.Horizontal;
        joystickY = joystick.Vertical;
        lookDir = new Vector3(joystickX, 0f, joystickY);

        if (lookDir != Vector3.zero)
        {
            Rotate();
            hitPreview.SetActive(true);
        }

        if (Input.touchCount > 0 && hitPreview.activeInHierarchy)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Ended:
                    hit.SetActive(true);
                    StartCoroutine(DeactivateHit());
                    hitPreview.SetActive(false);
                    break;
            }
        }
    }


    private IEnumerator DeactivateHit()
    {
        yield return new WaitForSeconds(.1f);
        hit.SetActive(false);
    }

    private void Rotate()
    {
        Quaternion lookRot = Quaternion.LookRotation(lookDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 40f);
    }
}
