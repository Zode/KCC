namespace KCC;

/// <summary>
/// Stair stepping mode for KCC characters.
/// Determines what type of stairstepping is allowed.
/// </summary>
public enum StairStepGroundMode : byte
{
	/// <summary>
	/// Always stairstep.
	/// </summary>
	None,
	/// <summary>
	/// Stairstep only when solid is hit.
	/// </summary>
	RequireSolid,
	/// <summary>
	/// Stairstep only when stable solid is hit.
	/// </summary>
	RequireStableSolid,
	/// <summary>
	/// Stairstep only when solid with ground tag is hit.
	/// </summary>
	RequireGround,
	/// <summary>
	/// Stairstep only when stable solid with ground tag is hit.
	/// </summary>
	RequireStableGround,
}