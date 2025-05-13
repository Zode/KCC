using FlaxEngine;

namespace KCC;
#nullable enable

/// <summary>
/// Utility class that makes kinematic elevators and such faster to program.
/// </summary>
[ActorContextMenu("New/Physics/Kinematic Mover"), ActorToolbox("Physics")]
public class KinematicMover : KinematicBase
{
	/// <summary>
	/// The current velocity in simulation
	/// </summary>
	[NoSerialize, HideInEditor] public Vector3 KinematicVelocity {get; private set;} = Vector3.Zero;
	/// <summary>
	/// The current angular velocity in simulation
	/// </summary>
	[NoSerialize, HideInEditor] public Vector3 KinematicAngularVelocity {get; private set;} = Vector3.Zero;

	/// <summary>
	/// Interface for controlling
	/// </summary>
	[NoSerialize, HideInEditor] public IKinematicMover? Controller {get; set;} = null;

	/// <inheritdoc />
    public override void OnEnable()
    {
        base.OnEnable();

		_kccPlugin.Register(this);

		MaxAngularVelocity = float.MaxValue;
		MaxDepenetrationVelocity = float.MaxValue;
		IsKinematic = true;

		SetPosition(Position);
		SetOrientation(Orientation);
    }

	/// <inheritdoc />
    public override void OnDisable()
    {
		_kccPlugin.Unregister(this);

        base.OnDisable();
    }

	/// <summary>
	/// Occurs on kinematic update
	/// </summary>
	public void KinematicUpdate()
	{
		if(Controller is null)
		{
			return;
		}

		Controller.KinematicUpdate(out _transientPosition, out _transientOrientation);

		KinematicVelocity = (TransientPosition - InitialPosition) / Time.DeltaTime;
		Quaternion fromCurrentToGoal = TransientOrientation * Quaternion.Invert(InitialOrientation);
		KinematicAngularVelocity = Mathf.DegreesToRadians * fromCurrentToGoal.EulerAngles / Time.DeltaTime;
	}
}