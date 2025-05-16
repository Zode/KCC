using System;
using FlaxEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;

#if FLAX_EDITOR
using FlaxEditor;
using FlaxEditor.SceneGraph;
#endif

#if USE_LARGE_WORLDS
using Real = System.Double;
#else
using Real = System.Single;
#endif

//important: please read these following short articles:
// https://www.peroxide.dk/papers/collision/collision.pdf
// https://arxiv.org/ftp/arxiv/papers/1211/1211.0059.pdf

namespace KCC;
#nullable enable

/// <summary>
/// KCC Simulation driven character controller object.
/// </summary>
[ActorContextMenu("New/Physics/Kinematic Character Controller"), ActorToolbox("Physics")]
public class KinematicCharacterController : KinematicBase
{
    /// <summary>
    /// Collision shape of the character.
    /// </summary>
    [EditorDisplay("Character")]
    [EditorOrder(100)]
    public ColliderType ColliderType {get; set;} = ColliderType.Capsule;
    private Collider? _collider = null;
    /// <summary>
    /// The contact offset value for the automatically generated collider (must be positive).
    /// </summary>
    [EditorDisplay("Character")]
    [EditorOrder(101)]
    public float ColliderContactOffset {get => _colliderContactOffset; set {_colliderContactOffset = value; SetColliderSize();}}
    private float _colliderContactOffset = 2.0f;
    /// <summary>
    /// The contact offset value that determines the distance that the character hovers above any surface (must be positive).
    /// </summary>
    [EditorDisplay("Character")]
    [EditorOrder(102)]
    public Real KinematicContactOffset {get; set;} = 2.0f;
    /// <summary>
    /// Height of the character.
    /// </summary>
    [EditorDisplay("Character")]
    [EditorOrder(103)]
    public float ColliderHeight {get => _colliderHeight; set {_colliderHeight = value; SetColliderSize();}}
    private float _colliderHeight = 150.0f;
    /// <summary>
    /// Half the height of the character.
    /// </summary>
    [NoSerialize, HideInEditor] public float ColliderHalfHeight {get; private set;} = 150.0f / 2.0f;
    /// <summary>
    /// Radius of the character (only applicable when ColliderType is Capsule or Sphere).
    /// </summary>
    [EditorDisplay("Character")]
    [EditorOrder(104)]
    public float ColliderRadius {get => _colliderRadius; set {_colliderRadius = value; SetColliderSize();}}
    private float _colliderRadius = 50.0f;
    /// <summary>
    /// Half the radius of the character.
    /// </summary>
    [NoSerialize, HideInEditor] public float ColliderHalfRadius {get; private set;} = 50.0f / 2.0f;
    /// <summary>
    /// Box extents of the character (only applicable when ColliderType is Box).
    /// </summary>
    [NoSerialize, HideInEditor] public Vector3 BoxExtents => _boxExtents;
    private Vector3 _boxExtents = Vector3.Zero;
    /// <summary>
    /// Maximum allowed amount of unstuck iterations.
    /// </summary>
    [EditorDisplay("Physics")]
    [EditorOrder(105)]
    public int MaxUnstuckIterations {get => _maxUnstuckIterations; set => _maxUnstuckIterations = Math.Clamp(value, 0, int.MaxValue);}
    private int _maxUnstuckIterations = 10;
    /// <summary>
    /// Should we filter collisions?
    /// If enabled, the controller will be queried for collision filtering, this is expensive.
    /// If disabled, the character will assume everything to be solid, this is less expensive.
    /// </summary>
    [EditorDisplay("Physics")]
    [EditorOrder(106)]
    public bool FilterCollisions {get; set;} = false;
    /// <summary>
    /// Determines how much the character should slide upon coming to contact with a surface.
    /// </summary>
    [EditorDisplay("Physics")]
    [EditorOrder(108)]
    public float SlideMultiplier {get => _slideMultiplier; set => _slideMultiplier = Mathf.Clamp(value, 0.0f, 1.0f);}
    private float _slideMultiplier = 0.75f;
    /// <summary>
    /// If set to true, the character slide will also be affected by the surface's physics material settings.
    /// </summary>
    [EditorDisplay("Physics")]
    [EditorOrder(109)]
    public bool SlideAccountForPhysicsMaterial {get; set;} = true;
    /// <summary>
    /// The layer mask upon which the character collides with.
    /// </summary>
    [EditorDisplay("Physics")]
    [EditorOrder(107)]
    public LayersMask CollisionMask {get; set;} = new();
    /// <summary>
    /// Tag used to determine if a collision should be considered valid ground or not.
    /// If left empty, all surfaces determined by MaxSlopeAngle are considered valid ground.
    /// </summary>
    [EditorDisplay("Grounding")]
    [EditorOrder(110)]
    public Tag GroundTag {get; set;} = new();
    /// <summary>
    /// Is the character currently grounded.
    /// </summary>
    [NoSerialize, HideInEditor] public bool IsGrounded {get; private set;} = false;
    private bool _wasPreviouslyGrounded = false;
    /// <summary>
    /// Determines if grounding is allowed at all.
    /// </summary>
    [NoSerialize, HideInEditor] public bool CanGround {get; set;} = true;
    private bool _forceUnground = false;
    /// <summary>
    /// Ground normal upon which the character is currently standing on.
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
    /// Maximum allowed ground snap distance to keep the character grounded while IsGrounded is true.
    /// </summary>
    [EditorDisplay("Grounding")]
    [EditorOrder(112)]
    public float GroundSnappingDistance {get => _groundSnappingDistance; set => _groundSnappingDistance = Mathf.Clamp(value, 0.0f, float.MaxValue);}
    private float _groundSnappingDistance = 1024.0f;
    /// <summary>
    /// Maximum allowed ground slope angle, all surfaces below or equal to this limit are considered to be ground.
    /// </summary>
    [EditorDisplay("Grounding")]
    [EditorOrder(113)]
    public float MaxSlopeAngle {get => _maxSlopeAngle; set => _maxSlopeAngle = Mathf.Clamp(value, 0.0f, 180.0f);}
    private float _maxSlopeAngle = 66.0f;
    /// <summary>
    /// Determines if stair stepping is allowed at all.
    /// </summary>
    [EditorDisplay("Stairstepping")]
    [EditorOrder(114)]
    public bool AllowStairStepping {get; set;} = true;
    /// <summary>
    /// Maximum allowed stair step height distance.
    /// </summary>
    [EditorDisplay("Stairstepping")]
    [EditorOrder(115)]
    public float StairStepDistance {get => _stairStepDistance; set => _stairStepDistance = Mathf.Clamp(value, 0.0f, float.MaxValue);}
    private float _stairStepDistance = 50.0f;
    /// <summary>
    /// Behavior mode for stair stepping.
    /// </summary>
    [EditorDisplay("Stairstepping")]
    [EditorOrder(116)]
    public StairStepGroundMode StairStepGroundMode {get; set;} = StairStepGroundMode.RequireStableSolid;
    /// <summary>
    /// Minimum distance the character must be able to move forward on a detected step for it to be considered valid.
    /// </summary>
    [EditorDisplay("Stairstepping")]
    [EditorOrder(117)]
    public float StairStepMinimumForwardDistance {get => _stairStepMinimumForwardDistance; set => _stairStepMinimumForwardDistance = Mathf.Clamp(value, 0.0f, float.MaxValue);}
    private float _stairStepMinimumForwardDistance = 0.01f;
    /// <summary>
    /// Maximum amount of stair step iterations per frame.
    /// </summary>
    [EditorDisplay("Stairstepping")]
    [EditorOrder(118)]
    public int MaxStairStepIterations {get => _maxStairStepIterations; set => _maxStairStepIterations = Math.Clamp(value, 0, int.MaxValue);} 
    private int _maxStairStepIterations = 10;
    /// <summary>
    /// Determines if the character should move with RigidBodies it is standing on.
    /// </summary>
    [EditorDisplay("RigidBody interactions")]
    [EditorOrder(119)]
    public RigidBodyMoveMode RigidBodyMoveMode {get; set;} = RigidBodyMoveMode.KinematicMoversOnly;
    /// <summary>
    /// Determines if the character should solve the movements caused by rigidbodies stood upon.
    /// If enabled, the character will sweep the movements, this is more expensive and more unstable but will cause less potential collision issues.
    /// If disabled, the character will not sweep the movements, this is less expensive and more stable but will cause potential collision issues.
    /// </summary>
    [EditorDisplay("RigidBody interactions")]
    [EditorOrder(120)]
    public bool SolveRigidBodyMovements {get; set;} = false;
    /// <summary>
    /// Determine how to handle dynamic rigidbodies that we have collided with.
    /// </summary>
    [EditorDisplay("RigidBody interactions")]
    [EditorOrder(121)]
    public RigidBodyInteractionMode RigidBodyInteractionMode {get; set;} = RigidBodyInteractionMode.PureKinematic;
    private const int KCC_MAX_RB_INTERACTIONS = 1024;
    //Evil optimization global variables, used to track rigidbody interactions
    private static readonly RigidBodyInteraction[] _rigidBodiesCollided = new RigidBodyInteraction[KCC_MAX_RB_INTERACTIONS];
    private static int _rigidBodiesCollidedCount = 0;
    /// <summary>
    /// The simulated mass amount for dynamic rigidbody handling.
    /// </summary>
    [EditorDisplay("RigidBody interactions")]
    [EditorOrder(122)]
    public float SimulatedMass {get => _simulatedMass; set => _simulatedMass = Mathf.Clamp(value, 0.0f, float.MaxValue);}
    private float _simulatedMass = 1000.0f;

