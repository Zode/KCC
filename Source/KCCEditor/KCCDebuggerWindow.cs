using FlaxEditor;
using FlaxEditor.GUI;
using FlaxEditor.GUI.Tree;
using FlaxEditor.Surface.Archetypes;
using FlaxEditor.Windows;
using FlaxEngine;
using FlaxEngine.GUI;
using KCC.Debugger;

namespace KCC;

/// <summary>
/// </summary>
public class KCCDebuggerWindow : EditorWindow
{
	private readonly ToolStripButton _recordButton;
	private readonly ToolStripButton _clearButton;
	private readonly ToolStripButton _toBeginningButton;
	private readonly ToolStripButton _previousFrameButton;
	private readonly ToolStripButton _toEndButton;
	private readonly ToolStripButton _nextFrameButton;
	private readonly Label _infoLabel;
	private readonly Panel _treePanel;
	private readonly Tree _tree;

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

		ToolStrip toolStrip = new()
		{
			Parent = this,
		};

		_recordButton = toolStrip.AddButton(Editor.Icons.Play64);
		_recordButton.AutoCheck = true;
		_recordButton.LinkTooltip("Start capturing KCC events");
		_recordButton.Clicked += OnRecordingChanged;

		_clearButton = toolStrip.AddButton(Editor.Icons.Rotate64);
		_clearButton.LinkTooltip("Clear all captured KCC events");
		_clearButton.Clicked += KCCDebugger.ClearFrames;

		toolStrip.AddSeparator();

		_toBeginningButton = toolStrip.AddButton(Editor.Icons.Left64);
		_toBeginningButton.LinkTooltip("To first frame");
		_toBeginningButton.Clicked += () => { KCCDebugger.Frame = 0; };

		_previousFrameButton = toolStrip.AddButton(Editor.Icons.Left64);
		_previousFrameButton.LinkTooltip("Previous frame");
		_previousFrameButton.Clicked += () =>
		{
			if(KCCDebugger.Frame > 0)
			{
				KCCDebugger.Frame--;
			}
		};

		_nextFrameButton = toolStrip.AddButton(Editor.Icons.Right64);
		_nextFrameButton.LinkTooltip("Next frame");
		_nextFrameButton.Clicked += () =>
		{
			if(KCCDebugger.Frame < KCCDebugger.Frames.Count - 1 )
			{
				KCCDebugger.Frame++;
			}
		};

		_toEndButton = toolStrip.AddButton(Editor.Icons.Right64);
		_toEndButton.LinkTooltip("To last frame");
		_toEndButton.Clicked += () => { KCCDebugger.Frame = KCCDebugger.Frames.Count - 1; };

		toolStrip.AddSeparator();
		_infoLabel = new Label()
		{
			Parent = toolStrip,
			Text = "No frames.",
		};

		_treePanel = new Panel()
		{
			Parent = this,
			IsScrollable = true,
			ScrollBars = ScrollBars.Both,
			AnchorPreset = AnchorPresets.StretchAll,
			Offsets = new Margin(0, 0, toolStrip.Bottom, 0),
		};

		_tree = new Tree(true)
		{
			Margin = new Margin(0.0f, 0.0f, -16.0f, _treePanel.ScrollBarsSize), // Hide root node
			Parent = _treePanel,
			IsScrollable = true,
			DrawRootTreeLine = false,
		};

		KCCDebugger.FrameChanged += OnFrameChanged;
		UpdateButtons();
	}

	private void OnRecordingChanged()
	{
		KCCDebugger.Enabled = Recording;
		_recordButton.Icon = Recording ? Editor.Icons.Stop64 : Editor.Icons.Play64;
	}

	private void OnFrameChanged()
	{
		UpdateButtons();
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
		_infoLabel.Text = $"Frame {KCCDebugger.Frame} / {KCCDebugger.Frames.Count - 1}";
	}

	private void UpdateButtons()
	{
		_previousFrameButton.Enabled = KCCDebugger.Frame > 0;
		_nextFrameButton.Enabled = KCCDebugger.Frame < KCCDebugger.Frames.Count - 1;
		_toBeginningButton.Enabled = KCCDebugger.Frame != KCCDebugger.NO_FRAMES;
		_toEndButton.Enabled = KCCDebugger.Frame != KCCDebugger.NO_FRAMES;
	}

	/// <summary>
	/// Handle drawing renderables from debug data
	/// </summary>
	public void DrawRenderables()
	{
		if(!Visible || _tree.SelectedNode == null)
		{
			return;
		}

		if(_tree.SelectedNode is EventNode eventNode)
		{
			eventNode.Event.Render();
		}
	}
}