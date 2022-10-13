using System.Collections.Generic;
using UnityEngine;
using Fusion;
using static GameState;
using TMPro;

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

	private float timestamp_lastBonus;
	private float timestamp_lastCombat;

	public float tracker_turnTime; 

	[Networked]
	public int winningFaction { get; set; }
    

	public NetworkDebugStart starter;

	public UIScreen pauseScreen;
	public UIScreen optionsScreen;
    public UI_Oly uiOly;

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

	public void ShowPlayerCount()
	{
		if (debug_scoreDisplay)
		{
			debug_scoreDisplay.text = PlayerRegistry.Count.ToString() + " Players Connected";
		}
		else
		{
			if (GameObject.Find("debugtext_Objective"))
			{ debug_scoreDisplay = GameObject.Find("debugtext_Objective").GetComponent<TextMeshPro>(); }
			
		}

	}








	public void Update()
	{

		if (Runner && Runner.IsServer)
		{
			if (State.Current == EGameState.Play)
			{
				//TrackTurnTime();
			}
		}

		
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

		if (tracker_turnTime - timestamp_lastCombat >= GameConstants.TURNLENGTH_COMBAT)
		{
			timestamp_lastCombat = tracker_turnTime;

			float atkRoll = Random.Range(0, 10.0f);
			float defRoll = Random.Range(0, 10.0f);
			RPC_ConcludeCombatTurn(atkRoll,defRoll);
		}
		 if (tracker_turnTime - timestamp_lastBonus >= GameConstants.TURNLENGTH_BONUS)
		{
			timestamp_lastBonus = tracker_turnTime;


			ConcludeBonusTurn();
		}

	}

	public void ResetCombatTurnTime()
	{
		//if no combats are in the list the timer could be at any value, reset it so the new combat takes the default time
		timestamp_lastCombat = tracker_turnTime;
	}


	[Rpc(RpcSources.All, RpcTargets.All)]
	public void RPC_ConcludeCombatTurn(float _atkRoll, float _defRoll)
	{
		//the combat manager checks the oldest active combat and resolves a round using these values as the roll
		combatManager.ConcludeCombatTurn(_atkRoll, _defRoll);

	}









	public void ConcludeBonusTurn()
	{
		//

		foreach (ControlPoint el in GetObjectives())
		{

			if (el.controllerFaction != -1)
			{

				PlayerControl controllingPlayer = GetPlayer(el.controllerFaction);

				if (controllingPlayer)
				{
					controllingPlayer.RPC_AddUnit(el.transform.position, controllingPlayer.faction, 1);


					//if (el.bonus == bonus_type.new_unit)
					//{


					//}
					//else if(el.bonus == bonus_type.power)
					//{


					//	foreach (Unit unitOnPoint in el.GetUnits())
					//	{
					//		if (unitOnPoint.faction == controllingPlayer.faction)
					//		{
					//			unitOnPoint.UpdateCount((int)el.bonusCombatStrength);

					//			controllingPlayer.RPC_UpdateUnitStrength(controllingPlayer.faction, unitOnPoint.id, unitOnPoint.Count());
								
					//		}
					//	}
					//}


				}

			}

		
		}

	}



	public void Server_StartGame()
	{
		if (Runner.IsServer == false)
		{
			Debug.LogWarning("This method is server-only");
			return;
		}

		//if (State.Current == EGameState.GameOver)
		//{
		//	RestartGame();
		//	return;
		//}
		if (State.Current != EGameState.Pregame) return;


		State.Server_SetState(EGameState.Play);


	}

	public void Server_EndGame()
	{
		if (Runner.IsServer == false)
		{
			Debug.LogWarning("This method is server-only");
			return;
		}

		if (State.Current != EGameState.Play) return;

		Debug.LogWarning("Server_EndGame");

		State.Server_SetState(EGameState.GameOver);


	}


	public void RestartGame()
	{
		gameTime = 0;
		timestamp_lastBonus = 0;
		timestamp_lastCombat = 0;

		foreach (ControlPoint el in GetObjectives())
		{
			el.ResetToStart();
		}

			//State.Server_SetState(EGameState.Play);
	}






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

		DisplayScore();
		//check that the controller 'player' isnt neutral [e.g. no players control it]
		if (winningFaction == -1) { return; }

	


		if (PlayerObjectiveTracker()[winningFaction] >= winCondition)
		{ 
			Debug.Log(winningFaction + " Wins the Game!");
			Server_EndGame();
		}
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


	public PlayerControl GetPlayer(int _faction)
	{

		PlayerControl newPlayer = null;

		PlayerRegistry.ForEach(pObj =>
		{
			if (pObj.Index == _faction)
			{
				newPlayer = pObj.GetComponent<PlayerControl>();
			}


		});

		return newPlayer;
	}



	public TextMeshPro debug_scoreDisplay;

	public void DisplayScore()
	{
	
		if (debug_scoreDisplay)
		{

			string winningText = "";

			PlayerRegistry.ForEach(pObj =>
			{
				if (PlayerObjectiveTracker().ContainsKey(pObj.Index))
				{
					winningText += "Player " + pObj.Index + " controls " + PlayerObjectiveTracker()[pObj.Index] + " bases. \n";
				}

			});

			debug_scoreDisplay.text = winningText;
			if(uiOly != null)
				uiOly.Scores(winningText);


        }
		else
		{
			if (GameObject.Find("debugtext_Objective"))
			{ debug_scoreDisplay = GameObject.Find("debugtext_Objective").GetComponent<TextMeshPro>(); }
		}


	}











	void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner)
	{
		UIScreen.CloseAll();
	}

    #region INetworkRunnerCallbacks
    void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, Fusion.Sockets.NetAddress remoteAddress, Fusion.Sockets.NetConnectFailedReason reason) { }
	void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
	void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner) { }
	void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player) 
	{ 
		Debug.Log("OnPlayerJoined");
		ShowPlayerCount();
	}
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
