#if FLAX_EDITOR
using System.Collections.Generic;
using FlaxEngine;

namespace KCC.Debugger;

/// <summary>
/// Represents a KCC Debugger frame.
/// </summary>
[HideInEditor]
public class Frame
{
	/// <summary>
	/// List of debugger events.
	/// </summary>
	public List<Event> Events {get; set;} = [];

	/// <summary>
	/// Reset the pass flag on events
	/// </summary>
	public void ResetRenderables()
	{
		foreach(Event @event in Events)
		{
			@event.ResetRenderables();
		}
	}
}
#endif