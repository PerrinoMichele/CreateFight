using UnityEngine;
using Unity.AI.Navigation;

[RequireComponent(typeof(NavMeshSurface))]

public class NavMeshHandler : MonoBehaviour
{
    private NavMeshSurface myNavMeshSurface;

    private void Awake()
    {
        myNavMeshSurface = GetComponent<NavMeshSurface>();
    }

    void Start()
    {
        myNavMeshSurface.BuildNavMesh();
    }

    void UpdateNavMesh()
    {

    }
}
