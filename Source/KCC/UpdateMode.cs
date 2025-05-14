namespace KCC;

/// <summary>
/// KCC Interpolation mode.
/// NOTE: This will be removed in the future once Flax is capable of synching event loop with rendering. (waiting for PR to be accepted).
/// </summary>
public enum InterpolationMode : byte
{
	/// <summary>
	/// Interpolation should happen in LateUpdate.
	/// </summary>
	LateUpdate,
	/// <summary>
	/// Interpolation should happen in Update.
	/// </summary>
	Update,
}