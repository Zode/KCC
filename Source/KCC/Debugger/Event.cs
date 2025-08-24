#if FLAX_EDITOR
using System.Collections.Generic;
using FlaxEngine;

namespace KCC.Debugger;
#nullable enable

/// <summary>
/// Represents a KCC Debugger event.
/// </summary>
public class Event(Actor actor, string name)
{
	/// <summary>
	/// The associated Flax actor.
	/// </summary>
	public Actor Actor {get; set;} = actor;
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
	/// Render all the renderables from this event and its sub events.
	/// </summary>
	public void Render()
	{
		foreach(Renderable renderable in Renderables)
		{
			renderable.Render();
		}

		foreach(Event @event in Events)
		{
			@event.Render();
		}
	}
}
#endif