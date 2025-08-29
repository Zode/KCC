using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FlaxEditor;
using FlaxEditor.GUI;
using FlaxEditor.GUI.Tree;
using FlaxEditor.SceneGraph;
using FlaxEditor.Surface.Archetypes;
using FlaxEditor.Windows;
using FlaxEngine;
using FlaxEngine.GUI;
using KCC.Debugger;
using KCC.Debugger.Renderables;

namespace KCC;
#nullable enable

/// <summary>
/// </summary>
public class KCCDebuggerWindow : EditorWindow
{
	private readonly ToolStrip _toolStrip;
	private readonly ToolStripButton _recordButton;
	private readonly ToolStripButton _clearButton;
	private readonly ToolStripButton _toBeginningButton;
	private readonly ToolStripButton _previousFrameButton;
	private readonly ToolStripButton _toEndButton;
	private readonly ToolStripButton _nextFrameButton;
	private readonly Label _infoLabel;
	private readonly Slider _frameSlider;
	private readonly Panel _treePanel;
	private readonly Tree _tree;
	private bool _ignoreNextSelectionChange = false;
	private List<Guid> _actorRestoreList = [];

	/// <summary>
	/// Value indicating if KCC event recording is enabled.
	/// </summary>
	public bool Recording 
	{
		get => _recordButton.Checked;
		set
		{
			if(value != Recording)
			{
				_recordButton.Checked = value;
				OnRecordingChanged();
			}
		}
	}

	/// <inheritdoc />
	public KCCDebuggerWindow()
	 : base(Editor.Instance, true, ScrollBars.None)
	{
		Title = "KCC Debugger";

		_toolStrip = new ToolStrip()
		{
			Parent = this,
		};

		_recordButton = _toolStrip.AddButton(Editor.Icons.Play64);
		_recordButton.AutoCheck = true;
		_recordButton.LinkTooltip("Start capturing KCC events");
		_recordButton.Clicked += OnRecordingChanged;

		_clearButton = _toolStrip.AddButton(Editor.Icons.Rotate64);
		_clearButton.LinkTooltip("Clear all captured KCC events");
		_clearButton.Clicked += KCCDebugger.ClearFrames;

		_toolStrip.AddSeparator();

		_toBeginningButton = _toolStrip.AddButton(Editor.Icons.Left64);
		_toBeginningButton.LinkTooltip("To first frame");
		_toBeginningButton.Clicked += () => { KCCDebugger.Frame = 0; };

		_previousFrameButton = _toolStrip.AddButton(Editor.Icons.Left64);
		_previousFrameButton.LinkTooltip("Previous frame");
		_previousFrameButton.Clicked += () =>
		{
			if(KCCDebugger.Frame > 0)
			{
				KCCDebugger.Frame--;
			}
		};

		_nextFrameButton = _toolStrip.AddButton(Editor.Icons.Right64);
		_nextFrameButton.LinkTooltip("Next frame");
		_nextFrameButton.Clicked += () =>
		{
			if(KCCDebugger.Frame < KCCDebugger.Frames.Count - 1 )
			{
				KCCDebugger.Frame++;
			}
		};

		_toEndButton = _toolStrip.AddButton(Editor.Icons.Right64);
		_toEndButton.LinkTooltip("To last frame");
		_toEndButton.Clicked += () => { KCCDebugger.Frame = KCCDebugger.Frames.Count - 1; };

		_toolStrip.AddSeparator();
		_infoLabel = new Label()
		{
			Parent = _toolStrip,
			Text = "No frames.",
		};

		_frameSlider = new Slider()
		{
			Parent = _toolStrip,
			Minimum = 0.0f,
			Maximum = 100.0f,
		};

		_frameSlider.ValueChanged += () => {
			if(_frameSlider.IsSliding)
			{
				KCCDebugger.Frame = (int)Mathf.Min(_frameSlider.Value / 100.0f * KCCDebugger.Frames.Count, KCCDebugger.Frames.Count - 1);
			}
		};

		_treePanel = new Panel()
		{
			Parent = this,
			IsScrollable = true,
			ScrollBars = ScrollBars.Both,
			AnchorPreset = AnchorPresets.StretchAll,
			Offsets = new Margin(0, 0, _toolStrip.Bottom, 0),
		};

		_tree = new Tree(true)
		{
			Margin = new Margin(0.0f, 0.0f, -16.0f, _treePanel.ScrollBarsSize), // Hide root node
			Parent = _treePanel,
			IsScrollable = true,
			DrawRootTreeLine = false,
		};

		KCCDebugger.FrameChanged += OnFrameChanged;
		_tree.SelectedChanged += OnTreeSelectionChanged;

		UpdateButtons();
	}

