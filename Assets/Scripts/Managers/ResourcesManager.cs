using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ResourcesManager : MonoBehaviour
{
	readonly List<NetworkObject> managedObjects = new List<NetworkObject>();

	public void Manage(NetworkObject obj)
	{
		managedObjects.Add(obj);
	}

	public void Purge()
	{
		foreach (var obj in managedObjects)
		{
			if (obj) GameManager.Instance.Runner.Despawn(obj);
		}
		managedObjects.Clear();
	}
}
