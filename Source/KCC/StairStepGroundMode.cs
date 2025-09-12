using System;

namespace KCC;

/// <summary>
/// Stair stepping mode for KCC characters.
/// Determines what type of stairstepping is allowed.
/// </summary>
public enum StairStepGroundMode
{
	/// <summary>
	/// Always stairstep.
	/// </summary>
	None = GroundFlag.None,
	/// <summary>
	/// Stairstep only when solid is hit.
	/// </summary>
	RequireSolid = GroundFlag.Solid,
	/// <summary>
	/// Stairstep only when stable solid is hit.
	/// </summary>
	RequireStableSolid = GroundFlag.Solid | GroundFlag.Stable,
	/// <summary>
	/// Stairstep only when solid with ground tag is hit.
	/// </summary>
	RequireGround = GroundFlag.Solid | GroundFlag.GroundTag,
	/// <summary>
	/// Stairstep only when stable solid with ground tag is hit.
	/// </summary>
	RequireStableGround = GroundFlag.Solid | GroundFlag.Stable | GroundFlag.GroundTag,
}