	private void OnRecordingChanged()
	{
		KCCDebugger.Enabled = Recording;
		_recordButton.Icon = Recording ? Editor.Icons.Stop64 : Editor.Icons.Play64;
	}

	private void OnFrameChanged()
	{
		if(KCCDebugger.Frame != KCCDebugger.NO_FRAMES)
		{
			BubbleUpSelections();
			_actorRestoreList = _tree.Selection
				.Cast<EventNode>()
				.Where(node => node.Event.ActorID is not null)
				.Select(node => (Guid)node.Event.ActorID!)
				.ToList();
		}
		
		UpdateButtons();
		_tree.Selection.Clear();
		_tree.RemoveChildren();

		if(KCCDebugger.Frame == KCCDebugger.NO_FRAMES)
		{
			_infoLabel.Text = "No frames";
			return;
		}

		TreeNode root = new(false)
		{
			ChildrenIndent = 0,
			Text = "Frame root",
		};
		
		_tree.AddChild(root);
		foreach(Event @event in KCCDebugger.Frames[KCCDebugger.Frame].Events)
		{
			EventNode node = new(@event)
			{
				Text = @event.Name,
				Parent = root,
			};

			node.ConstructChildren();
		}

		root.Expand(true);
		foreach(Guid id in _actorRestoreList)
		{
			EventNode? eventNode = FindTopmostEventForActor(id, root);
			if(eventNode is null)
			{
				continue;
			}

			_tree.Selection.Add(eventNode);
			eventNode.ExpandAllParents();
		}

		_actorRestoreList.Clear();
		_infoLabel.Text = $"Frame {KCCDebugger.Frame} / {KCCDebugger.Frames.Count - 1}";
		if(!_frameSlider.IsSliding)
		{
			_frameSlider.Value = Mathf.Min(KCCDebugger.Frame, KCCDebugger.Frames.Count - 1) / (float)KCCDebugger.Frames.Count * 100.0f;
		}

		//force selection in KCC Debugger window, since we refreshed the tree
		OnTreeSelectionChanged(_tree.Selection, _tree.Selection);
		OnSceneEditingSelectionChanged(); 

		FocusOnFrame(KCCDebugger.Frame);
	}

	private void UpdateButtons()
	{
		_previousFrameButton.Enabled = KCCDebugger.Frame > 0;
		_nextFrameButton.Enabled = KCCDebugger.Frame < KCCDebugger.Frames.Count - 1;
		_toBeginningButton.Enabled = KCCDebugger.Frame != KCCDebugger.NO_FRAMES;
		_toEndButton.Enabled = KCCDebugger.Frame != KCCDebugger.NO_FRAMES;
		_frameSlider.Enabled = KCCDebugger.Frame != KCCDebugger.NO_FRAMES;
	}

	/// <inheritdoc />
	public override void OnParentResized()
	{
		base.OnParentResized();

		float totalWidth = 32.0f; //arbitrary amount, just to pervent the slider being cut off.
		for(int i = 0; i < _toolStrip.ChildrenCount - 1; i++)
		{
			Control child = _toolStrip.Children[i];
			totalWidth += child.Width;
		}

		_frameSlider.Width = _toolStrip.Width - totalWidth;
	}

	/// <summary>
	/// Handle drawing renderables from debug data
	/// </summary>
	public void DrawRenderables()
	{
		if(!Visible || KCCDebugger.Frame == KCCDebugger.NO_FRAMES || _tree.Selection.Count == 0)
		{
			return;
		}

		KCCDebugger.Frames[KCCDebugger.Frame].ResetRenderables();
		foreach(TreeNode node in _tree.Selection)
		{
			if(node is not EventNode eventNode)
			{
				continue;
			}

			eventNode.Event.Render();
		}

		DrawOnionSkin(KCCDebugger.Frame, 10);
	}

