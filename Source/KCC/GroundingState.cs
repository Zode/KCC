namespace KCC;

/// <summary>
/// Ground state for KCC characters.
/// </summary>
public enum GroundState
{
	/// <summary>
	/// Character is not grounded (not touching any valid ground).
	/// </summary>
	Ungrounded,
	/// <summary>
	/// Character is grounded (standing on valid ground).
	/// </summary>
	Grounded,
}