using UnityEngine;
using UnityEngine.AI;

public class VehicleVFX : MonoBehaviour
{
    //[SerializeField] bool shoot;      // Testing

    [SerializeField] ParticleSystem dustParticles = null;
    //[SerializeField] ParticleSystem[] gunParticles = null;

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
        if (navMeshAgent && OnNavMesh())
        {
            if (navMeshAgent.remainingDistance > 5)
                VehicleMoving(true);
            else
                VehicleMoving(false);
        }
        

        //// if shoot is true, Shoot true
        //if (shoot)
        //    Shoot(true);
        //else
        //    Shoot(false);
    }

    void VehicleMoving(bool moving) 
    {
        if (moving && dustParticles.isStopped)
        {
            dustParticles.Play();
            //Debug.Log("Play");
        }
        else if (!moving && dustParticles.isPlaying)
        {
            dustParticles.Stop();
            //Debug.Log("Stop");
        }

    }

    public bool OnNavMesh()
    {


      
            NavMeshHit hit;

            if (NavMesh.SamplePosition(transform.position, out hit, 1f, NavMesh.AllAreas))
            {
                if (NavMesh.SamplePosition(navMeshAgentCached.destination, out hit, 1f, NavMesh.AllAreas))
                {
                    return true;
                }
            }
        return false;

        


    }

    //public void Shoot(bool shooting)
    //{
    //    if(shooting)
    //        foreach(var particle in gunParticles)            
    //            particle.Play();
    //    else
    //        foreach (var particle in gunParticles)
    //            particle.Stop();
    //}
}
