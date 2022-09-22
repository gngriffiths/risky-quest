using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class GameState : NetworkBehaviour
{
	public enum EGameState { Off, Pregame, Play, GameOver }

	[Networked] public EGameState Previous { get; set; }
	[Networked] public EGameState Current { get; set; }

	[Networked] TickTimer Delay { get; set; }
	[Networked] EGameState DelayedState { get; set; }

	protected StateMachine<EGameState> StateMachine = new StateMachine<EGameState>();

	public override void Spawned()
	{
		StateMachine[EGameState.Off].onExit = newState =>
		{
			Debug.Log($"Exited {EGameState.Off} to {newState}");

			if (Runner.IsServer) { }

			if (Runner.IsPlayer) // [PLAYER] Off -> *
			{
				GameManager.im.gameUI.InitPregame(Runner);
			}
		};

		StateMachine[EGameState.Pregame].onEnter = state =>
		{
			Debug.Log($"Entered {EGameState.Pregame} from {state}");

			if (Runner.IsServer) // [SERVER] * -> Pregame
			{
				PlayerRegistry.ForEach(pObj =>
				{
                    // Reset player
					//pObj.Controller.IsDead = false;
					//pObj.Controller.IsSuspect = false;
					//pObj.Controller.cc.SetPosition(Vector3.zero);
					//pObj.Controller.EndInteraction();
					//pObj.Controller.Server_UpdateDeadState();
				});

				GameManager.rm.Purge();
			}

			if (Runner.IsPlayer) // [PLAYER] * -> Pregame
			{
				GameManager.im.gameUI.InitPregame(Runner);
				//GameManager.im.gameUI.colorUI.Init();
			}
		};

		StateMachine[EGameState.Play].onEnter = state =>
		{
			Debug.Log($"Entered {EGameState.Play} from {state}");

			if (Runner.IsServer) // [SERVER] * -> Play
			{
				// Set players' starting positions
				//PlayerRegistry.ForEach(
				//	obj => obj.Controller.cc.SetPosition(GameManager.Instance.mapData.GetSpawnPosition(obj.Index)));


				if (state == EGameState.Pregame) // [SERVER] Pregame -> Play
				{

				}
			}
		};

		StateMachine[EGameState.GameOver].onExit = state => GameManager.im.gameUI.CloseOverlay();

		StateMachine[EGameState.GameOver].onEnter = state =>
		{
			Debug.Log($"Entered {EGameState.GameOver} from {state}");

			if (Runner.IsServer) // [SERVER] * -> ImpostorWin
			{
				Server_DelaySetState(EGameState.Pregame, 3);
			}

			if (Runner.IsPlayer) // [PLAYER] * -> ImpostorWin
			{

			}
		};
	}

	public override void FixedUpdateNetwork()
	{
		if (Runner.IsServer)
		{
			if (Delay.Expired(Runner))
			{
				Delay = TickTimer.None;
				Server_SetState(DelayedState);
			}
		}

		if (Runner.IsForward)
			StateMachine.Update(Current, Previous);
	}

	public void Server_SetState(EGameState st)
	{
		if (Current == st) return;
		Previous = Current;
		Current = st;
	}
	
	public void Server_DelaySetState(EGameState newState, float delay)
	{
		Debug.Log($"Delay state change to {newState} for {delay}s");
		Delay = TickTimer.CreateFromSeconds(Runner, delay);
		DelayedState = newState;
	}
}