using UnityEngine;

public class VehicleChain : MonoBehaviour
{
    [SerializeField] public Transform ChainLink;            // The target for connecting vehicles to aim for (the chain link must be outside of the NavMeshAgent radius).

    float maxVehicleConnectionDistance = 10f;               // If two vehicles are within this distance, they connect straight away. Otherwise they connect when a collider hits.

    public int GroupID { get; private set; }                 // Vehicle not grouped if ID is 0.

    bool joinGroup;

    PlayerNavMesh playerNavMesh;

    private void Awake()
    {
        playerNavMesh = GetComponent<PlayerNavMesh>();
    }

    private void OnEnable()
    {
        VehicleGroupManager.DisbandGroup += DisbandGroup;       
    }

    private void OnDisable()
    {
        VehicleGroupManager.DisbandGroup -= DisbandGroup;
    }

    void MakeGroup()                                                    // Click on 1st vehicle, then click on 2nd vehicle. On 2nd vehicle MakeGroup() is called.
    {
        if (GroupID == 0)
        {
            GroupID = MakeGroupID.NewID();
            VehicleGroupManager.LastVehicles.Add(GroupID, this);
        }
    }

    void NavigateToVehicle(VehicleChain vehicle)                         // Click on 1st vehicle, then click on 2nd vehicle. On 1st vehicle FindVehicle() is called.
    {
        joinGroup = true;
        GroupID = vehicle.GroupID;
        playerNavMesh.TransformTarget = vehicle.ChainLink;

        if (Vector3.Distance(transform.position, vehicle.transform.position) > maxVehicleConnectionDistance)           // If the two vehicles are too close for the collider, connect them straight away.
        {
            ConnectVehicle();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(TryGetComponent<VehicleChain>(out var vehicleChain))
        {
            if (joinGroup && vehicleChain.GroupID == GroupID)           // And last vehicle?
            {
                ConnectVehicle();
            }
        }
    }

    // Once the vehicle reaches its target vehicle it checks to see what group the vehicle is in and then joins the end of the chain.
    void ConnectVehicle()
    {
        joinGroup = false;
        
        VehicleGroupManager.LastVehicles.TryGetValue(GroupID, out var lastVehicle);         // Get last vehicle in chain.
        if(lastVehicle != null)
        {
            VehicleGroupManager.LastVehicles[GroupID] = this;              // Set this vehicle as the last vehicle in the chain.

            playerNavMesh.TransformTarget = lastVehicle.ChainLink;
        }
    }    


    void CancelNavigateToVehicle()                  // If the player clicks on this vehicle, then on the terrain for it to move to position, cancel this vehicle from moving to the other vehicle.
    {
        // NOTE: playerNavMesh.TransformTarget is set by the move script. No need to clear here.
        joinGroup = false;
        GroupID = 0;
    }

    private void DisbandGroup(int groupID)
    {
        if (this.GroupID == groupID)
        {
            this.GroupID = 0;
            VehicleGroupManager.LastVehicles.Remove(groupID);           // If VehicleGroupManager class is changed to a Mono, place action in that class rather than on every vehicle.
            DisconnectVehicle();
        }
    }

    void DisconnectVehicle()
    {
        playerNavMesh.TransformTarget = null;
    }
}
