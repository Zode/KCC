#if FLAX_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using FlaxEngine;

namespace KCC.Debugger;

/// <summary>
/// Represents a KCC Debugger event.
/// </summary>
[HideInEditor]
public class Event(Guid? id, string name)
{
	/// <summary>
	/// The associated Flax actor. If set this is the "topmost" event for this actor,
	/// used for synching selection between scene and debugger.
	/// </summary>
	public Guid? ActorID {get; set;} = id;
	/// <summary>
	/// Child events (if any).
	/// </summary>
	public List<Event> Events {get; set;} = [];
	/// <summary>
	/// Event name.
	/// </summary>
	public string Name {get; set;} = name;
	/// <summary>
	/// List of renderables.
	/// </summary>
	public List<Renderable> Renderables {get; set;} = [];
	/// <summary>
	/// Was this already rendered during this pass?
	/// </summary>
	public bool AlreadyRendered {get; set;} = false;
	/// <summary>
	/// Timer for this event
	/// </summary>
	public Stopwatch Timer {get; set;} = new();

	/// <summary>
	/// Render all the renderables from this event and its sub events.
	/// </summary>
	public void Render(bool skipChild = false)
	{
		if(AlreadyRendered)
		{
			return;
		}

		AlreadyRendered = true;
		foreach(Renderable renderable in Renderables)
		{
			renderable.Render();
		}

		if(skipChild)
		{
			return;
		}

		foreach(Event @event in Events)
		{
			@event.Render();
		}
	}

	/// <summary>
	/// Reset the pass flag on this event and its subevents
	/// </summary>
	public void ResetRenderables(bool skipChild = false)
	{
		AlreadyRendered = false;

		if(skipChild)
		{
			return;
		}
		
		foreach(Event @event in Events)
		{
			@event.ResetRenderables();
		}
	}

	/// <summary>
	/// Calculate the total time from subevents only
	/// </summary>
	/// <returns></returns>
	public TimeSpan CalculateSubeventTime()
	{
		TimeSpan result = TimeSpan.Zero;
		foreach(Event @event in Events)
		{
			result += @event.Timer.Elapsed;
		}

		return result;
	}
}
#endif