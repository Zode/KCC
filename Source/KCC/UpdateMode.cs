namespace KCC;

/// <summary>
/// </summary>
public enum InterpolationMode : byte
{
	/// <summary>
	/// Interpolation should happen in LateUpdate
	/// </summary>
	LateUpdate,
	/// <summary>
	/// Interpolation should happen in Update
	/// </summary>
	Update,
}