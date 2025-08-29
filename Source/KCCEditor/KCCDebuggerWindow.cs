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
			KCCDebugger.Frame = (int)Mathf.Min(_frameSlider.Value / 100.0f * KCCDebugger.Frames.Count, KCCDebugger.Frames.Count - 1);
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
		if(!_frameSlider.IsSliding)
		{
			_frameSlider.Value = Mathf.Min(KCCDebugger.Frame / KCCDebugger.Frames.Count * 100.0f, KCCDebugger.Frames.Count - 1);
		}
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