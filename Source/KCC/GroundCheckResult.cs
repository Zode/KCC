namespace KCC;

/// <summary>
/// Ground check result for KCC characters.
/// </summary>
public enum GroundCheckResult
{
	/// <summary>
	/// Character is not grounded (trace did not hit anything).
	/// </summary>
	NoGround,
	/// <summary>
	/// Character is grounded, but is not stable (trace hit something).
	/// </summary>
	NotStable,
	/// <summary>
	/// Character is grounded, and it is stable (trace hit something).
	/// </summary>
	Stable,
}