	private void DrawOnionSkin(int frame, int around)
	{
		//TODO: setting for this
		if(!Visible || KCCDebugger.Frame == KCCDebugger.NO_FRAMES || _tree.Selection.Count == 0)
		{
			return;
		}

		IEnumerable<EventNode> eventNodes = _tree.Selection.Cast<EventNode>();
		for(int i = frame - around; i <= frame + around; i++)
		{
			if(i >= KCCDebugger.Frames.Count)
			{
				return;
			}

			if(i == KCCDebugger.Frame || i < 0)
			{
				continue;
			}

			foreach(Event @event in KCCDebugger.Frames[i].Events)
			{
				foreach(EventNode eventNode in eventNodes)
				{
					if(eventNode.Event.ActorID != @event.ActorID)
					{
						continue;
					}

					@event.ResetRenderables(true);
					@event.Render(true);
					goto breakToOnionSkinLoop;
				}	
			}

			breakToOnionSkinLoop:
			;
		}
	}

	private void FocusOnFrame(int frame)
	{
		//TODO: setting for this
		if(!Visible || KCCDebugger.Frame == KCCDebugger.NO_FRAMES || _tree.Selection.Count == 0)
		{
			return;
		}

		IEnumerable<EventNode> eventNodes = _tree.Selection.Cast<EventNode>();
		//hack: calculate center from the events first renderable, since we know that to always exist.
		BoundingSphere averageSphere = BoundingSphere.Empty;
		Vector3 averageCenter = Vector3.Zero;
		int count = 0;
		foreach(EventNode eventNode in eventNodes)
		{
			if(eventNode.Event.ActorID is null)
			{
				continue;
			}

			switch(eventNode.Event.Renderables[0])
			{
				case Box box:
				{
					averageSphere = BoundingSphere.Merge(averageSphere, BoundingSphere.FromBox(box.OrientedBoundingBox.GetBoundingBox()));
					averageCenter = box.OrientedBoundingBox.Center;
					count++;
					break;
				}

				case Sphere sphere:
				{
					averageSphere = BoundingSphere.Merge(averageSphere, new BoundingSphere(sphere.Position, sphere.Radius));
					averageCenter = sphere.Position;
					count++;
					break;
				}

				case Capsule capsule:
				{
					averageSphere = BoundingSphere.Merge(averageSphere, new BoundingSphere(capsule.Position, capsule.Radius));
					averageCenter = capsule.Position;
					count++;
					break;
				}

				default:
					throw new NotImplementedException();
			}
		}

		if(count == 0 || averageSphere == BoundingSphere.Empty)
		{
			return;
		}

		averageCenter /= count;
		Editor.Instance.Windows.EditWin.Viewport.ViewportCamera.SetArcBallView(
			Editor.Instance.Windows.EditWin.Viewport.ViewOrientation,
			averageCenter, averageSphere.Radius * 5
		);
	}

	/// <summary>
	/// Called when the scene editing selection changes, used to synch up the current scene selection with the tree event node. 
	/// </summary>
	public void OnSceneEditingSelectionChanged()
	{
		if(KCCDebugger.Frame == KCCDebugger.NO_FRAMES)
		{
			return;
		}

		if(_ignoreNextSelectionChange)
		{
			_ignoreNextSelectionChange = false;
			return;
		}

		IEnumerable<ActorNode> actors = Editor.Instance.SceneEditing.Selection
			.OfType<ActorNode>();

		//deselect old
		for(int i = _tree.Selection.Count - 1; i >= 0; i--)
		{
			if(_tree.Selection[i] is not EventNode eventNode ||
				eventNode.Event.ActorID is null)
			{
				continue;
			}

			if(actors.Any(actorNode => actorNode.Actor.ID == eventNode.Event.ActorID))
			{
				continue;
			}

			_tree.Selection.RemoveAt(i);
		}

		//select new
		foreach(ActorNode actorNode in actors)
		{
			EventNode? topmostNode = FindTopmostEventForActor(actorNode.Actor, (ContainerControl)_tree.Children[0]);
			if(topmostNode is null)
			{
				continue;
			}

			if(!_tree.Selection.Contains(topmostNode))
			{
				_tree.Selection.Add(topmostNode);
				topmostNode.ExpandAllParents();
			}
		}
	}

