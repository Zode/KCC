using FlaxEngine;

namespace KCC;

/// <summary>
/// KCC Simulation driven object.
/// Base class for KCC Actors.
/// </summary>
public class KinematicBase : RigidBody
{
	/// <summary>
	/// The initial position before simulation.
	/// </summary>
	[NoSerialize, HideInEditor] public Vector3 InitialPosition {get; set;} = Vector3.Zero;
	/// <summary>
	/// The initial orientation before simulation.
	/// </summary>
	[NoSerialize, HideInEditor] public Quaternion InitialOrientation {get; set;} = Quaternion.Identity;
	/// <summary>
	/// The current position in simulation.
	/// </summary>
	[NoSerialize, HideInEditor] public Vector3 TransientPosition {get {return _transientPosition;} set {_transientPosition = value;}}
	/// <summary>
	/// The current position in simulation.
	/// </summary>
	protected Vector3 _transientPosition = Vector3.Zero; 
	/// <summary>
	/// The current orientation in simulation.
	/// </summary>
	[NoSerialize, HideInEditor] public Quaternion TransientOrientation {get {return _transientOrientation;} set{_transientOrientation = value;}}
	/// <summary>
	/// The current orientation in simulation.
	/// </summary>
	protected Quaternion _transientOrientation = Quaternion.Identity;
	/// <summary>
	/// Reference to KCC plugin.
	/// </summary>
	protected KCC _kccPlugin;

	/// <summary>
	/// Initializes a new instance of KinematicBase.
	/// </summary>
	public KinematicBase()
		: base()
	{
		_kccPlugin = PluginManager.GetPlugin<KCC>();
	}

	/// <summary>
	/// Set the mover's position directly.
	/// </summary>
	/// <param name="position">World space position</param>
	public void SetPosition(Vector3 position)
	{
		Position = position;
		InitialPosition = position;
		TransientPosition = position;		
	}

	/// <summary>
	/// Set the mover's orientation directly.
	/// </summary>
	/// <param name="orientation">World space orientation</param>
	public void SetOrientation(Quaternion orientation)
	{
		Orientation = orientation;
		InitialOrientation = orientation;
		TransientOrientation = orientation;
	}
}