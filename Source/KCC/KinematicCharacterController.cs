using System;
using FlaxEngine;
using System.Collections.Generic;
using System.Linq;

//important: please read these following short articles:
// https://www.peroxide.dk/papers/collision/collision.pdf
// https://arxiv.org/ftp/arxiv/papers/1211/1211.0059.pdf

#if FLAX_EDITOR
using FlaxEditor;
using FlaxEditor.SceneGraph;
#endif

namespace KCC;
#nullable enable

/// <summary>
/// </summary>
[ActorContextMenu("New/Physics/Kinematic Character Controller"), ActorToolbox("Physics")]
public class KinematicCharacterController : KinematicBase
{
    /// <summary>
    /// Collision shape of the character
    /// </summary>
    [EditorDisplay("Character")]
    [EditorOrder(100)]
    public ColliderType ColliderType {get; set;} = ColliderType.Capsule;
    #pragma warning disable 8618
    private Collider _collider;
    #pragma warning restore 8618
    /// <summary>
    /// The contact offset value for the automatically generated collider (must be positive)
    /// </summary>
    [EditorDisplay("Character")]
    [EditorOrder(101)]
    public float ColliderContactOffset {get => _colliderContactOffset; set {_colliderContactOffset = value; SetColliderSize();}}
    private float _colliderContactOffset = 2.0f;
    /// <summary>
    /// The contact offset value that determines the distance that the character hovers above any surface (must be positive)
    /// </summary>
    [EditorDisplay("Character")]
    [EditorOrder(102)]
    public float KinematicContactOffset {get; set;} = 2.0f;
    /// <summary>
    /// Height of the character
    /// </summary>
    [EditorDisplay("Character")]
    [EditorOrder(103)]
    public float ColliderHeight {get => _colliderHeight; set {_colliderHeight = value; SetColliderSize();}}
    private float _colliderHeight = 150.0f;
    /// <summary>
    /// Half the height of the character
    /// </summary>
    [NoSerialize, HideInEditor] public float ColliderHalfHeight {get; private set;} = 150.0f / 2.0f;
    /// <summary>
    /// Radius of the character (only applicable when ColliderType is Capsule or Sphere)
    /// </summary>
    [EditorDisplay("Character")]
    [EditorOrder(104)]
    public float ColliderRadius {get => _colliderRadius; set {_colliderRadius = value; SetColliderSize();}}
    private float _colliderRadius = 50.0f;
    /// <summary>
    /// Half the radius of the character
    /// </summary>
    [NoSerialize, HideInEditor] public float ColliderHalfRadius {get; private set;} = 50.0f / 2.0f;
    /// <summary>
    /// Box extents of the character (only applicable when ColliderType is Box)
    /// </summary>
    [NoSerialize, HideInEditor] public Vector3 BoxExtents => _boxExtents;
    private Vector3 _boxExtents = Vector3.Zero;
    /// <summary>
    /// Maximum allowed amount of unstuck iterations
    /// </summary>
    [EditorDisplay("Physics")]
    [EditorOrder(105)]
    public int MaxUnstuckIterations {get => _maxUnstuckIterations; set => _maxUnstuckIterations = Math.Clamp(value, 0, int.MaxValue);}
    private int _maxUnstuckIterations = 10;
    /// <summary>
    /// Should we filter collisions?
    /// If enabled, the controller will be queried for collision filtering, this is expensive
    /// If disabled, the character will assume everything to be solid, this is less expensive
    /// </summary>
    [EditorDisplay("Physics")]
    [EditorOrder(106)]
    public bool FilterCollisions {get; set;} = false;
    /// <summary>
    /// Determines how much the character should slide upon coming to contact with a surface
    /// </summary>
    [EditorDisplay("Physics")]
    [EditorOrder(108)]
    public float SlideMultiplier {get => _slideMultiplier; set => _slideMultiplier = Mathf.Clamp(value, 0.0f, 1.0f);}
    private float _slideMultiplier = 0.75f;
    /// <summary>
    /// If set to true, the character slide will also be affected by the surface's physics material settings
    /// </summary>
    [EditorDisplay("Physics")]
    [EditorOrder(109)]
    public bool SlideAccountForPhysicsMaterial {get; set;} = true;
    /// <summary>
    /// The layer mask upon which the character collides with
    /// </summary>
    [EditorDisplay("Physics")]
    [EditorOrder(107)]
    public LayersMask CollisionMask {get; set;} = new();
    /// <summary>
    /// Tag used to determine if a collision should be considered valid ground or not,
    /// If left empty, all surfaces determined by MaxSlopeAngle are considered valid ground.
    /// </summary>
    [EditorDisplay("Grounding")]
    [EditorOrder(110)]
    public Tag GroundTag {get; set;} = new();
    /// <summary>
    /// Is the character currently grounded
    /// </summary>
    [NoSerialize, HideInEditor] public bool IsGrounded {get; private set;} = false;
    private bool _wasPreviouslyGrounded = false;
    /// <summary>
    /// Determines if grounding is allowed at all
    /// </summary>
    [NoSerialize, HideInEditor] public bool CanGround {get; set;} = true;
    private bool _forceUnground = false;
    /// <summary>
    /// Ground normal upon which the character is currently standing on,
    /// If not touching ground this will be the opposite of the gravity orientation.
    /// </summary>
    [NoSerialize, HideInEditor] public Vector3 GroundNormal {get; private set;} = Vector3.Up;
    /// <summary>
    /// Distance to surface until the character is considered grounded, KinematicContactOffset is automatically added on this.
    /// </summary>
    [EditorDisplay("Grounding")]
    [EditorOrder(111)]
    public float GroundingDistance {get => _groundingDistance; set => _groundingDistance = Mathf.Clamp(value, 0.0f, float.MaxValue);}
    private float _groundingDistance = 1.0f;
    /// <summary>
    /// Maximum allowed ground snap distance to keep the character grounded while IsGrounded is true
    /// </summary>
    [EditorDisplay("Grounding")]
    [EditorOrder(112)]
    public float GroundSnappingDistance {get => _groundSnappingDistance; set => _groundSnappingDistance = Mathf.Clamp(value, 0.0f, float.MaxValue);}
    private float _groundSnappingDistance = 1024.0f;
    /// <summary>
    /// Maximum allowed ground slope angle, all surfaces below or equal to this limit are considered to be ground
    /// </summary>
    [EditorDisplay("Grounding")]
    [EditorOrder(113)]
    public float MaxSlopeAngle {get => _maxSlopeAngle; set => _maxSlopeAngle = Mathf.Clamp(value, 0.0f, 180.0f);}
    private float _maxSlopeAngle = 66.0f;
    /// <summary>
    /// Determines if stair stepping is allowed at all
    /// </summary>
    [EditorDisplay("Stairstepping")]
    [EditorOrder(114)]
    public bool AllowStairStepping {get; set;} = true;
    /// <summary>
    /// Maximum allowed stair step height distance
    /// </summary>
    [EditorDisplay("Stairstepping")]
    [EditorOrder(115)]
    public float StairStepDistance {get => _stairStepDistance; set => _stairStepDistance = Mathf.Clamp(value, 0.0f, float.MaxValue);}
    private float _stairStepDistance = 50.0f;
    /// <summary>
    /// Behavior mode for stair stepping
    /// </summary>
    [EditorDisplay("Stairstepping")]
    [EditorOrder(116)]
    public StairStepGroundMode StairStepGroundMode {get; set;} = StairStepGroundMode.RequireStableSolid;
    /// <summary>
    /// Minimum distance the character must be able to move forward on a detected step for it to be considered valid
    /// </summary>
    [EditorDisplay("Stairstepping")]
    [EditorOrder(117)]
    public float StairStepMinimumForwardDistance {get => _stairStepMinimumForwardDistance; set => _stairStepMinimumForwardDistance = Mathf.Clamp(value, 0.0f, float.MaxValue);}
    private float _stairStepMinimumForwardDistance = 0.01f;
    /// <summary>
    /// Maximum amount of stair step iterations per frame
    /// </summary>
    [EditorDisplay("Stairstepping")]
    [EditorOrder(118)]
    public int MaxStairStepIterations {get => _maxStairStepIterations; set => _maxStairStepIterations = Math.Clamp(value, 0, int.MaxValue);} 
    private int _maxStairStepIterations = 10;
    /// <summary>
    /// Determines if the character should move with rigidbodies it is standing on
    /// </summary>
    [EditorDisplay("RigidBody interactions")]
    [EditorOrder(119)]
    public RigidBodyMoveMode RigidBodyMoveMode {get; set;} = RigidBodyMoveMode.KinematicMoversOnly;
    /// <summary>
    /// Determines if the character should solve the movements caused by rigidbodies stood upon
    /// If enabled the character will sweep the movements, this is more expensive and more unstable but will cause less potential collision issues
    /// If disabled the character will not sweep the movements, this is less expensive and more stable but will cause potential collision issues
    /// </summary>
    [EditorDisplay("RigidBody interactions")]
    [EditorOrder(120)]
    public bool SolveRigidBodyMovements {get; set;} = false;
    /// <summary>
    /// Determine how to handle dynamic rigidbodies that we have collided with
    /// </summary>
    [EditorDisplay("RigidBody interactions")]
    [EditorOrder(121)]
    public RigidBodyInteractionMode RigidBodyInteractionMode {get; set;} = RigidBodyInteractionMode.PureKinematic;
    private readonly List<RigidBodyInteraction> _rigidBodiesCollided = [];
    /// <summary>
    /// The simulated mass amount for dynamic rigidbody handling
    /// </summary>
    [EditorDisplay("RigidBody interactions")]
    [EditorOrder(122)]
    public float SimulatedMass {get => _simulatedMass; set => _simulatedMass = Mathf.Clamp(value, 0.0f, float.MaxValue);}
    private float _simulatedMass = 1000.0f;

