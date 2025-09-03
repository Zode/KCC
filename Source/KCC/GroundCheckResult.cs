namespace KCC;

/// <summary>
/// Ground check result for KCC characters.
/// </summary>
public enum GroundCheckResult : byte
{
	/// <summary>
	/// Character is not on anything solid (trace did not hit anything).
	/// </summary>
	NoSolid,
	/// <summary>
	/// Character is on something solid, but is not stable (trace did hit something).
	/// </summary>
	SolidNotStable,
	/// <summary>
	/// Character is on something solid and it is stable, but it does not have the ground tag (trace did hit something).
	/// </summary>
	SolidStableNotGround,
	/// <summary>
	/// Character is on something solid and it is stable and had the ground tag (trace did hit something).
	/// </summary>
	SolidStableGround,
}