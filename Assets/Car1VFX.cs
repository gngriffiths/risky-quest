using UnityEngine;
using UnityEngine.AI;

public class Car1VFX : MonoBehaviour
{
    [SerializeField] ParticleSystem[] jetParticles = null;

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


    private void Update()
    {
        if (navMeshAgent.velocity.sqrMagnitude > 1)
            VehicleMoving(true);
        else
            VehicleMoving(false);
    }

    void VehicleMoving(bool moving)
    {
        if (moving && jetParticles[0].isStopped)
            foreach (var particle in jetParticles)
                particle.Play();
        else if (!moving && jetParticles[0].isPlaying)
            foreach (var particle in jetParticles)
                particle.Stop();
    }
}