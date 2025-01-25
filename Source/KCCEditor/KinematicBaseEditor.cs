using FlaxEditor.CustomEditors;
using FlaxEditor.CustomEditors.Dedicated;
using FlaxEditor.CustomEditors.Elements;
using FlaxEngine;

namespace KCC;

/// <summary>
/// Custom editor for KinematicBase
/// </summary>
[CustomEditor(typeof(KinematicBase))]
public class KinematicBaseEditor : ActorEditor
{
	/// <inheritdoc /> 
	public override void Initialize(LayoutElementsContainer layout)
	{
		base.Initialize(layout);

		for(int i = 0; i < layout.Children.Count; i++)
		{
			if(layout.Children[i] is GroupElement group && group.Panel.HeaderText.Equals("Rigid Body", System.StringComparison.Ordinal))
			{
				layout.Children.Remove(group);
				layout.ContainerControl.Children.Remove(group.Panel);
				break;
			}
		}
	}
}