    private Vector3 _internalVelocity = Vector3.Zero;
    private float _internalGravityVelocity = 0.0f;
    /// <summary>
    /// The current gravity as normalized euler angles
    /// </summary>
    [NoSerialize, HideInEditor] public Vector3 GravityEulerNormalized {get; private set;} = Vector3.Down;
    /// <summary>
    /// Velocity, ignoring movements from rigidbody we stood upon
    /// </summary>
    [NoSerialize, HideInEditor] public Vector3 KinematicVelocity {get; set;} = Vector3.Zero;
    /// <summary>
    /// Velocity only from rigidbody we stood upon
    /// </summary>
    [NoSerialize, HideInEditor] public Vector3 KinematicAttachedVelocity {get; set;} = Vector3.Zero;
    /// <summary>
    /// The character's controller
    /// </summary>
    [NoSerialize, HideInEditor] public IKinematicCharacter? Controller {get; set;} = null;
    /// <summary>
    /// The RigidBody we are attached to
    /// </summary>
    [NoSerialize, HideInEditor] public RigidBody? AttachedRigidBody => _attachedRigidBody;
    private RigidBody? _attachedRigidBody = null;

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

		PluginManager.GetPlugin<KCC>().Register(this);

        MaxAngularVelocity = float.MaxValue;
		MaxDepenetrationVelocity = float.MaxValue;
		IsKinematic = true;

        SetPosition(Position);
        SetOrientation(Orientation);

