#if FLAX_EDITOR
using System;
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
	/// Time taken during this frame
	/// </summary>
	public TimeSpan Time {get; set;} = TimeSpan.Zero;

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

	/// <summary>
	/// Calculate and cache the total time taken during this frame
	/// </summary>
	public void CalculateTime()
	{
		foreach(Event @event in Events)
		{
			Time += @event.Timer.Elapsed;
		}
	}
}
#endif