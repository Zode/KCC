using System;

namespace KCC;

/// <summary>
/// Ground flags for KCC characters.
/// </summary>
[Flags]
public enum GroundFlag
{
	/// <summary>
	/// Nothing.
	/// </summary>
	None = 0,
	/// <summary>
	/// Ground is solid.
	/// </summary>
	Solid = 1 << 0,
	/// <summary>
	/// Ground is stable.
	/// </summary>
	Stable = 1 << 1,
	/// <summary>
	/// Ground has the ground tag.
	/// </summary>
	GroundTag = 1 << 2,
}