using System;

namespace KCC;

/// <summary>
/// Physics flags for KCC characters.
/// </summary>
[Flags]
public enum PhysicsFlag
{
	/// <summary>
	/// Nothing.
	/// </summary>
	None = 0,
	/// <summary>
	/// Hit triggers during cast or overlap.
	/// </summary>
	HitTriggers = 1 << 0,
	/// <summary>
	/// Dispatch KinematicCollision event during cast or overlap.
	/// </summary>
	DispatchEvent = 1 << 1,
	/// <summary>
	/// Try to add <seealso cref="RigidBodyInteraction" />s during cast or overlap.
	/// </summary>
	RigidBodyInteractions = 1 << 2,
}