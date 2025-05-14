namespace KCC;

/// <summary>
/// Container for settings related to KCC.
/// </summary>
public class KCCSettings
{
	/// <summary>
	/// Should the system automatically simulate in fixed update?
	/// </summary>
	public bool AutoSimulation = true;
	/// <summary>
	/// Should the system interpolate characters and movers?
	/// </summary>
	public bool Interpolate = true;
	/// <summary>
	/// Initial list capacity of kinematic characters, this is not the limit but rather will avoid unnecessary memory allocations.
	/// </summary>
	public int CharacterInitialCapacity = 100;
	/// <summary>
	/// Initial list capacity of kinematic movers, this is not the limit but rather will avoid unnecessary memory allocations.
	/// </summary>
	public int MoverInitialCapacity = 100;
	/// <summary>
	/// The update mode where interpolation should happen (if enabled).
	/// </summary>
	public InterpolationMode InterpolationMode = InterpolationMode.LateUpdate;
	/// <summary>
	/// Determines if KCC should display its debug visuals in editor when a KCC character is selected.
	/// </summary>
	public bool DebugDisplay = true;
}