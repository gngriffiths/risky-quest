using UnityEngine;
using UnityEngine.AI;

public class Car1VFX : MonoBehaviour
{
    [SerializeField] bool attacking;

    [SerializeField] ParticleSystem[] gunParticles = null;

    //NavMeshAgent navMeshAgent
    //{
    //    get
    //    {
    //        if (navMeshAgentCached == null)
    //            navMeshAgentCached = GetComponentInParent<NavMeshAgent>();
    //        return navMeshAgentCached;
    //    }
    //}

    //NavMeshAgent navMeshAgentCached;

    private void Update()
    {
        if (attacking)
            Attacking(true);
        else
            Attacking(false);

        attacking = false;
    }

    void Attacking(bool moving)
    {
        if (moving && gunParticles[0].isStopped)
            foreach (var particle in gunParticles)
                particle.Play();
        else if (!moving && gunParticles[0].isPlaying)
            foreach (var particle in gunParticles)
                particle.Stop();
    }


    //private void Update()
    //{
    //    if (navMeshAgent.velocity.sqrMagnitude > 1)
    //        VehicleMoving(true);
    //    else
    //        VehicleMoving(false);
    //}

    //void VehicleMoving(bool moving)
    //{
    //    if (moving && jetParticles[0].isStopped)
    //        foreach (var particle in jetParticles)
    //            particle.Play();
    //    else if (!moving && jetParticles[0].isPlaying)
    //        foreach (var particle in jetParticles)
    //            particle.Stop();
    //}
}