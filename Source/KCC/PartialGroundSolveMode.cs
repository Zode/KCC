namespace KCC;

/// <summary>
/// Partial ground solving mode for KCC characters.
/// Determines how to check for partial ground status.
/// </summary>
public enum PartialGroundSolveMode
{
	/// <summary>
	/// Don't report partial grounds. Using this option gives more performance, but all partial grounds are reported as full grounding.
	/// </summary>
	None,
	/// <summary>
	/// Do four line traces at each of the "corners" of the character.
	/// </summary>
	FourPoint,
}
//TODO: research more detection tactics