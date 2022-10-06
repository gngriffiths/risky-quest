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
	public static CombatManager combatManager { get; private set; }


	private Dictionary<int, int> playerObjectiveCount;


	[Networked]
	public float gameTime { get; set; }
	private float turnTime = 2; 
	private float tracker_turnTime; 

	[Networked]
	public int winningFaction { get; set; }


	public NetworkDebugStart starter;

	public UIScreen pauseScreen;
	public UIScreen optionsScreen;

	public Transform parent_spawnPoints;
	public Transform parent_unitPool;

	public Transform GetUnitPool()
	{
		if (parent_unitPool) { return parent_unitPool; }

		return null;
	
	}


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            rm = GetComponent<ResourcesManager>();
            im = GetComponent<InterfaceManager>();
			State = GetComponent<GameState>();
			combatManager = GetComponent<CombatManager>();

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

    public override void FixedUpdateNetwork()
    {
		
		if (Runner.IsServer)
		{
			if (State.Current == EGameState.Play)
			{
				TrackTurnTime();
				gameTime += Time.deltaTime;
			}
		}

		// base.FixedUpdateNetwork();

		//if (Runner.IsForward && Runner.Simulation.Tick % 100 == 0)
		//	im.gameUI.pingText.text = $"Ping: {1000 * Runner.GetPlayerRtt(Runner.LocalPlayer):N0}ms";
	}

	public void TrackTurnTime()
	{
		tracker_turnTime += Time.deltaTime;

		if (tracker_turnTime >= turnTime)
		{
			tracker_turnTime = 0;
			float atkRoll = Random.Range(0, 10.0f);
			float defRoll = Random.Range(0, 10.0f);
			RPC_ConcludeCombatTurn(atkRoll,defRoll);
		}

	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public void RPC_ConcludeCombatTurn(float _atkRoll, float _defRoll)
	{
		combatManager.ConcludeCombatTurn(_atkRoll, _defRoll);

	}


		public void Server_StartGame()
	{
		if (Runner.IsServer == false)
		{
			Debug.LogWarning("This method is server-only");
			return;
		}

		if (State.Current != EGameState.Pregame) return;

		gameStarted = true;

		State.Server_SetState(EGameState.Play);


	}









	public bool gameStarted = false;



	public static void QuitGame()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}



	public List<ControlPoint> objectives;
	public int winCondition; // number of control points to win

	public void CheckForVictory(ControlPoint _obj)
	{
		if (PlayerRegistry.Count < 2) {return;}
		if (State.Current != EGameState.Play) {return;}



		if (GetObjectives().Contains(_obj) == false)
		{ GetObjectives().Add(_obj); }

		PlayerObjectiveTracker().Clear();
		winningFaction = -1;

		foreach (ControlPoint el in GetObjectives())
		{

			if (winningFaction == -1)
			{
				winningFaction = el.controllerFaction;
			}

			if (PlayerObjectiveTracker().ContainsKey(el.controllerFaction))
			{

				PlayerObjectiveTracker()[el.controllerFaction] = PlayerObjectiveTracker()[el.controllerFaction] + 1;
			}
			else
			{
				PlayerObjectiveTracker().Add(el.controllerFaction, 1);
			}

			if (PlayerObjectiveTracker()[winningFaction] < PlayerObjectiveTracker()[el.controllerFaction])
			{
				winningFaction = el.controllerFaction;
			}
		}


		//check that the controller 'player' isnt neutral [e.g. no players control it]
		if (winningFaction == -1) { return; }

		if (PlayerObjectiveTracker()[winningFaction] >= winCondition)
		{ Debug.Log(winningFaction + " Wins the Game!"); }
		else { Debug.Log(winningFaction + " is in the Lead"); }

	}

	public List<ControlPoint> GetObjectives()
	{

		if (objectives == null || objectives.Count == 0)
		{
			objectives = new List<ControlPoint>();

			foreach (ControlPoint el in FindObjectsOfType<ControlPoint>())
			{
				objectives.Add(el);
			}

		}


		return objectives;
	}

	public Dictionary<int, int> PlayerObjectiveTracker()
	{
		if (playerObjectiveCount == null)
		{
			playerObjectiveCount = new Dictionary<int, int>();

		}


		return playerObjectiveCount;
	}














	void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner)
	{
		UIScreen.CloseAll();
	}

    #region INetworkRunnerCallbacks
    void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, Fusion.Sockets.NetAddress remoteAddress, Fusion.Sockets.NetConnectFailedReason reason) { }
	void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
	void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner) { }
	void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player) { Debug.Log("OnPlayerJoined"); }
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