        _collider = ColliderType switch
        {
            ColliderType.Box => AddChild<BoxCollider>(),
            ColliderType.Capsule => AddChild<CapsuleCollider>(),
            ColliderType.Sphere => AddChild<SphereCollider>(),
            _ => throw new NotImplementedException(),
        };

        _collider.Layer = Layer;
        _collider.Tags = Tags;
        _collider.HideFlags = HideFlags.DontSave;// | HideFlags.DontSelect | HideFlags.HideInHierarchy;
        SetColliderSize();
    }

	/// <inheritdoc />
    public override void OnDisable()
    {
		PluginManager.GetPlugin<KCC>().Unregister(this);

        base.OnDisable();
    }

    /// <summary>
    /// </summary>
    public void KinematicUpdate()
    {
        
        if(Scale.X != 1.0f || Scale.Y != 1.0f || Scale.Z != 1.0f)
        {
            #if FLAX_EDITOR
            Debug.LogError("Kinematic controller has non uniform scale!", this);
            #endif
            return;
        }

        if(Controller is null)
        {
            return;
        }

        Controller.KinematicMoveUpdate(out _internalVelocity, out Quaternion _internalOrientation);
        TransientOrientation = _internalOrientation;
        GravityEulerNormalized = (Vector3.Down * TransientOrientation).Normalized;

        _internalGravityVelocity = _internalVelocity.Y;
        _internalVelocity *= TransientOrientation;

        _rigidBodiesCollided.Clear();

        if(IsGrounded)
        {
            _internalVelocity = Controller.KinematicGroundProjection(_internalVelocity, GravityEulerNormalized);
        }

        //flax bug with quaternions from eulers: for now, force perfect down if this is somehow wrong.
		if(GravityEulerNormalized.IsZero)
		{
			GravityEulerNormalized = Vector3.Down;
		}

        //solve any collisions from rigidbodies (including other kinematics), so we can actually try to move
        TransientPosition += UnstuckSolve(0.0f);

        SolveSweep();
        SolveRigidBodyInteractions();
        KinematicVelocity = TransientPosition - InitialPosition;

        RayCastHit? groundTrace = SolveGround();
        _forceUnground = false;
        if(!_wasPreviouslyGrounded && IsGrounded)
        {
            _wasPreviouslyGrounded = true;
            Controller.KinematicGroundingEvent(GroundState.Grounded, groundTrace);
        }
        else if(_wasPreviouslyGrounded && !IsGrounded)
        {
            _wasPreviouslyGrounded = false;
            Controller.KinematicGroundingEvent(GroundState.Ungrounded, groundTrace);
        } 

        if(AttachedRigidBody is not null && RigidBodyMoveMode != RigidBodyMoveMode.None)
        {
            if(SolveRigidBodyMovements)
            {
                KinematicAttachedVelocity = MovementFromRigidBody(AttachedRigidBody, TransientPosition);
                _internalVelocity = KinematicAttachedVelocity;
                SolveSweep();
                //hack: move upwards by contact offset so that we don't clip into the rigidbody if its swinging wildly
                TransientPosition += -GravityEulerNormalized * KinematicContactOffset;
            }
            else
            {
                KinematicAttachedVelocity = MovementFromRigidBody(AttachedRigidBody, TransientPosition);
                TransientPosition += KinematicAttachedVelocity;
                //hack: move upwards by contact offset so that we don't clip into the rigidbody if its swinging wildly
                TransientPosition += -GravityEulerNormalized * KinematicContactOffset;
            }
        }
        else
        {
            KinematicAttachedVelocity = Vector3.Zero;
        }

        Controller.KinematicPostUpdate();
    }

    private void SetColliderSize()
    {
        ColliderHalfHeight = ColliderHeight / 2.0f;
        ColliderHalfRadius = ColliderRadius / 2.0f;

        _boxExtents.X = ColliderHalfRadius;
        _boxExtents.Z = ColliderHalfRadius;
        _boxExtents.Y = ColliderHalfHeight;
        
        _collider.ContactOffset = ColliderContactOffset;
        SetColliderSizeWithInflation(0.0f);
    }

    private void SetColliderSizeWithInflation(float inflate)
    {
        switch(ColliderType)
        {
            case ColliderType.Box:
                BoxCollider box = _collider.As<BoxCollider>();
                box.Size = new(ColliderRadius + inflate, ColliderHeight + inflate, ColliderRadius + inflate);
                break;

            case ColliderType.Capsule:
                CapsuleCollider capsule = _collider.As<CapsuleCollider>();
                capsule.Radius = ColliderRadius + inflate;
                capsule.Height = ColliderHeight - ColliderHalfRadius + inflate;
                //and for some reason this is wrongly rotated in the Z axis by default..
                capsule.LocalOrientation = Quaternion.RotationZ(1.57079633f);
                break;

            case ColliderType.Sphere:
                SphereCollider sphere = _collider.As<SphereCollider>();
                sphere.Radius = ColliderRadius + inflate;
                break;

            default:
                throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Return all colliders we are overlapping with
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="colliders"></param>
    /// <param name="layerMask"></param>
    /// <param name="hitTriggers"></param>
    /// <param name="inflate"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public bool OverlapCollider(Vector3 origin, out Collider[] colliders, uint layerMask = uint.MaxValue, bool hitTriggers = true, float inflate = 1.0f)
    {
        if(!FilterCollisions)
        {
            return ColliderType switch
            {
                ColliderType.Box => Physics.OverlapBox(origin, BoxExtents * inflate, out colliders, TransientOrientation, layerMask, hitTriggers),
                ColliderType.Capsule => Physics.OverlapCapsule(origin, ColliderRadius * inflate, (ColliderHeight - ColliderHalfRadius) * inflate, out colliders, TransientOrientation * Quaternion.RotationZ(1.57079633f), layerMask, hitTriggers),
                ColliderType.Sphere => Physics.OverlapSphere(origin, ColliderRadius * inflate, out colliders, layerMask, hitTriggers),
                _ => throw new NotImplementedException(),
            };
        }

        #pragma warning disable IDE0018
        Collider[] temporaryColliders;
        #pragma warning restore IDE0018 
        bool result = ColliderType switch
        {
            ColliderType.Box => Physics.OverlapBox(origin, BoxExtents * inflate, out temporaryColliders, TransientOrientation, layerMask, hitTriggers),
            ColliderType.Capsule => Physics.OverlapCapsule(origin, ColliderRadius * inflate, (ColliderHeight - ColliderHalfRadius) * inflate, out temporaryColliders, TransientOrientation * Quaternion.RotationZ(1.57079633f), layerMask, hitTriggers),
            ColliderType.Sphere => Physics.OverlapSphere(origin, ColliderRadius * inflate, out temporaryColliders, layerMask, hitTriggers),
            _ => throw new NotImplementedException(),
        };

        colliders = Array.FindAll(temporaryColliders, x => IsColliderValid(x));
        return result;
    }

    /// <summary>
    /// Return all colliders collided with by the cast
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="direction"></param>
    /// <param name="trace"></param>
    /// <param name="distance"></param>
    /// <param name="layerMask"></param>
    /// <param name="hitTriggers"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public bool CastCollider(Vector3 origin, Vector3 direction, out RayCastHit trace, float distance = float.MaxValue, uint layerMask = uint.MaxValue, bool hitTriggers = true)
    {
        if(Controller is null)
        {
            #if FLAX_EDITOR
            Debug.LogError("IKinematicCharacter controller is missing", this);
            #endif

            trace = new()
            {
                Distance = distance
            };
            return false;
        }

        #if FLAX_EDITOR
        if(!direction.IsNormalized)
        {
            Debug.LogError($"CastCollider direction is not normalized! {direction}", this);
        }

        if(DebugIsSelected())
        {
            DebugDraw.DrawWireArrow(origin, Quaternion.FromDirection(direction), distance*0.01f, 1.0f, Color.Yellow, Time.DeltaTime, false);
        }
        #endif

        bool result;
        if(!FilterCollisions)
        {
            result = ColliderType switch
            {
                ColliderType.Box => Physics.BoxCast(origin, BoxExtents, direction, out trace, TransientOrientation, distance, layerMask, hitTriggers),
                ColliderType.Capsule => Physics.CapsuleCast(origin, ColliderRadius, ColliderHeight - ColliderHalfRadius, direction, out trace, TransientOrientation * Quaternion.RotationZ(1.57079633f), distance, layerMask, hitTriggers),
                ColliderType.Sphere => Physics.SphereCast(origin, ColliderRadius, direction, out trace, distance, layerMask, hitTriggers),
                _ => throw new NotImplementedException(),
            };

            if(!result)
            {
                trace.Distance = distance;
            }
            else
            {
                Controller.KinematicCollision(trace);

                RigidBody? otherRb = trace.Collider.AttachedRigidBody;
                if(otherRb is not null)
                {
                    TryAddRigidBodyInteraction(trace, otherRb);
                }

                #if FLAX_EDITOR
                if(DebugIsSelected())
                {
                    DebugDraw.DrawWireArrow(trace.Point, Quaternion.FromDirection(trace.Normal), 1.0f, 1.0f, Color.Red, Time.DeltaTime, false);
                }
                #endif
            }

            return result;
        }

        #pragma warning disable IDE0018
        RayCastHit[] traces;
        #pragma warning restore IDE0018 
        result = ColliderType switch
        {
            ColliderType.Box => Physics.BoxCastAll(origin, BoxExtents, direction, out traces, TransientOrientation, distance, layerMask, hitTriggers),
            ColliderType.Capsule => Physics.CapsuleCastAll(origin, ColliderRadius, ColliderHeight - ColliderHalfRadius, direction, out traces, TransientOrientation * Quaternion.RotationZ(1.57079633f), distance, layerMask, hitTriggers),
            ColliderType.Sphere => Physics.SphereCastAll(origin, ColliderRadius, direction, out traces, distance, layerMask, hitTriggers),
            _ => throw new NotImplementedException(),
        };

        int i = 0;
        if(result)
        {
            result = false;

            Array.Sort(traces, (lValue, rValue) => lValue.Distance.CompareTo(rValue.Distance));
            for(; i < traces.Length; i++)
            {
                if(traces[i].Collider is not Collider collider)
                {
                    //must be a terrain patch, for now we always collide with terrain
                    result = true;
                    break;
                }

                if(IsColliderValid(collider))
                {
                    result = true;
                    break;
                }
            }
        }

        if(!result)
        {
            //make up a bogus trace, as this is only used for stair stepping
            trace = new()
            {
                Distance = distance
            };
        }
        else
        {
            trace = traces[i]; //have to do it this way, because C# cries otherwise
            Controller.KinematicCollision(trace);

            RigidBody? otherRb = trace.Collider.AttachedRigidBody;
            if(otherRb is not null)
            {
                TryAddRigidBodyInteraction(trace, otherRb);
            }

            #if FLAX_EDITOR
            if(DebugIsSelected())
            {
                DebugDraw.DrawWireArrow(trace.Point, Quaternion.FromDirection(trace.Normal), 1.0f, 1.0f, Color.Red, Time.DeltaTime, false);
            }
            #endif
        }

        return result;
    }
    
    private bool IsColliderValid(Collider collider)
    {
        if(collider == _collider)
        {
            return false;
        }

        if(Controller is null)
        {
            #if FLAX_EDITOR
            Debug.LogError("IKinematicCharacter controller is missing", this);
            #endif

            return false;
        }

        return Controller.KinematicCollisionValid(collider);
    }

    private void TryAddRigidBodyInteraction(RayCastHit trace, RigidBody rigidBody)
    {
        //only allow non-KCC rigidbodies for now
        if(rigidBody is KinematicCharacterController otherKCC)
        {
            return;
        }

        if(rigidBody.IsKinematic)
        {
            return;
        }

        if(_rigidBodiesCollided.Any(x => x.RigidBody == rigidBody))
        {
            return;
        }

        RigidBodyInteraction rbInteraction = new()
        {
            RigidBody = rigidBody,
            Point = trace.Point,
            Normal = trace.Normal,
            CharacterVelocity = _internalVelocity,
            BodyVelocity = rigidBody.LinearVelocity,
        };

        _rigidBodiesCollided.Add(rbInteraction);
    }

    private void SolveSweep()
    {
        Vector3 originalVelocityNormalized = _internalVelocity.Normalized;
        int unstuckSolves = 0;

        //we can realistically only collide with 2 planes before we lose all degrees of freedom (3 plane intersection is a point)
        Vector3 firstPlane = Vector3.Zero;
        for(int i = 0; i < 3; i++)
        {
            if(_internalVelocity.IsZero)
            {
                return;
            }

            //are we about to go backwards? (unwanted direction, fixes issues  with jiggling in corners with obtuse angles)
            if(Vector3.Dot(originalVelocityNormalized, _internalVelocity.Normalized) < 0.0f)
            {
                return;
            }
            
            if(!CastCollider(TransientPosition, _internalVelocity.Normalized, out RayCastHit trace, _internalVelocity.Length + KinematicContactOffset, CollisionMask, false))
            {
                #if FLAX_EDITOR
                if(DebugIsSelected())
                {
                    DebugDrawCollider(TransientPosition + _internalVelocity, TransientOrientation, Color.Blue, Time.DeltaTime, false);
                    DebugDraw.DrawWireArrow(TransientPosition, Quaternion.FromDirection(_internalVelocity.Normalized), _internalVelocity.Length * 0.01f, 1.0f, Color.Blue, Time.DeltaTime, false);
                }
                #endif

                //no collision, full speed ahead!
                TransientPosition += _internalVelocity;
                return;
            }

            if(trace.Distance == 0.0f && unstuckSolves < MaxUnstuckIterations)
            {
                //trace collided with zero distance?
                //trace must have started inside something, so we're most likely stuck.
                //try to solve issue with inflated collider and re-try sweep.
                TransientPosition += UnstuckSolve(KinematicContactOffset);
                i--;
                unstuckSolves++;
                continue;
            }

            //pull back a bit, otherwise we would be constantly intersecting with the plane
            float distance = Math.Max(trace.Distance - KinematicContactOffset, 0.0f);

            #if FLAX_EDITOR
            if(DebugIsSelected())
            {
                DebugDrawCollider(TransientPosition + (_internalVelocity.Normalized * distance), TransientOrientation, Color.Blue, Time.DeltaTime, false);
                DebugDraw.DrawWireArrow(TransientPosition, Quaternion.FromDirection(_internalVelocity.Normalized), distance*0.01f, 1.0f, Color.Blue, Time.DeltaTime, false);
            }
            #endif

            //move to collision point
            TransientPosition += _internalVelocity.Normalized * distance;

            if(IsGrounded)
            {
                SolveStairSteps(ref _transientPosition, ref _internalVelocity, ref distance, ref trace.Normal);
            }

            if(i == 0)
            {
                firstPlane = trace.Normal;
                //project for next iteration
                _internalVelocity = Vector3.ProjectOnPlane(_internalVelocity.Normalized, trace.Normal) * Math.Max(_internalVelocity.Length - distance, 0.0f);
            }
            else if(i == 1)
            {
                //project for next (final) iteration, but only along the crease
                Vector3 wantedVelocity = Vector3.ProjectOnPlane(_internalVelocity.Normalized, firstPlane) * Math.Max(_internalVelocity.Length - distance, 0.0f);
                wantedVelocity = Vector3.ProjectOnPlane(wantedVelocity.Normalized, trace.Normal) * Math.Max(wantedVelocity.Length - distance, 0.0f);

                Vector3 crease = Vector3.Cross(firstPlane, trace.Normal).Normalized;
                float creaseDistance = Vector3.Dot(wantedVelocity, crease);

                #if FLAX_EDITOR
                if(DebugIsSelected())
                {
                    DebugDraw.DrawLine(TransientPosition + (crease * 64), TransientPosition - (crease * 64), Color.Purple, Time.DeltaTime, false);
                }
                #endif

                //consider anything 90 deg and less to be acute, and anything above to be obtuse.
                bool isAcute = Vector3.Dot(firstPlane, trace.Normal) <= 0.0f;

                //obtuse corners need to be handled differently, least we want the controller to get snagged in them.
                if(!isAcute)
                {
                    Vector3 averagePlane = (trace.Normal + firstPlane).Normalized;
                    Vector3 averageLeft = Vector3.Left * Quaternion.FromDirection(averagePlane);
 
                    //are we moving more towards the left plane?
                    bool firstIsBetter = Vector3.Dot(originalVelocityNormalized, averageLeft) >= 0.0f;

                    #if FLAX_EDITOR
                    if(DebugIsSelected())
                    {
                        DebugDraw.DrawWireArrow(TransientPosition, Quaternion.FromDirection(averagePlane), 1.0f, 1.0f, Color.Purple, Time.DeltaTime, false);
                    }
                    #endif

                    if(Vector3.Dot(averageLeft, firstPlane) > 0.0f)
                    {
                        //swap if going the other way because the plane order changes
                        firstIsBetter = !firstIsBetter;
                    }

                    //also nudge by both planes in hopes of pushing out of the corner, similar to how quake3 handles this.
                    //normally the surrounding code would fix the issue, however it is not enough to solve vertical movement in obtuse corners
                    //so this is needed, sadly this does introduce slight jiggling in some obtuse corners :( but its better than getting stuck.
                    _internalVelocity += firstPlane; 
                    _internalVelocity += trace.Normal;
                    if(Vector3.Dot(_internalVelocity.Normalized, GravityEulerNormalized) > 0.0f)
                    {
                        TransientPosition += firstPlane * 0.1f;
                        TransientPosition += trace.Normal * 0.1f;
                    }
                    
                    //first plane is always the left plane due to how the sweep works.
                    if(firstIsBetter)
                    {
                        _internalVelocity = Vector3.ProjectOnPlane(_internalVelocity.Normalized, firstPlane) * Math.Max(_internalVelocity.Length - distance, 0.0f);

                        #if FLAX_EDITOR
                        if(DebugIsSelected())
                        {
                            DebugDraw.DrawWireArrow(TransientPosition, Quaternion.FromDirection(firstPlane), 1.0f, 1.0f, Color.Purple, Time.DeltaTime, false);
                        }
                        #endif
                    }
                    else
                    {
                        _internalVelocity = Vector3.ProjectOnPlane(_internalVelocity.Normalized, trace.Normal) * Math.Max(_internalVelocity.Length - distance, 0.0f);
                        
                        #if FLAX_EDITOR
                        if(DebugIsSelected())
                        {
                            DebugDraw.DrawWireArrow(TransientPosition, Quaternion.FromDirection(trace.Normal), 1.0f, 1.0f, Color.Purple, Time.DeltaTime, false);
                        }
                        #endif
                    }
                }
                else
                {
                    //constrain to crease
                    _internalVelocity = Vector3.ProjectOnPlane(crease, trace.Normal) * creaseDistance;
                }
            }

            //also slow down depending on the angle of hit plane (and physics material if enabled)
            _internalVelocity *= (1.0f - Math.Abs(Vector3.Dot(_internalVelocity.Normalized, trace.Normal))) * SlideMultiplier;
            if(SlideAccountForPhysicsMaterial && trace.Material is not null)
            {
                _internalVelocity *= 1.0f - trace.Material.Friction;
            }
        }
    }

    private void SolveStairSteps(ref Vector3 position, ref Vector3 velocity, ref float distance, ref Vector3 sweepNormal)
    {
        if(!AllowStairStepping || AttachedRigidBody is not null)
        {
            return;
        }

        Vector3 oldPosition = position;
        int iterations = 0;
        while(SolveStairStep(ref position, ref velocity, ref distance, ref sweepNormal) && iterations < MaxStairStepIterations)
        {
            iterations++;

            //ensure we actually stepped a stair before attempting to iterate again
            if(oldPosition.Y >= position.Y)
            {
                break;
            }
        }
    }

    private bool SolveStairStep(ref Vector3 position, ref Vector3 velocity, ref float distance, ref Vector3 sweepNormal)
    {
        if(velocity.IsZero)
        {
            return false;
        }

        Vector3 velocityNormalized = velocity.Normalized;
        if(IsNormalStableGround(sweepNormal))
        {
            return false;
        }

        //can we clear upwards (by any amount)?
        CastCollider(position, -GravityEulerNormalized, out RayCastHit trace, StairStepDistance + KinematicContactOffset, CollisionMask, false);

        float temporaryDistance = Math.Max(trace.Distance - KinematicContactOffset, 0.0f);
        if(temporaryDistance == 0.0f)
        {
            return false;
        }

        //move to possible ceiling position
        Vector3 temporaryPosition = position - (GravityEulerNormalized * temporaryDistance);
        temporaryDistance = Math.Max(velocity.Length - distance, 0.0f);
        if(temporaryDistance == 0.0f)
        {
            return false;
        }

        Vector3 remainingVelocity = velocityNormalized * temporaryDistance;
        if(remainingVelocity.IsZero)
        {
            return false;
        }

        Vector3 remainingVelocityNormalized = remainingVelocity.Normalized;

        //can we clear forwards with the remaining velocity (by any amount)?
        bool HitForward = CastCollider(temporaryPosition, remainingVelocityNormalized, out trace, temporaryDistance + KinematicContactOffset, CollisionMask, false);
        Vector3 newNormal = trace.Normal;
        temporaryDistance = Math.Max(trace.Distance - KinematicContactOffset, 0.0f);
        if(temporaryDistance == 0.0f || temporaryDistance < StairStepMinimumForwardDistance)
        {
            return false;
        }

        if(HitForward)
        {
            RigidBody? rb = trace.Collider.AttachedRigidBody;
            if(rb is not null)
            {
                if(!rb.IsKinematic)
                {
                    return false;
                }
            }
        }
        
        //move to possible new wall collision position
        temporaryPosition += remainingVelocityNormalized * temporaryDistance;
        //can we stand on this? if so then also snap to floor.
        bool hasSolidBelow = CastCollider(temporaryPosition, GravityEulerNormalized, out trace, StairStepDistance + KinematicContactOffset, CollisionMask, false);
        //all modes need some sort of solid.
        if(!hasSolidBelow && StairStepGroundMode != StairStepGroundMode.None)
        {
            return false;
        }

        switch(StairStepGroundMode)
        {
            case StairStepGroundMode.RequireStableSolid:
            case StairStepGroundMode.RequireStableGround:
                if(!IsNormalStableGround(trace.Normal))
                {
                    return false;
                }

                break;
        }

        switch(StairStepGroundMode)
        {
            case StairStepGroundMode.RequireGround:
            case StairStepGroundMode.RequireStableGround:
                if(GroundTag.Index != 0 && !trace.Collider.HasTag(GroundTag))
                {
                    return false;
                }

                break;
        }

        position = temporaryPosition + (GravityEulerNormalized * Math.Max(trace.Distance - KinematicContactOffset, 0.0f));
        velocity = remainingVelocity;
        distance = temporaryDistance;

        //update potential wall for next sweep solve iteration also
        if(HitForward)
        {
            sweepNormal = newNormal;
        }

        #if FLAX_EDITOR
        if(DebugIsSelected())
        {
            DebugDrawCollider(position, TransientOrientation, Color.Magenta, 1.0f, false);
        }
        #endif

        return true;
    }

    private Vector3 MovementFromRigidBody(RigidBody rigidBody, Vector3 position)
    {
        if(Controller is null)
        {
            #if FLAX_EDITOR
            Debug.LogError("IKinematicCharacter controller is missing", this);
            #endif
            
            return Vector3.Zero;
        }

        if(rigidBody is KinematicMover mover)
        {
            rigidBody.LinearVelocity = mover.KinematicVelocity;
            rigidBody.AngularVelocity = mover.KinematicAngularVelocity;
        }

        Vector3 velocity = rigidBody.LinearVelocity * Time.DeltaTime;
        Vector3 center = rigidBody.Transform.TransformPoint(rigidBody.CenterOfMass);
        Vector3 offset = position - center;
        Quaternion rotation = Quaternion.Euler(rigidBody.AngularVelocity * Mathf.RadiansToDegrees * Time.DeltaTime);
        Vector3 final = center + (offset * rotation);
        velocity += final - position;

        Controller.KinematicAttachedRigidBodyUpdate(rigidBody);
        return velocity;
    }

    /// <summary>
    /// Force the character to unground
    /// </summary>
    public void ForceUnground()
    {
        _forceUnground = true;
    }

    private RayCastHit? SolveGround()
    {
        if(_forceUnground)
        {
            AttachToRigidBody(null);
            IsGrounded = false;
            GroundNormal = -GravityEulerNormalized;
            return null;
        }

        if(!CanGround)
        {
            AttachToRigidBody(null);
            IsGrounded = false;
            GroundNormal = -GravityEulerNormalized;
            return null;
        }

        //no point grounding if not going downwards (this prevents the controller from grounding during forced unground jumps)
        if(!IsGrounded && _internalGravityVelocity > 0)
        {
            return null;
        }

        RayCastHit? groundTrace;
        if(!IsGrounded)
        {
            groundTrace = GroundCheck(GroundingDistance);
        }
        else
        {
            groundTrace = GroundCheck(GroundingDistance + StairStepDistance);
        }

        SnapToGround();
        return groundTrace;
    }

    private RayCastHit? GroundCheck(float distance)
    {
        IsGrounded = CastCollider(TransientPosition, GravityEulerNormalized, out RayCastHit trace, distance + KinematicContactOffset, CollisionMask, false);
        if(!IsGrounded)
        {
            AttachToRigidBody(null);
            GroundNormal = -GravityEulerNormalized;
            return null;
        }

        if(!IsNormalStableGround(trace.Normal))
        {
            AttachToRigidBody(null);
            IsGrounded = false;
            GroundNormal = -GravityEulerNormalized;
            return null;
        }

        if(GroundTag.Index != 0 && !trace.Collider.HasTag(GroundTag))
        {
            AttachToRigidBody(null);
            IsGrounded = false;
            GroundNormal = -GravityEulerNormalized;
            return null;
        }

        GroundNormal = trace.Normal;
        AttachToRigidBody(trace.Collider.AttachedRigidBody);
        return trace;
    }

    private void SnapToGround()
    {
        if(!IsGrounded)
        {
            return;
        }

        if(!CastCollider(TransientPosition, GravityEulerNormalized, out RayCastHit trace, GroundSnappingDistance + KinematicContactOffset, CollisionMask, false))
        {
            return;
        }

        TransientPosition += GravityEulerNormalized * Math.Max(trace.Distance - KinematicContactOffset, 0.0f);
    }

    /// <summary>
    /// Is the normal vector considered stable ground?
    /// </summary>
    /// <param name="normal"></param>
    /// <returns>true if stable</returns>
    public bool IsNormalStableGround(Vector3 normal)
    {
        return Vector3.Angle(-GravityEulerNormalized, normal) <= MaxSlopeAngle;
    }

    /// <summary>
    /// Calculates the necessary vector3 to move out of collisions
    /// </summary>
    /// <param name="inflate">extra size added to the collider size</param>
    /// <returns></returns>
    public Vector3 UnstuckSolve(float inflate)
    {
        if(Controller is null)
        {
            #if FLAX_EDITOR
            Debug.LogError("IKinematicCharacter controller is missing", this);
            #endif

            return Vector3.Zero;
        }

        if(!OverlapCollider(TransientPosition, out Collider[] colliders, CollisionMask, false, 1.0f + inflate))
        {
            return Vector3.Zero;
        }

        //we're inside something, so calculate a vector that would push us out of everything we are currently colliding with.
        Vector3 requiredPush = Vector3.Zero;

        //need inflate the colliders a bit for the ComputePenetration, as the collider's contact offset is ignored
        SetColliderSizeWithInflation(inflate);
        for(int i = 0; i < colliders.Length; i++)
        {
            if(!Collider.ComputePenetration(_collider, colliders[i], out Vector3 penetrationDirection, out float penetrationDistance))
            {
                continue; 
            }

            Controller.KinematicUnstuckEvent(colliders[i], penetrationDirection, penetrationDistance);
            if(inflate == 0.0f)
            {
                requiredPush += penetrationDirection * (penetrationDistance + KinematicContactOffset);
            }
            else
            {
                requiredPush += penetrationDirection * penetrationDistance;
            }
        }

        SetColliderSizeWithInflation(0.0f);
        
        return requiredPush;
    }

    /// <summary>
    /// Utility function to project a direction along the ground normal without any lateral movement (accounts for gravity direction)
    /// </summary>
    /// <param name="direction">normalized direction</param>
    /// <returns>projected normalized direction</returns>
    public Vector3 GroundTangent(Vector3 direction)
    {
        Vector3 right = Vector3.Cross(direction, -GravityEulerNormalized) ;
        return Vector3.Cross(GroundNormal, right).Normalized;
    }

    private void SolveRigidBodyInteractions()
    {
        if(RigidBodyInteractionMode == RigidBodyInteractionMode.None)
        {
            return;
        }

        foreach(RigidBodyInteraction rbInteraction in _rigidBodiesCollided)
        {
            if(rbInteraction.RigidBody == AttachedRigidBody)
            {
                continue;
            }

            if(RigidBodyInteractionMode == RigidBodyInteractionMode.Manual)
            {
                if(Controller is null)
                {
                    #if FLAX_EDITOR
                    Debug.LogError("IKinematicCharacter controller is missing", this);
                    #endif
                    
                    return;
                }
                
                Controller.KinematicRigidBodyInteraction(rbInteraction);
                continue;
            }

            float massRatio = 1.0f;
            if(RigidBodyInteractionMode == RigidBodyInteractionMode.SimulateKinematic)
            {
                massRatio = SimulatedMass / (SimulatedMass + rbInteraction.RigidBody.Mass);
            }

            Vector3 force = Vector3.ProjectOnPlane(rbInteraction.CharacterVelocity, GravityEulerNormalized) * SimulatedMass;
            rbInteraction.RigidBody.WakeUp();
            rbInteraction.RigidBody.AddForceAtPosition(force * massRatio, rbInteraction.Point, ForceMode.Impulse);
        }

        return;
    }

    private void AttachToRigidBody(RigidBody? rigidBody)
    {
        if(Controller is null)
        {
            #if FLAX_EDITOR
            Debug.LogError("IKinematicCharacter controller is missing", this);
            #endif
            
            return;
        }

        if(rigidBody == _attachedRigidBody || RigidBodyMoveMode == RigidBodyMoveMode.None)
        {
            return;
        }

        if(rigidBody is null)
        {
            if(_attachedRigidBody is not null)
            {
                Controller.KinematicAttachedRigidBodyEvent(false, _attachedRigidBody);
                _attachedRigidBody = null;
            }
            
            return;
        }


        if(RigidBodyMoveMode == RigidBodyMoveMode.KinematicMoversOnly)
        {
            if(rigidBody is not KinematicMover)
            {
                AttachToRigidBody(null);
                return;
            }
        }

        if(!Controller.KinematicCanAttachToRigidBody(rigidBody))
        {
            AttachToRigidBody(null);
            return;
        }

        _attachedRigidBody = rigidBody;
        Controller.KinematicAttachedRigidBodyEvent(true, rigidBody);
    }

    #if FLAX_EDITOR
    private bool DebugIsSelected()
    {
        foreach(SceneGraphNode node in Editor.Instance.SceneEditing.Selection)
        {
            if(node is not ActorNode actorNode)
            {
                continue;
            }

            if(actorNode.Actor == this)
            {
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc />
    public override void OnDebugDrawSelected()
    {
        base.OnDebugDrawSelected();

        if(!Editor.IsPlayMode)
        {
            DebugDrawCollider(Position, Orientation, Color.YellowGreen, 0.0f, false);
            return;
        }

        DebugDrawCollider(TransientPosition, TransientOrientation, Color.YellowGreen, 0.0f, false);
        DebugDraw.DrawWireArrow(TransientPosition, TransientOrientation, 1.0f, 1.0f, Color.GreenYellow, 0.0f, false);
        DebugDraw.DrawWireArrow(TransientPosition, Quaternion.FromDirection(_internalVelocity.Normalized), _internalVelocity.Length*0.01f, 1.0f, Color.YellowGreen, 0.0f, false);
    }

    private void DebugDrawCollider(Vector3 position, Quaternion orientation, Color color, float time, bool depthTest)
    {
        switch(ColliderType)
        {
            case ColliderType.Box:
                DebugDrawBox(position, orientation, color, time, depthTest);
                break;

            case ColliderType.Capsule:
                DebugDrawCapsule(position, orientation, color, time, depthTest);
                break;

            case ColliderType.Sphere:
                DebugDrawSphere(position, color, time, depthTest);
                break;

            default:
                throw new NotImplementedException();
        }
    }

    private void DebugDrawBox(Vector3 position, Quaternion orientation, Color color, float time, bool depthTest)
    {
        Matrix matrix = Matrix.CreateWorld(position, Vector3.Forward * orientation, Vector3.Up * orientation);
        DebugDraw.DrawWireBox(new OrientedBoundingBox(
            new Vector3(ColliderHalfRadius, -ColliderHalfHeight, -ColliderHalfRadius), matrix),
            color, time, depthTest);
    }

    private void DebugDrawCapsule(Vector3 position, Quaternion orientation, Color color, float time, bool depthTest)
    {
        //for some reason this is rotated by 90 degrees unlike other debug draws..
        Quaternion fixedOrientation = orientation * Quaternion.RotationX(1.57079633f);
        DebugDraw.DrawWireTube(position, fixedOrientation, ColliderRadius, ColliderHeight, color, time, depthTest);
    }

    private void DebugDrawSphere(Vector3 position, Color color, float time, bool depthTest)
    {
        DebugDraw.DrawWireSphere(new(position, ColliderRadius), color, time, depthTest);
    }
    #endif
}