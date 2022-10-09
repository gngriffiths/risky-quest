using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VehicleGroupManager 
{
    public static Action<int> DisbandGroup;

    public static Dictionary<int, VehicleChain> LastVehicles = new();
}
