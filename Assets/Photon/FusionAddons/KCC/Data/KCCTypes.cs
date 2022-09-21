namespace Fusion.KCC
{
	using System;

	/// <summary>
	/// Controls mode in which KCC operates. Use <c>Fusion</c> for fully networked characters, <c>Unity</c> can be used for non-networked local characters (NPCs, cinematics, ...).
	/// </summary>
	public enum EKCCDriver
	{
		None   = 0,
		Unity  = 1,
		Fusion = 2,
	}

	/// <summary>
	/// Defines KCC render behavior for input/state authority.
    /// <list type="bullet">
    /// <item><description>None - Skips render completely. Useful when render update is perfectly synchronized with fixed update or debugging.</description></item>
    /// <item><description>Predict - Full processing and physics query.</description></item>
    /// <item><description>Interpolate - Interpolation between last two fixed updates.</description></item>
    /// </list>
	/// </summary>
	public enum EKCCRenderBehavior
	{
		None        = 0,
		Predict     = 1,
		Interpolate = 2,
	}

	/// <summary>
	/// Defines KCC physics behavior.
    /// <list type="bullet">
    /// <item><description>None - Skips almost all execution including processors, collider is despawned.</description></item>
    /// <item><description>Capsule - Full processing with capsule collider spawned.</description></item>
    /// <item><description>Void - Skips internal physics query, collider is despawned, processors are executed.</description></item>
    /// </list>
	/// </summary>
	public enum EKCCShape
	{
		None    = 0,
		Capsule = 1,
		Void    = 2,
	}

	public enum EKCCStage
	{
		None                  = 0,
		SetInputProperties    = 1,
		SetDynamicVelocity    = 2,
		SetKinematicDirection = 3,
		SetKinematicTangent   = 4,
		SetKinematicSpeed     = 5,
		SetKinematicVelocity  = 6,
		ProcessPhysicsQuery   = 7,
		Stay                  = 8,
		Interpolate           = 9,
	}

	[Flags]
	public enum EKCCStages
	{
		None                  = 0,
		SetInputProperties    = 1 << EKCCStage.SetInputProperties,
		SetDynamicVelocity    = 1 << EKCCStage.SetDynamicVelocity,
		SetKinematicDirection = 1 << EKCCStage.SetKinematicDirection,
		SetKinematicTangent   = 1 << EKCCStage.SetKinematicTangent,
		SetKinematicSpeed     = 1 << EKCCStage.SetKinematicSpeed,
		SetKinematicVelocity  = 1 << EKCCStage.SetKinematicVelocity,
		ProcessPhysicsQuery   = 1 << EKCCStage.ProcessPhysicsQuery,
		All                   = -1
	}

	public enum EKCCFeature
	{
		None         = 0,
		StepUp       = 1,
		SnapToGround = 2,
	}

	[Flags]
	public enum EKCCFeatures
	{
		None         = 0,
		StepUp       = 1 << EKCCFeature.StepUp,
		SnapToGround = 1 << EKCCFeature.SnapToGround,
		All          = -1
	}

	public enum EColliderType
	{
		None    = 0,
		Sphere  = 1,
		Capsule = 2,
		Box     = 3,
		Mesh    = 4,
		Terrain = 5,
	}
}
