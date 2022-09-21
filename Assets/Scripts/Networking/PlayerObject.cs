using UnityEngine;
using Fusion;


public class PlayerObject : NetworkBehaviour
{
	public static PlayerObject Local { get; private set; }

	[Networked]
	public PlayerRef Ref { get; set; }
	[Networked]
	public byte Index { get; set; }

	public byte ColorIndex { get; set; }

	//public Color GetColor => GameManager.rm.playerColours[ColorIndex];

	[field: SerializeField] public SphereCollider KillRadiusTrigger { get; private set; }


	public void Server_Init(PlayerRef pRef, byte index, byte color)
	{
		Debug.Assert(Runner.IsServer);

		Ref = pRef;
		Index = index;
		ColorIndex = color;
	}

	public override void Spawned()
	{
		base.Spawned();

		if (Object.HasStateAuthority)
		{
			PlayerRegistry.Server_Add(Runner, Object.InputAuthority, this);
		}

		if (Object.HasInputAuthority)
		{
			Local = this;
			Rpc_SetNickname(PlayerPrefs.GetString("nickname"));
		}
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	void Rpc_SetNickname(string nick)
	{
		//Nickname = nick;
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	public void Rpc_SetColor(byte c)
	{
		if (PlayerRegistry.IsColorAvailable(c))
			ColorIndex = c;
	}

}
