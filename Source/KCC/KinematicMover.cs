using System.Collections.Generic;
using FlaxEngine;

#if FLAX_EDITOR
using FlaxEditor;
#endif

namespace KCC;
#nullable enable

/// <summary>
/// KCC Simulation driven object.
/// Utility class that makes kinematic elevators and such faster to program.
/// </summary>
[ActorContextMenu("New/Physics/Kinematic Mover"), ActorToolbox("Physics")]
public class KinematicMover : KinematicBase
{
	/// <summary>
	/// The current velocity in simulation.
	/// </summary>
	[NoSerialize, HideInEditor] public Vector3 KinematicVelocity {get; private set;} = Vector3.Zero;
	/// <summary>
	/// The current angular velocity in simulation.
	/// </summary>
	[NoSerialize, HideInEditor] public Vector3 KinematicAngularVelocity {get; private set;} = Vector3.Zero;

	/// <summary>
	/// Interface for controlling.
	/// </summary>
	[NoSerialize, HideInEditor] public IKinematicMover? Controller {get; set;} = null;

	private List<Collider> _rigidBodyColliders = [];
	private List<Collider> _kinematicColliders = [];

	/// <inheritdoc />
    public override void OnEnable()
    {
        base.OnEnable();

		#if FLAX_EDITOR
        if(!Editor.IsPlayMode)
        {
            return;
        }
        #endif

		SetupRigidBody();

		foreach(Collider collider in GetChildren<Collider>())
		{
			_kinematicColliders.Add(collider);

			Collider rigidBodyCollider = (Collider)collider.Clone();
			rigidBodyCollider.HideFlags = HideFlags.DontSave;
			rigidBodyCollider.Parent = _rigidBody;
			_rigidBodyColliders.Add(rigidBodyCollider);
		}

		SwitchKinematics(false);

		KCCGlobal.Plugin.Register(this);

		SetPosition(Position);
		SetOrientation(Orientation);
		SyncKinematics();
    }

	/// <inheritdoc />
    public override void OnDisable()
    {
		KCCGlobal.Plugin.Unregister(this);

        base.OnDisable();
    }

	/// <summary>
	/// Calculate movement. This should not ever be called directly.
	/// </summary>
	public void KinematicUpdate()
	{
		if(Controller is null || _rigidBody == null)
		{
			return;
		}

		Controller.KinematicUpdate(out _transientPosition, out _transientOrientation);

		_rigidBody.LinearVelocity = KinematicVelocity = (TransientPosition - InitialPosition) / Time.DeltaTime;

		Quaternion fromCurrentToGoal = TransientOrientation * Quaternion.Invert(InitialOrientation);
		_rigidBody.AngularVelocity = KinematicAngularVelocity = Mathf.DegreesToRadians * fromCurrentToGoal.EulerAngles / Time.DeltaTime;
	}

	/// <inheritdoc />
	public override void SwitchKinematics(bool mode)
	{
		for(int i = 0; i < _kinematicColliders.Count; i++)
		{
			_rigidBodyColliders[i].Layer = mode ? 0 : _kinematicColliders[i].Layer;
			_kinematicColliders[i].IsActive = mode;
		}
	}

	/// <inheritdoc />
	public override void SyncKinematics()
	{
		Position = TransientPosition;
		Orientation = TransientOrientation;
		
		/*for(int i = 0; i < _kinematicColliders.Count; i++)
		{
			_kinematicColliders[i].Orientation = TransientOrientation;
			_kinematicColliders[i].Position = TransientPosition;
		}*/
	}
}