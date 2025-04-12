using UnityEngine;
using UnityEngine.UI;

public class OutlineMaker : MonoBehaviour
{
    Outline outline;

    void Start()
    {
        outline = GetComponent<Outline>();
        if (transform.position.y >= 0) { outline.enabled = true; }
    }


}