    private Vector3 _internalVelocity = Vector3.Zero;
    private Real _internalGravityVelocity = 0.0f;
    /// <summary>
    /// The current gravity as normalized euler angles.
    /// </summary>
    [NoSerialize, HideInEditor] public Vector3 GravityEulerNormalized {get; private set;} = Vector3.Down;
    /// <summary>
    /// Velocity, ignoring movements from RigidBody we stood upon.
    /// </summary>
    [NoSerialize, HideInEditor] public Vector3 KinematicVelocity {get; set;} = Vector3.Zero;
    /// <summary>
    /// Velocity only from rigidbody we stood upon.
    /// </summary>
    [NoSerialize, HideInEditor] public Vector3 KinematicAttachedVelocity {get; set;} = Vector3.Zero;
    /// <summary>
    /// The character's controller.
    /// </summary>
    [NoSerialize, HideInEditor] public IKinematicCharacter? Controller {get; set;} = null;
    /// <summary>
    /// The RigidBody we are attached to.
    /// </summary>
    [NoSerialize, HideInEditor] public RigidBody? AttachedRigidBody => _attachedRigidBody;
    private RigidBody? _attachedRigidBody = null;
    //The amount of maximum colliders flax will ever report back.
    private const int FLAX_PHYSICS_MAX_QUERY = 128;
    //Evil optimization global variable, used to cache collider validity for sorting colliders in OverlapCollider
    private static readonly BitArray _colliderValidities = new(FLAX_PHYSICS_MAX_QUERY, false);

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

