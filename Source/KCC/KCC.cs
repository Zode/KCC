using System;
using FlaxEngine;
using System.Collections.Generic;

#if FLAX_EDITOR
using FlaxEditor;
#endif

namespace KCC;
#nullable enable

/// <summary>
/// </summary>
/// <seealso cref="FlaxEngine.GamePlugin" />
public class KCC : GamePlugin
{
    private KCCSettings? _kccSettings = null;
    private List<KinematicMover> _kinematicMovers = [];
    private List<KinematicCharacterController> _kinematicCharacters = [];
    private float _interpolationDeltaTime = 0.0f;
    private float _interpolationStartTime = 0.0f;
    /// <summary>
    /// This event is fired before any simulation or pre-simulation interpolation setup happens, before all KCC actors
    /// </summary>
    public event Action? PreSimulationUpdateEvent;
    /// <summary>
    /// This event is fired when the simulation happens, before all KCC actors
    /// </summary>
    public event Action? SimulationUpdateEvent;
    /// <summary>
    /// This event is fired after the simulation and post-simulation interpolation setup has happened, after all KCC actors
    /// </summary>
    public event Action? PostSimulationUpdateEvent;

    /// <inheritdoc />
    public KCC()
    {
        _description = new PluginDescription
        {
            Name = "KCC",
            Category = "Other",
            Author = "Zode",
            AuthorUrl = null,
            HomepageUrl = null,
            RepositoryUrl = "https://github.com/Zode/KCC",
            Description = "Kinematic Character Controller",
            Version = new Version(1, 0, 2),
            IsAlpha = false,
            IsBeta = false,
        };
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        base.Initialize();
       
        JsonAsset kccSettingsJson = Engine.GetCustomSettings("KCC Settings");
        if(kccSettingsJson == null)
        {
            #if FLAX_EDITOR
            Debug.LogError("Missing KCC settings");
            #endif
            return;
        }

        Scripting.LateUpdate += OnLateUpdate;
        Scripting.FixedUpdate += OnFixedUpdate;
        Scripting.Update += OnUpdate;

        _kccSettings = kccSettingsJson.CreateInstance<KCCSettings>();
        _kinematicMovers = new(_kccSettings.MoverInitialCapacity);
        _kinematicCharacters = new(_kccSettings.CharacterInitialCapacity);
    }

    /// <inheritdoc />
    public override void Deinitialize()
    {
        Scripting.Update -= OnUpdate;
        Scripting.FixedUpdate -= OnFixedUpdate;
        Scripting.LateUpdate -= OnLateUpdate;
        base.Deinitialize();
    }

    /// <inheritdoc />
    public void OnLateUpdate()
    {
        if(_kccSettings is null ||
            !_kccSettings.Interpolate ||
            _kccSettings.InterpolationMode != InterpolationMode.LateUpdate)
        {
            return;
        }

        InterpolationUpdate();
    }

    /// <inheritdoc />
    public void OnUpdate()
    {
        if(_kccSettings is null ||
            !_kccSettings.Interpolate ||
            _kccSettings.InterpolationMode != InterpolationMode.Update)
        {
            return;
        }

        InterpolationUpdate();
    }

    /// <inheritdoc />
    public void OnFixedUpdate()
    {
        if(_kccSettings is null || !_kccSettings.AutoSimulation)
        {
            return;
        }

        //don't bother processing when game is paused
        //also fixes an issue where CastCollider would be fed a non-normalized direction as result of game being paused
        if(Time.TimeScale == 0.0f)
        {
            return;
        }

        PreSimulationUpdate();
        SimulationUpdate();
        PostSimulationUpdate();
    }

    /// <summary>
    /// Register a kinematic mover to the simulaton
    /// </summary>
    /// <param name="mover"></param>
    public void Register(KinematicMover mover)
    {
        #if FLAX_EDITOR
        if(!Editor.IsPlayMode)
        {
            return;
        }
        #endif

        _kinematicMovers.Add(mover);
    }

    /// <summary>
    /// Register a kinematic character to the simulaton
    /// </summary>
    /// <param name="character"></param>
    public void Register(KinematicCharacterController character)
    {
        #if FLAX_EDITOR
        if(!Editor.IsPlayMode)
        {
            return;
        }
        #endif

        _kinematicCharacters.Add(character);
    }

