using System.Collections.Generic;
using UnityEngine;
using Fusion;
using static GameState;

public class GameManager : NetworkBehaviour, INetworkRunnerCallbacks
{
    public static GameManager Instance { get; private set; }
	public static GameState State { get; private set; }
    public static ResourcesManager rm { get; private set; }
	public static InterfaceManager im { get; private set; }

	public NetworkDebugStart starter;

	public UIScreen pauseScreen;
	public UIScreen optionsScreen;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            rm = GetComponent<ResourcesManager>();
            im = GetComponent<InterfaceManager>();
			State = GetComponent<GameState>();
        }
        else
        {
			Destroy(gameObject);
        }
    }

	public override void Spawned()
	{
		base.Spawned();

		if (Runner.IsServer)
		{
			State.Server_SetState(EGameState.Pregame);
		}

		Runner.AddCallbacks(this);
	}

	public override void Despawned(NetworkRunner runner, bool hasState)
	{
		base.Despawned(runner, hasState);
		runner.RemoveCallbacks(this);
		starter.Shutdown();
	}

	//public override void FixedUpdateNetwork()
	//{
	//	base.FixedUpdateNetwork();

	//	//if (Runner.IsForward && Runner.Simulation.Tick % 100 == 0)
	//	//	im.gameUI.pingText.text = $"Ping: {1000 * Runner.GetPlayerRtt(Runner.LocalPlayer):N0}ms";
	//}

	public void Server_StartGame()
	{
		if (Runner.IsServer == false)
		{
			Debug.LogWarning("This method is server-only");
			return;
		}

		if (State.Current != EGameState.Pregame) return;


		State.Server_SetState(EGameState.Play);
	}

	public static void QuitGame()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}	

	
	void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner)
	{
		UIScreen.CloseAll();
	}

    #region INetworkRunnerCallbacks
    void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, Fusion.Sockets.NetAddress remoteAddress, Fusion.Sockets.NetConnectFailedReason reason) { }
	void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
	void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner) { }
	void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
	void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
	void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input) { }
	void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
	void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
	void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
	void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
	void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
	void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, System.ArraySegment<byte> data) { }
	void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner) { }
	void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner) { }
	void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    #endregion
}
