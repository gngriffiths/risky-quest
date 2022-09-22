using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ResourcesManager : MonoBehaviour
{
    public Color[] PlayerColours = new Color[12];									// NOTE: Max 12 colours. Would need to change this value for more colours.

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
