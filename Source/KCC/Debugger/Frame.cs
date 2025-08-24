#if FLAX_EDITOR
using System.Collections.Generic;

namespace KCC.Debugger;

/// <summary>
/// Represents a KCC Debugger frame.
/// </summary>
public class Frame
{
	/// <summary>
	/// List of debugger events.
	/// </summary>
	public List<Event> Events {get; set;} = [];
}
#endif