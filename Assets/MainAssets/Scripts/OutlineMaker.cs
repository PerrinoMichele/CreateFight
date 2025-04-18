using UnityEngine;
using UnityEngine.UI;

public class OutlineMaker : MonoBehaviour
{
    Outline outline;
    public Material dither;

    void Start()
    {
        outline = GetComponent<Outline>();
        if (transform.position.y >= 0) { outline.enabled = true; }

        //if (transform.position.y == 1) { transform.Find("Cube").gameObject.SetActive(true); }
    }

    private void Update()
    {
        
    }
}
