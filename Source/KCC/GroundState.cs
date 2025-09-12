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
	/// The character is partially grounded (partialy standing on valid ground), checked by four line traces at each "corner' of the character.
	/// </summary>
	PartiallyGrounded,
	/// <summary>
	/// Character is fully grounded (standing on valid ground).
	/// </summary>
	Grounded,
}