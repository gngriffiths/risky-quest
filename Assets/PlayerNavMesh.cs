using UnityEngine;
using UnityEngine.AI;

public class PlayerNavMesh : MonoBehaviour
{
    public Transform TransformTarget = null;

    NavMeshAgent navMeshAgent;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (TransformTarget == null)
            return;
        
        navMeshAgent.SetDestination(TransformTarget.position);
    }
}
