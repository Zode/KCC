using FlaxEditor.GUI.Tree;

namespace KCC.Debugger;

/// <inheritdoc />
public class EventNode : TreeNode
{
	/// <summary>
	/// The associated KCC Debugger event.
	/// </summary>
	public Event Event {get; set;}

	/// <summary>
	/// Initializes a new instance of the <see cref="EventNode" /> class.
	/// </summary>
	/// <param name="event"></param>
	public EventNode(Event @event)
	: base(false)
	{
		Event = @event;
	}

	/// <summary>
	/// Process the subevents inside the <see cref="Event" />, and make them into <see cref="EventNode" /> for the <see cref="Tree" />.
	/// </summary>
	public void ConstructChildren()
	{
		foreach(Event @event in Event.Events)
		{
			EventNode node = new(@event)
			{
				Text = @event.Name,
				Parent = this,
			};
			
			node.ConstructChildren();
		}
	}
}