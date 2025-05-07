
using UnityEngine;

public class DisableOutOfView : MonoBehaviour
{
    private Behaviour[] componentsToDisable;

    void Start()
    {
        // Add any components you want to disable when invisible
        componentsToDisable = GetComponents<Behaviour>();
    }

    void OnBecameInvisible()
    {
        foreach (var comp in componentsToDisable)
        {
            if (comp != this) comp.enabled = false;
        }
    }

    void OnBecameVisible()
    {
        foreach (var comp in componentsToDisable)
        {
            if (comp != this) comp.enabled = true;
        }
    }
}