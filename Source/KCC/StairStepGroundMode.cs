namespace KCC;

/// <summary>
/// </summary>
public enum StairStepGroundMode : byte
{
	/// <summary>
	/// Always stairstep
	/// </summary>
	None,
	/// <summary>
	/// Stairstep only when solid is hit
	/// </summary>
	RequireSolid,
	/// <summary>
	/// Stairstep only when stable solid is hit
	/// </summary>
	RequireStableSolid,
	/// <summary>
	/// Stairstep only when solid with ground tag is hit
	/// </summary>
	RequireGround,
	/// <summary>
	/// Stairstep only when stable solid with ground tag is hit
	/// </summary>
	RequireStableGround,
}