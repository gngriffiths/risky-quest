using UnityEngine;
using UnityEngine.AI;

public class VehicleVFX : MonoBehaviour
{
    [SerializeField] ParticleSystem dustParticles = null;
    NavMeshAgent navMeshAgent
    {
        get
        {
            if (navMeshAgentCached == null)
                navMeshAgentCached = GetComponentInParent<NavMeshAgent>();
            return navMeshAgentCached;
        }
    }

    NavMeshAgent navMeshAgentCached;

    // If the gameobject is moving, play particle effect

    private void Update()
    {
        //if(navMeshAgent.velocity.sqrMagnitude > 0.1)
        //    VehicleMoving(true);
        //else
        //    VehicleMoving(false);

        if (navMeshAgent.remainingDistance > 2)
            VehicleMoving(true);
        else
            VehicleMoving(false);

    }
    
    void VehicleMoving(bool moving) 
    {
        if (moving && dustParticles.isStopped)
            dustParticles.Play();
        else if (!moving && dustParticles.isPlaying)
            dustParticles.Stop();

    }
}
