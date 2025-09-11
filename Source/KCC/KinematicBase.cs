using System;
using System.Runtime.CompilerServices;
using FlaxEngine;

namespace KCC;
#nullable enable

/// <summary>
/// KCC Simulation driven object.
/// Base class for KCC Actors.
/// </summary>
public class KinematicBase : Actor
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
	/// The KCC Actor's RigidBody
	/// </summary>
	protected RigidBody? _rigidBody = null;
	/// <summary>
	/// The KCC Actor's RigidBody
	/// </summary>
	public RigidBody? RigidBody => _rigidBody;

	/// <summary>
	/// Set the mover's position directly.
	/// </summary>
	/// <param name="position">World space position</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetPosition(Vector3 position)
	{
		Position = position;
		InitialPosition = position;
		TransientPosition = position;
		SyncKinematics();	
	}

	/// <summary>
	/// Set the mover's orientation directly.
	/// </summary>
	/// <param name="orientation">World space orientation</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetOrientation(Quaternion orientation)
	{
		Orientation = orientation;
		InitialOrientation = orientation;
		TransientOrientation = orientation;
		SyncKinematics();
	}

	/// <summary>
	/// Set up the KCC Actor's RigidBody
	/// </summary>
	public void SetupRigidBody()
	{
		_rigidBody = AddChild<RigidBody>();
		_rigidBody.LinearDamping = 0.0f;
		_rigidBody.AngularDamping = 0.0f;
		_rigidBody.MaxAngularVelocity = float.MaxValue;
		_rigidBody.MaxDepenetrationVelocity = float.MaxValue;
		_rigidBody.IsKinematic = true;
		_rigidBody.StaticFlags = StaticFlags;
		_rigidBody.Layer = Layer;
		_rigidBody.Tags = Tags;
		_rigidBody.HideFlags = HideFlags.DontSave;
	}

	/// <summary>
	/// Switch the Collider(s) to kinematic Collider(s) or vice versa.
	/// </summary>
	/// <param name="mode">if <c>true</c> Collider(s) are set to kinematic Collider(s), if <c>false</c> Collider(s) are set to RigidBody Collider(s)</param>
	public virtual void SwitchKinematics(bool mode)
	{
	}

	/// <summary>
	/// Synchronize the kinematic colliders with the KCC Actor's transient position and orientation
	/// </summary>
	public virtual void SyncKinematics()
	{
	}
}