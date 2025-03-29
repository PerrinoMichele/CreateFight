using UnityEngine;
using Unity.AI.Navigation;

public class TestDELETE : MonoBehaviour
{
    public NavMeshSurface mySurfaceNavMesh;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mySurfaceNavMesh = FindFirstObjectByType<NavMeshSurface>();
        Destroy(gameObject, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        mySurfaceNavMesh.BuildNavMesh();
    }
}
