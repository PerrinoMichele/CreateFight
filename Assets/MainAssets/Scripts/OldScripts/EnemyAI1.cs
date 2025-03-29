using UnityEngine;
using UnityEngine.AI;

public class EnemyAI1 : MonoBehaviour
{
    public Transform player;
    public NavMeshAgent agent;
    private void Start()
    {
        player = FindFirstObjectByType<InputPlayer>().transform;
        agent = gameObject.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(player.position);
    }
}