    /// <summary>
    /// Unregister a kinematic mover from the simulation
    /// </summary>
    /// <param name="mover"></param>
    public void Unregister(KinematicMover mover)
    {
        #if FLAX_EDITOR
        if(!Editor.IsPlayMode)
        {
            return;
        }
        #endif

        _kinematicMovers.Remove(mover);
    }

    /// <summary>
    /// Unregister a kinematic character from the simulation
    /// </summary>
    /// <param name="character"></param>
    public void Unregister(KinematicCharacterController character)
    {
        #if FLAX_EDITOR
        if(!Editor.IsPlayMode)
        {
            return;
        }
        #endif

        _kinematicCharacters.Remove(character);
    }

    /// <summary>
    /// Saves necessary info for interpolation before the simulation
    /// </summary>
    public void PreSimulationUpdate()
    {
        PreSimulationUpdateEvent?.Invoke();

        foreach(KinematicMover mover in _kinematicMovers)
        {
            mover.InitialPosition = mover.TransientPosition;
            mover.InitialOrientation = mover.TransientOrientation;

            mover.Position = mover.TransientPosition;
            mover.Orientation = mover.TransientOrientation;
        }

        foreach(KinematicCharacterController character in _kinematicCharacters)
        {
            character.InitialPosition = character.TransientPosition;
            character.InitialOrientation = character.TransientOrientation;

            character.Position = character.TransientPosition;
            character.Orientation = character.TransientOrientation;
        }
    }

    /// <summary>
    /// Tick simulation
    /// </summary>
    public void SimulationUpdate()
    {
        SimulationUpdateEvent?.Invoke();

        foreach(KinematicMover mover in _kinematicMovers)
        {
            mover.KinematicUpdate();
        }

        foreach(KinematicCharacterController character in _kinematicCharacters)
        {
            character.KinematicUpdate();
        }
    }

    /// <summary>
    /// Sets up for interpolation after the simulation
    /// </summary>
    public void PostSimulationUpdate()
    {
        _interpolationDeltaTime = Time.DeltaTime;
        _interpolationStartTime = Time.TimeSinceStartup;

        if(_kccSettings is null ||
            !_kccSettings.Interpolate)
        {
            foreach(KinematicMover mover in _kinematicMovers)
            {
                mover.Position = mover.TransientPosition;
                mover.Orientation = mover.TransientOrientation;
            }

            foreach(KinematicCharacterController character in _kinematicCharacters)
            {
                character.Position = character.TransientPosition;
                character.Orientation = character.TransientOrientation;
            }

            PostSimulationUpdateEvent?.Invoke();
            return;
        }

        foreach(KinematicMover mover in _kinematicMovers)
        {
            mover.Position = mover.InitialPosition;
            mover.Orientation = mover.InitialOrientation;
        }

        foreach(KinematicCharacterController character in _kinematicCharacters)
        {
            character.Position = character.InitialPosition;
            character.Orientation = character.InitialOrientation;
        }

        PostSimulationUpdateEvent?.Invoke();
    }

    /// <summary>
    /// Processes per frame interpolation
    /// </summary>
    public void InterpolationUpdate()
    {
        if(_interpolationDeltaTime <= 0.0f)
        {
            return;
        }

        float factor = Mathf.Clamp((Time.TimeSinceStartup - _interpolationStartTime) / _interpolationDeltaTime, 0.0f, 1.0f);
        foreach(KinematicMover mover in _kinematicMovers)
        {
            mover.Position = Vector3.Lerp(mover.InitialPosition, mover.TransientPosition, factor);
            mover.Orientation = Quaternion.Slerp(mover.InitialOrientation, mover.TransientOrientation, factor);
        }

        foreach(KinematicCharacterController character in _kinematicCharacters)
        {
            character.Position = Vector3.Lerp(character.InitialPosition, character.TransientPosition, factor);
            character.Orientation = Quaternion.Slerp(character.InitialOrientation, character.TransientOrientation, factor);
        }
    }
}