		_kccPlugin.Register(this);

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
		_kccPlugin.Unregister(this);

        base.OnDisable();
    }

    /// <summary>
    /// Calculate movement. This should not ever be called directly.
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

        #if FLAX_EDITOR
        Profiler.BeginEvent("Controller.KinematicMoveUpdate");
        #endif

        Controller.KinematicMoveUpdate(out _internalVelocity, out Quaternion _internalOrientation);

        #if FLAX_EDITOR
        Profiler.EndEvent();
        Profiler.BeginEvent("KCC.KinematicUpdate");
        #endif

        TransientOrientation = _internalOrientation;
        GravityEulerNormalized = (Vector3.Down * TransientOrientation).Normalized;

        _internalGravityVelocity = _internalVelocity.Y;
        _internalVelocity *= TransientOrientation;

        _rigidBodiesCollidedCount = 0;

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
        TransientPosition += UnstuckSolve((float)KinematicContactOffset);

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
            }
            else
            {
                KinematicAttachedVelocity = MovementFromRigidBody(AttachedRigidBody, TransientPosition);
                TransientPosition += KinematicAttachedVelocity;
            }
            
            //hack: move upwards by contact offset so that we don't clip into the rigidbody if its swinging wildly
            TransientPosition += -GravityEulerNormalized * KinematicContactOffset;
        }
        else
        {
            KinematicAttachedVelocity = Vector3.Zero;
        }

        //Move to the calculated position so that the next iterating character will be aware of this character's result
        // this hopefully improves stability between character <-> character interactions
        Position = TransientPosition;
        Orientation = TransientOrientation;

        #if FLAX_EDITOR
        Profiler.EndEvent();
        Profiler.BeginEvent("Controller.KinematicPostUpdate");
        #endif

        Controller.KinematicPostUpdate();

        #if FLAX_EDITOR
        Profiler.EndEvent();
        #endif
    }

    /// <summary>
    /// Set the collider actor sizes according to the controller's size properties
    /// </summary>
    private void SetColliderSize()
    {
        ColliderHalfHeight = ColliderHeight / 2.0f;
        ColliderHalfRadius = ColliderRadius / 2.0f;

        _boxExtents.X = ColliderHalfRadius;
        _boxExtents.Z = ColliderHalfRadius;
        _boxExtents.Y = ColliderHalfHeight;
        
        if(_collider == null)
        {
            return;
        }

        _collider.ContactOffset = ColliderContactOffset;
        SetColliderSizeWithInflation(0.0f);
    }

    /// <summary>
    /// Set the size of the collider actor with possible extra inflation value
    /// </summary>
    /// <param name="inflate">Extra size added to the collider size</param>
    /// <exception cref="NotImplementedException">Thrown if unsupported collider type (should never happen)</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetColliderSizeWithInflation(float inflate)
    {
        if(_collider == null)
        {
            return;
        }

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
    /// Return all colliders we are overlapping with in an array.
    /// Will filter if collision filtering is enabled for this character.
    /// Array is also sorted in an unordered sequence where all valid colliders are at the beginning of the array, and all invalid colliders are at the end of the array.
    /// </summary>
    /// <param name="origin">Point in world space to trace at</param>
    /// <param name="colliders"></param>
    /// <param name="layerMask"></param>
    /// <param name="hitTriggers"></param>
    /// <param name="inflate">Extra size added to the collider size</param>
    /// <returns>Last "valid collision" index in the collider array, will be 0 if no collisions happened</returns>
    /// <exception cref="NotImplementedException">Thrown if unsupported collider type (should never happen)</exception>
    public int OverlapCollider(Vector3 origin, out Collider[] colliders, uint layerMask = uint.MaxValue, bool hitTriggers = true, float inflate = 0.0f)
    {
        #if FLAX_EDITOR
        Profiler.BeginEvent("KCC.OverlapCollider");
        #endif

        bool result = false;
        if(!FilterCollisions)
        {
            result = ColliderType switch
            {
                ColliderType.Box => Physics.OverlapBox(origin, BoxExtents + inflate, out colliders, TransientOrientation, layerMask, hitTriggers),
                ColliderType.Capsule => Physics.OverlapCapsule(origin, (float)(ColliderRadius + inflate), (float)(ColliderHeight - ColliderHalfRadius + inflate), out colliders, TransientOrientation * Quaternion.RotationZ(1.57079633f), layerMask, hitTriggers),
                ColliderType.Sphere => Physics.OverlapSphere(origin, (float)(ColliderRadius + inflate), out colliders, layerMask, hitTriggers),
                _ => throw new NotImplementedException(),
            };

            #if FLAX_EDITOR
            Profiler.EndEvent();
            #endif

            return colliders.Length;
        }

        result = ColliderType switch
        {
            ColliderType.Box => Physics.OverlapBox(origin, BoxExtents + inflate, out colliders, TransientOrientation, layerMask, hitTriggers),
            ColliderType.Capsule => Physics.OverlapCapsule(origin, (float)(ColliderRadius + inflate), (float)(ColliderHeight - ColliderHalfRadius + inflate), out colliders, TransientOrientation * Quaternion.RotationZ(1.57079633f), layerMask, hitTriggers),
            ColliderType.Sphere => Physics.OverlapSphere(origin, (float)(ColliderRadius + inflate), out colliders, layerMask, hitTriggers),
            _ => throw new NotImplementedException(),
        };

        if(!result)
        {
            #if FLAX_EDITOR
            Profiler.EndEvent();
            #endif

            return 0;
        }

        //first check the collider validity and cache it so we don't cause overhead from function calls
        for(int i = 0; i < colliders.Length; i++)
        {
            _colliderValidities[i] = IsColliderValid(colliders[i]);
        }

        //sort collider array so that all valid colliders are in unordered sequence
        int lastValidIndex = 0;
        for(int a = 0; a < colliders.Length; a++)
        {
            //what we have is already ok, continue on
            if(_colliderValidities[a])
            {
                lastValidIndex++;
                continue;
            }

            //this is not valid, see if we have anything valid ahead of us that we can swap with.
            for(int b = a + 1; b < colliders.Length; b++)
            {
                if(!_colliderValidities[b])
                {
                    //early exit the sort in case we have boatloads of invalids at the end, no point trying to sort them when there is nothing to sort.
                    if(b + 1 == colliders.Length)
                    {
                        goto earlyExitFromSort;
                    }

                    continue;
                }
                
                //tuple swap not used here to avoid allocating a ValueTuple<T,T>
                Collider tempCollider = colliders[a];
                colliders[a] = colliders[b];
                colliders[b] = tempCollider;
                bool tempValidity = _colliderValidities[a];
                _colliderValidities[a] = _colliderValidities[b];
                _colliderValidities[b] = tempValidity;
                lastValidIndex++;
                break;
			}
		}

        earlyExitFromSort:
        #if FLAX_EDITOR
        Profiler.EndEvent();
        #endif

        return lastValidIndex;
    }

    /// <summary>
    /// Return all colliders collided with by the cast.
    /// Will filter if collision filtering is enabled for this character.
    /// </summary>
    /// <param name="origin">Point in world space to trace from</param>
    /// <param name="direction"></param>
    /// <param name="trace"></param>
    /// <param name="distance"></param>
    /// <param name="layerMask"></param>
    /// <param name="hitTriggers"></param>
    /// <returns>True if collided with anything</returns>
    /// <exception cref="NotImplementedException">Thrown if unsupported collider type (should never happen)</exception>
    public bool CastCollider(Vector3 origin, Vector3 direction, out RayCastHit trace, Real distance = Real.MaxValue, uint layerMask = uint.MaxValue, bool hitTriggers = true)
    {
        #if FLAX_EDITOR
        Profiler.BeginEvent("KCC.CastCollider");
        #endif

        if(Controller is null)
        {
            #if FLAX_EDITOR
            Debug.LogError("IKinematicCharacter controller is missing", this);
            #endif

            trace = new()
            {
                Distance = (float)distance,
            };

            #if FLAX_EDITOR
            Profiler.EndEvent();
            #endif

            return false;
        }

        #if FLAX_EDITOR
        if(!direction.IsNormalized)
        {
            Debug.LogError($"CastCollider direction is not normalized! {direction}", this);
        }

        if(DebugIsSelected())
        {
            DebugDraw.DrawWireArrow(origin, Quaternion.FromDirection(direction), (float)distance*0.01f, 1.0f, Color.Yellow, Time.DeltaTime, false);
        }
        #endif

        bool result;
        if(!FilterCollisions)
        {
            result = ColliderType switch
            {
                ColliderType.Box => Physics.BoxCast(origin, BoxExtents, direction, out trace, TransientOrientation, (float)distance, layerMask, hitTriggers),
                ColliderType.Capsule => Physics.CapsuleCast(origin, ColliderRadius, ColliderHeight - ColliderHalfRadius, direction, out trace, TransientOrientation * Quaternion.RotationZ(1.57079633f), (float)distance, layerMask, hitTriggers),
                ColliderType.Sphere => Physics.SphereCast(origin, ColliderRadius, direction, out trace, (float)distance, layerMask, hitTriggers),
                _ => throw new NotImplementedException(),
            };

            if(!result)
            {
                trace.Distance = (float)distance;
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

            #if FLAX_EDITOR
            Profiler.EndEvent();
            #endif

            return result;
        }

        #pragma warning disable IDE0018
        RayCastHit[] traces;
        #pragma warning restore IDE0018 
        result = ColliderType switch
        {
            ColliderType.Box => Physics.BoxCastAll(origin, BoxExtents, direction, out traces, TransientOrientation, (float)distance, layerMask, hitTriggers),
            ColliderType.Capsule => Physics.CapsuleCastAll(origin, ColliderRadius, ColliderHeight - ColliderHalfRadius, direction, out traces, TransientOrientation * Quaternion.RotationZ(1.57079633f), (float)distance, layerMask, hitTriggers),
            ColliderType.Sphere => Physics.SphereCastAll(origin, ColliderRadius, direction, out traces, (float)distance, layerMask, hitTriggers),
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
                Distance = (float)distance,
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

        #if FLAX_EDITOR
        Profiler.EndEvent();
        #endif

        return result;
    }
    
    /// <summary>
    /// Check if the other collider should be ignored.
    /// </summary>
    /// <param name="collider"></param>
    /// <returns>False if should be ignored, True if should be considered</returns>
    private bool IsColliderValid(Collider collider)
    {
        if(_collider == null)
        {
            #if FLAX_EDITOR
            Debug.LogError("KinematicCharacterController collider is missing", this);
            #endif

            return false;
        }

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

    /// <summary>
    /// Try to register a new unique rigidbody interaction.
    /// Will be rejected if the interaction is already registered to prevent duplicate interactions from happening.
    /// </summary>
    /// <param name="trace"></param>
    /// <param name="rigidBody"></param>
    private void TryAddRigidBodyInteraction(RayCastHit trace, RigidBody rigidBody)
    {
        //only allow non-KCC rigidbodies for now
        if(rigidBody is KinematicCharacterController)
        {
            return;
        }

        if(rigidBody.IsKinematic)
        {
            return;
        }

        if(_rigidBodiesCollidedCount >= KCC_MAX_RB_INTERACTIONS)
        {
            #if FLAX_EDITOR
            Debug.LogWarning($"Maximum RigidBody interactions reached! (have: {_rigidBodiesCollidedCount}, limit: {KCC_MAX_RB_INTERACTIONS})", this);
            #endif

            return;
        }

        for(int i = 0; i < _rigidBodiesCollidedCount; i++)
        {
            if(_rigidBodiesCollided[i].RigidBody == rigidBody)
            {
                return;
            }
        }

        _rigidBodiesCollided[_rigidBodiesCollidedCount].RigidBody = rigidBody;
        _rigidBodiesCollided[_rigidBodiesCollidedCount].Point = trace.Point;
        _rigidBodiesCollided[_rigidBodiesCollidedCount].Normal = trace.Normal;
        _rigidBodiesCollided[_rigidBodiesCollidedCount].CharacterVelocity = _internalVelocity;
        _rigidBodiesCollided[_rigidBodiesCollidedCount].BodyVelocity = rigidBody.LinearVelocity;
        _rigidBodiesCollidedCount++;
    }

    /// <summary>
    /// The main solver, this will move the character as long as there is velocity left, and we haven't gone over 3 collisions in this sweep.
    /// </summary>
    private void SolveSweep()
    {
        #if FLAX_EDITOR
        Profiler.BeginEvent("KCC.SolveSweep");
        #endif

        Vector3 originalVelocityNormalized = _internalVelocity.Normalized;
        int unstuckSolves = 0;

        //we can realistically only collide with 2 planes before we lose all degrees of freedom (intersection of three planes is a point)
        Vector3 firstPlane = Vector3.Zero;
        for(int i = 0; i < 3; i++)
        {
            if(_internalVelocity.IsZero)
            {
                #if FLAX_EDITOR
                Profiler.EndEvent();
                #endif

                return;
            }

            //are we about to go backwards? (unwanted direction, fixes issues with jiggling in corners with obtuse angles)
            if(Math.Round(Vector3.Dot(originalVelocityNormalized, _internalVelocity.Normalized), 4, MidpointRounding.ToZero) < 0.0f)
            {
                #if FLAX_EDITOR
                Profiler.EndEvent();
                #endif

                return;
            }
            
            if(!CastCollider(TransientPosition, _internalVelocity.Normalized, out RayCastHit trace, _internalVelocity.Length + KinematicContactOffset, CollisionMask, false))
            {
                #if FLAX_EDITOR
                if(DebugIsSelected())
                {
                    DebugDrawCollider(TransientPosition + _internalVelocity, TransientOrientation, Color.Blue, Time.DeltaTime, false);
                    DebugDraw.DrawWireArrow(TransientPosition, Quaternion.FromDirection(_internalVelocity.Normalized), (float)_internalVelocity.Length * 0.01f, 1.0f, Color.Blue, Time.DeltaTime, false);
                }
                #endif

                //no collision, full speed ahead!
                TransientPosition += _internalVelocity;

                #if FLAX_EDITOR
                Profiler.EndEvent();
                #endif

                return;
            }

            if(trace.Distance == 0.0f && unstuckSolves < MaxUnstuckIterations)
            {
                //trace collided with zero distance?
                //trace must have started inside something, so we're most likely stuck.
                //try to solve the issue with inflated collider and re-try sweep.
                TransientPosition += UnstuckSolve(0.0f);
                i--;
                unstuckSolves++;
                continue;
            }

            //pull back a bit, otherwise we would be constantly intersecting with the plane
            Real distance = Math.Max(trace.Distance - KinematicContactOffset, 0.0f);

            #if FLAX_EDITOR
            if(DebugIsSelected())
            {
                DebugDrawCollider(TransientPosition + (_internalVelocity.Normalized * distance), TransientOrientation, Color.Blue, Time.DeltaTime, false);
                DebugDraw.DrawWireArrow(TransientPosition, Quaternion.FromDirection(_internalVelocity.Normalized), (float)distance*0.01f, 1.0f, Color.Blue, Time.DeltaTime, false);
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
                Real creaseDistance = Vector3.Dot(wantedVelocity, crease);

                #if FLAX_EDITOR
                if(DebugIsSelected())
                {
                    DebugDraw.DrawLine(TransientPosition + (crease * 64), TransientPosition - (crease * 64), Color.Purple, Time.DeltaTime, false);
                }
                #endif

                //consider anything less than 90 deg to be acute, and anything above to be obtuse.
                bool isAcute = Math.Round(Vector3.Dot(firstPlane, trace.Normal), 4, MidpointRounding.ToZero) < 0.0f;

                //obtuse corners need extra handling, least we want the controller to get snagged in them.
                if(!isAcute)
                {
                    Vector3 averagePlane = (trace.Normal + firstPlane).Normalized;

                    //also nudge by both planes in hopes of pushing out of the corner, similar to how quake3 handles this.
                    //normally the surrounding code would fix the issue, however it is not enough to solve vertical movement in obtuse corners
                    //so this is needed, sadly this does introduce slight jiggling in some obtuse corners :( but it's better than getting stuck.
                    _internalVelocity += averagePlane; 
                    
                    if(Math.Round(Vector3.Dot(_internalVelocity.Normalized, GravityEulerNormalized), 4, MidpointRounding.ToZero) > 0.0f)
                    {
                        TransientPosition += averagePlane * 0.1f;
                    }
                }

                //constrain to crease
                _internalVelocity = Vector3.ProjectOnPlane(crease, trace.Normal) * creaseDistance;
            }

            //also slow down depending on the angle of hit plane (and physics material if enabled)
            _internalVelocity *= (1.0f - Math.Abs(Vector3.Dot(_internalVelocity.Normalized, trace.Normal))) * SlideMultiplier;
            if(SlideAccountForPhysicsMaterial && trace.Material is not null)
            {
                _internalVelocity *= 1.0f - trace.Material.Friction;
            }
        }

        #if FLAX_EDITOR
        Profiler.EndEvent();
        #endif
    }

    /// <summary>
    /// Solve all stair steps for the remaining velocity as long as valid stairs are found.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="velocity"></param>
    /// <param name="distance"></param>
    /// <param name="sweepNormal"></param>
    private void SolveStairSteps(ref Vector3 position, ref Vector3 velocity, ref Real distance, ref Vector3 sweepNormal)
    {
        if(!AllowStairStepping || AttachedRigidBody is not null)
        {
            return;
        }

        #if FLAX_EDITOR
        Profiler.BeginEvent("KCC.SolveStairSteps");
        #endif

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

        #if FLAX_EDITOR
        Profiler.EndEvent();
        #endif
    }

    /// <summary>
    /// Attempt to do a single stair step for the given velocity.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="velocity"></param>
    /// <param name="distance"></param>
    /// <param name="sweepNormal"></param>
    /// <returns>True if succeeded in stepping a stair</returns>
    private bool SolveStairStep(ref Vector3 position, ref Vector3 velocity, ref Real distance, ref Vector3 sweepNormal)
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

        Real temporaryDistance = Math.Max(trace.Distance - KinematicContactOffset, 0.0f);
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
        
        //move to a possible new wall collision position
        temporaryPosition += remainingVelocityNormalized * temporaryDistance;
        //can we stand on this? if so, then also snap to the floor.
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

    /// <summary>
    /// Calculate the movement from a RigidBody relative to this character, as if it was this character's parent.
    /// </summary>
    /// <param name="rigidBody"></param>
    /// <param name="position"></param>
    /// <returns>The relative movement</returns>
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
    /// Force the character to unground.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ForceUnground()
    {
        _forceUnground = true;
    }

    /// <summary>
    /// Trace to the ground and snap to it if necessary.
    /// </summary>
    /// <returns>Result for the ground standing upon, null if not touching any valid ground or ground at all</returns>
    private RayCastHit? SolveGround()
    {
        #if FLAX_EDITOR
        Profiler.BeginEvent("KCC.SolveGround");
        #endif

        if(_forceUnground)
        {
            AttachToRigidBody(null);
            IsGrounded = false;
            GroundNormal = -GravityEulerNormalized;

            #if FLAX_EDITOR
            Profiler.EndEvent();
            #endif

            return null;
        }

        if(!CanGround)
        {
            AttachToRigidBody(null);
            IsGrounded = false;
            GroundNormal = -GravityEulerNormalized;

            #if FLAX_EDITOR
            Profiler.EndEvent();
            #endif

            return null;
        }

        //no point grounding if not going downwards (this prevents the controller from grounding during forced unground jumps)
        if(!IsGrounded && _internalGravityVelocity > 0)
        {
            #if FLAX_EDITOR
            Profiler.EndEvent();
            #endif

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

        #if FLAX_EDITOR
        Profiler.EndEvent();
        #endif

        return groundTrace;
    }

    /// <summary>
    /// Do a ground trace check, considering if the ground is stable.
    /// Will also attempt to attach the character to a rigidbody if standing on one.
    /// </summary>
    /// <param name="distance"></param>
    /// <returns>Result if stable ground, null otherwise</returns>
    private RayCastHit? GroundCheck(float distance)
    {
        #if FLAX_EDITOR
        Profiler.BeginEvent("KCC.GroundCheck");
        #endif

        IsGrounded = CastCollider(TransientPosition, GravityEulerNormalized, out RayCastHit trace, distance + KinematicContactOffset, CollisionMask, false);
        if(!IsGrounded)
        {
            AttachToRigidBody(null);
            GroundNormal = -GravityEulerNormalized;

            #if FLAX_EDITOR
            Profiler.EndEvent();
            #endif

            return null;
        }

        if(!IsNormalStableGround(trace.Normal))
        {
            AttachToRigidBody(null);
            IsGrounded = false;
            GroundNormal = -GravityEulerNormalized;

            #if FLAX_EDITOR
            Profiler.EndEvent();
            #endif

            return null;
        }

        if(GroundTag.Index != 0 && !trace.Collider.HasTag(GroundTag))
        {
            AttachToRigidBody(null);
            IsGrounded = false;
            GroundNormal = -GravityEulerNormalized;

            #if FLAX_EDITOR
            Profiler.EndEvent();
            #endif

            return null;
        }

        GroundNormal = trace.Normal;
        AttachToRigidBody(trace.Collider.AttachedRigidBody);

        #if FLAX_EDITOR
        Profiler.EndEvent();
        #endif

        return trace;
    }

    /// <summary>
    /// Forcibly move the character to ground level, ignoring if its standable or not.
    /// </summary>
    private void SnapToGround()
    {
        #if FLAX_EDITOR
        Profiler.BeginEvent("KCC.SnapToGround");
        #endif

        if(!IsGrounded)
        {
            #if FLAX_EDITOR
            Profiler.EndEvent();
            #endif

            return;
        }

        if(!CastCollider(TransientPosition, GravityEulerNormalized, out RayCastHit trace, GroundSnappingDistance + KinematicContactOffset, CollisionMask, false))
        {
            #if FLAX_EDITOR
            Profiler.EndEvent();
            #endif

            return;
        }

        TransientPosition += GravityEulerNormalized * Math.Max(trace.Distance - KinematicContactOffset, 0.0f);

        #if FLAX_EDITOR
        Profiler.EndEvent();
        #endif
    }

    /// <summary>
    /// Is the normal vector considered stable ground?
    /// </summary>
    /// <param name="normal"></param>
    /// <returns>True if stable</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsNormalStableGround(Vector3 normal)
    {
        return Vector3.Angle(-GravityEulerNormalized, normal) <= MaxSlopeAngle;
    }

    /// <summary>
    /// Calculates the necessary vector3 to move out of collisions
    /// </summary>
    /// <param name="inflate">Extra size added to the collider size</param>
    /// <returns>Amount to push out by so that the character is no longer colliding with anything</returns>
    public Vector3 UnstuckSolve(float inflate)
    {
        #if FLAX_EDITOR
        Profiler.BeginEvent("KCC.UnstuckSolve");
        #endif

        if(Controller is null)
        {
            #if FLAX_EDITOR
            Debug.LogError("IKinematicCharacter controller is missing", this);
            Profiler.EndEvent();
            #endif

            return Vector3.Zero;
        }

        if(_collider == null)
        {
            #if FLAX_EDITOR
            Debug.LogError("KinematicCharacterController collider is missing", this);
            Profiler.EndEvent();
            #endif

            return Vector3.Zero;
        }

        int overlaps = 0;
        if((overlaps = OverlapCollider(TransientPosition, out Collider[] colliders, CollisionMask, false, inflate)) == 0)
        {
            #if FLAX_EDITOR
            Profiler.EndEvent();
            #endif

            return Vector3.Zero;
        }

        //we're inside something, so calculate a vector that would push us out of everything we are currently colliding with.
        Vector3 requiredPush = Vector3.Zero;

        //need inflate the colliders a bit for the ComputePenetration, as the collider's contact offset is ignored
        SetColliderSizeWithInflation(inflate);
        for(int i = 0; i < overlaps; i++)
        {
            if(!Collider.ComputePenetration(_collider, colliders[i], out Vector3 penetrationDirection, out float penetrationDistance))
            {
                continue; 
            }

            Controller.KinematicUnstuckEvent(colliders[i], penetrationDirection, penetrationDistance);
            requiredPush += penetrationDirection * penetrationDistance;
        }

        SetColliderSizeWithInflation(0.0f);
        
        #if FLAX_EDITOR
        Profiler.EndEvent();
        #endif

        return requiredPush;
    }

    /// <summary>
    /// Utility function to project a direction along the ground normal without any lateral movement (accounts for gravity direction)
    /// </summary>
    /// <param name="direction">Normalized direction</param>
    /// <returns>Projected normalized direction</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 GroundTangent(Vector3 direction)
    {
        Vector3 right = Vector3.Cross(direction, -GravityEulerNormalized) ;
        return Vector3.Cross(GroundNormal, right).Normalized;
    }

    /// <summary>
    /// Process all registered rigidbody collisions from the sweep step.
    /// Depending on interaction mode this will cause impacts on dynamic rigidbodies.
    /// </summary>
    private void SolveRigidBodyInteractions()
    {
        #if FLAX_EDITOR
        Profiler.BeginEvent("KCC.SolveRigidBodyInteractions");
        #endif

        if(RigidBodyInteractionMode == RigidBodyInteractionMode.None)
        {
            #if FLAX_EDITOR
            Profiler.EndEvent();
            #endif

            return;
        }

        for(int i = 0; i < _rigidBodiesCollidedCount; i++)
        {
            if(_rigidBodiesCollided[i].RigidBody == AttachedRigidBody)
            {
                continue;
            }

            if(RigidBodyInteractionMode == RigidBodyInteractionMode.Manual)
            {
                if(Controller is null)
                {
                    #if FLAX_EDITOR
                    Debug.LogError("IKinematicCharacter controller is missing", this);
                    Profiler.EndEvent();
                    #endif
                    
                    return;
                }
                
                Controller.KinematicRigidBodyInteraction(_rigidBodiesCollided[i]);
                continue;
            }

            float massRatio = 1.0f;
            if(RigidBodyInteractionMode == RigidBodyInteractionMode.SimulateKinematic)
            {
                massRatio = SimulatedMass / (SimulatedMass + _rigidBodiesCollided[i].RigidBody.Mass);
            }

            Vector3 force = Vector3.ProjectOnPlane(_rigidBodiesCollided[i].CharacterVelocity, GravityEulerNormalized) * SimulatedMass;
            _rigidBodiesCollided[i].RigidBody.WakeUp();
            _rigidBodiesCollided[i].RigidBody.AddForceAtPosition(force * massRatio, _rigidBodiesCollided[i].Point, ForceMode.Impulse);
        }

        #if FLAX_EDITOR
        Profiler.EndEvent();
        #endif
    }

    /// <summary>
    /// Attempt to attach this controller to a RigidBody (so that it follows its velocity and rotation).
    /// </summary>
    /// <param name="rigidBody"></param>
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool DebugIsSelected()
    {
        if(!IsDebugDrawEnabled())
        {
            return false;
        } 

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

        if(IsDebugDrawEnabled())
        {
            DebugDraw.DrawWireArrow(TransientPosition, TransientOrientation, 1.0f, 1.0f, Color.GreenYellow, 0.0f, false);
            DebugDraw.DrawWireArrow(TransientPosition, Quaternion.FromDirection(_internalVelocity.Normalized), (float)_internalVelocity.Length*0.01f, 1.0f, Color.YellowGreen, 0.0f, false);
        }
    }

    /// <summary>
    /// Draw this KinematicCharacterController's collider, EDITOR ONLY
    /// </summary>
    /// <param name="position">World position</param>
    /// <param name="orientation">World orientation</param>
    /// <param name="color">Color</param>
    /// <param name="time">Draw time</param>
    /// <param name="depthTest">Depth test</param>
    /// <exception cref="NotImplementedException">Thrown if unsupported collider type (should never happen)</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DebugDrawCollider(Vector3 position, Quaternion orientation, Color color, float time, bool depthTest)
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DebugDrawBox(Vector3 position, Quaternion orientation, Color color, float time, bool depthTest)
    {
        Matrix matrix = Matrix.CreateWorld(position, Vector3.Forward * orientation, Vector3.Up * orientation);
        DebugDraw.DrawWireBox(new OrientedBoundingBox(
            new Vector3(ColliderHalfRadius, -ColliderHalfHeight, -ColliderHalfRadius), matrix),
            color, time, depthTest);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DebugDrawCapsule(Vector3 position, Quaternion orientation, Color color, float time, bool depthTest)
    {
        //for some reason, this is rotated by 90 degrees unlike other debug draws..
        Quaternion fixedOrientation = orientation * Quaternion.RotationX(1.57079633f);
        DebugDraw.DrawWireCapsule(position, fixedOrientation, ColliderRadius, ColliderHeight, color, time, depthTest);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DebugDrawSphere(Vector3 position, Color color, float time, bool depthTest)
    {
        DebugDraw.DrawWireSphere(new(position, ColliderRadius), color, time, depthTest);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsDebugDrawEnabled()
    {
        return _kccPlugin is not null && _kccPlugin.KCCSettingsInstance != null && _kccPlugin.KCCSettingsInstance.DebugDisplay;
    }
    #endif
}