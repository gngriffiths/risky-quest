using UnityEngine;
using Fusion;

// Bridge between UI and Photon.

public class NetworkStartBridge : MonoBehaviour
{
	public NetworkDebugStart networkDebugStart;
	public TMPro.TMP_InputField codeField;

	private void OnEnable()
	{
		codeField.text = networkDebugStart.DefaultRoomName;
	}

	public void SetCode(string code)
	{
		networkDebugStart.DefaultRoomName = code;
	}
	
	public void StartHost()
	{
		if (string.IsNullOrWhiteSpace(networkDebugStart.DefaultRoomName))
			networkDebugStart.DefaultRoomName = RoomCode.Create();
		networkDebugStart.StartHost();
	}

	public void StartClient()
	{
		networkDebugStart.StartClient();
	}

	public void Shutdown()
	{
		foreach (var runner in NetworkRunner.Instances)
		{
			runner.Shutdown();
		}
	}
}