	/// <summary>
	/// Traverse the tree and find the topmost event from root
	/// </summary>
	/// <param name="actor"></param>
	/// <param name="parentContainer"></param>
	/// <returns></returns>
	private static EventNode? FindTopmostEventForActor(Actor actor, ContainerControl parentContainer)
	{
		EventNode? result = null;
		foreach(Control control in parentContainer.Children)
		{
			if(control is not EventNode eventNode)
			{
				continue;
			}

			if(eventNode.Event.ActorID == actor.ID)
			{
				result = eventNode;
				break;
			}

			result = FindTopmostEventForActor(actor, eventNode);
			if(result != null)
			{
				break;
			}
		}

		return result;
	}

	/// <summary>
	/// Traverse the tree and find the topmost event from root
	/// </summary>
	/// <param name="id"></param>
	/// <param name="parentContainer"></param>
	/// <returns></returns>
	private static EventNode? FindTopmostEventForActor(Guid id, ContainerControl parentContainer)
	{
		EventNode? result = null;
		foreach(Control control in parentContainer.Children)
		{
			if(control is not EventNode eventNode)
			{
				continue;
			}

			if(eventNode.Event.ActorID == id)
			{
				result = eventNode;
				break;
			}

			result = FindTopmostEventForActor(id, eventNode);
			if(result != null)
			{
				break;
			}
		}

		return result;
	}

	/// <summary>
	/// Tarverse the tree and find the topmost event from subevent
	/// </summary>
	/// <param name="container"></param>
	/// <returns></returns>
	private static EventNode? FindTopmostEventForSubevent(ContainerControl container)
	{
		if(container is not EventNode eventNode)
		{
			return null;
		}

		if(eventNode.Event.ActorID is not null)
		{
			return eventNode;
		}

		return FindTopmostEventForSubevent(eventNode.Parent);
	}

	/// <summary>
	/// Called when the KCC debugger tree selection changes, used to synch up the current scene selection with the tree event node. 
	/// </summary>
	private void OnTreeSelectionChanged(List<TreeNode> before, List<TreeNode> after)
	{
		if(KCCDebugger.Frame == KCCDebugger.NO_FRAMES)
		{
			return;
		}

		if(_ignoreNextSelectionChange)
		{
			_ignoreNextSelectionChange = false;
			return;
		}

		IEnumerable<EventNode> eventNodes = after
			.OfType<EventNode>()
			.Where(node => node.Event.ActorID is not null);

		//deselect old
		for(int i = Editor.Instance.SceneEditing.Selection.Count - 1; i >= 0; i--)
		{
			if(Editor.Instance.SceneEditing.Selection[i] is not ActorNode actorNode)
			{
				continue;
			}

			if(eventNodes.Any(node => node.Event.ActorID == actorNode.ID))
			{
				continue;
			}

			_ignoreNextSelectionChange = true;
			Editor.Instance.SceneEditing.Deselect(actorNode);
		}

		//select new
		foreach(EventNode node in eventNodes)
		{
			if(node.Event.ActorID is null)
			{
				continue;
			}

			if(Editor.Instance.SceneEditing.Selection.Any(actor => actor.ID == node.Event.ActorID))
			{
				continue;
			}

			_ignoreNextSelectionChange = true;
			Editor.Instance.SceneEditing.Select(Editor.Instance.Scene.GetActorNode((Guid)node.Event.ActorID), true);
		} 
	}

	/// <summary>
	/// Move all selections from subevents to topmost event for actors
	/// </summary>
	public void BubbleUpSelections()
	{
		for(int i = _tree.Selection.Count - 1; i >= 0; i--)
		{
			if(_tree.Selection[i] is not EventNode eventNode)
			{
				continue;
			}

			if(eventNode.Event.ActorID is not null)
			{
				continue;
			}

			EventNode? topmostNode = FindTopmostEventForSubevent(eventNode);
			if(topmostNode is null)
			{
				continue;
			}

			_tree.Selection.RemoveAt(i);
			if(!_tree.Selection.Contains(topmostNode))
			{
				_tree.Selection.Add(topmostNode);
				topmostNode.ExpandAllParents();
			}
		}
	}

	/// <summary>
	/// Called just before when the editor play mode begins
	/// </summary>
	public void OnPlayModeBeginning()
	{
		KCCDebugger.ClearFrames();
	}
}