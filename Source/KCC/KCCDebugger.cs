#if FLAX_EDITOR
using System;
using System.Collections.Generic;
using FlaxEditor;
using FlaxEngine;
using KCC.Debugger;
using KCC.Debugger.Renderables;

namespace KCC;

/// <summary>
/// </summary>
public static class KCCDebugger
{
	/// <summary>
	/// Constant for the <seealso cref="Frame" /> property meaning that there are no frames in <seealso cref="Frames" /> list.
	/// </summary>
	public const int NO_FRAMES = -1;

	/// <summary>
	/// Control whether KCCDebugger is enabled and recording events.
	/// </summary>
	public static bool Enabled {get; set;} = false;
	/// <summary>
	/// List of the debugger frames
	/// </summary>
	public static List<Frame> Frames {get; private set;} = [];
	private static Stack<Event> _frameParents = [];
	private static int _frame = NO_FRAMES;
	private static int _internalFrame = NO_FRAMES;
	/// <summary>
	/// The currently active <seealso cref="Frame" />.
	/// </summary>
	public static int Frame
	{
		get => _frame;
		set
		{
			if(value != _frame)
			{
				_frame = value;
				FrameChanged?.Invoke();
			}
		}
	}

	/// <summary>
	/// Action fired when the <seealso cref="Frame" /> property changes
	/// </summary>	
	public static Action FrameChanged;

	private static bool CanProcess => Enabled && Editor.IsPlayMode;
	private static bool CanProcessHasFrame => CanProcess && _internalFrame != NO_FRAMES;

	/// <summary>
	/// Start a new frame
	/// </summary>
	public static void BeginFrame()
	{
		if(!CanProcess)
		{
			return;
		}

		Frames.Add(new Frame());
		_internalFrame = Frames.Count - 1;
	}

	/// <summary>
	/// End the frame
	/// </summary>
	public static void EndFrame()
	{
		Frame = _internalFrame;
		if(!CanProcessHasFrame)
		{
			return;
		}

		if(_frameParents.Count > 0)
		{
			Debug.LogWarning("KCCDebugger: Ended frame with event parents in stack. Please ensure you are ending events properly.");
		}
	}

	/// <summary>
	/// Clear all frames from the debugger.
	/// </summary>
	public static void ClearFrames()
	{
		Frames.Clear();
		_internalFrame = -1;
		Frame = -1;
	}

	/// <summary>
	/// Begin a new debugger event
	/// </summary>
	/// <param name="name"></param>
	/// <param name="actor"></param>
	public static void BeginEvent(Actor actor, string name)
	{
		if(!CanProcessHasFrame)
		{
			return;
		}

		Event @event = new(actor, name);
		if(_frameParents.Count == 0)
		{
			Frames[_internalFrame].Events.Add(@event);
			_frameParents.Push(@event);
			return;
		}

		_frameParents.Peek().Events.Add(@event);
		_frameParents.Push(@event);
	}

	/// <summary>
	/// Begin a new debugger event
	/// </summary>
	/// <param name="name"></param>
	public static void BeginEvent(string name)
	{
		if(!CanProcessHasFrame)
		{
			return;
		}

		if(_frameParents.Count == 0)
		{
			Debug.LogWarning("KCCDebugger: Can't begin actorless event without parent event. Please ensure you are starting events properly.");
			return;
		}

		Event parent = _frameParents.Peek();
		Event @event = new(parent.Actor, name);
		parent.Events.Add(@event);
		_frameParents.Push(@event);
	}

	/// <summary>
	/// End a debugger event
	/// </summary>
	public static void EndEvent()
	{
		if(!CanProcessHasFrame)
		{
			return;
		}

		if(_frameParents.Count > 0)
		{
			_frameParents.Pop();
		}
	}

	/// <summary>
	/// Draw a line between two points.
	/// </summary>
	/// <param name="start">Starting position in world space.</param>
	/// <param name="end">Ending position in world space.</param>
	/// <param name="color"></param>
	/// <param name="depthTest"></param>
	public static void DrawLine(Vector3 start, Vector3 end, Color color, bool depthTest)
	{
		if(!CanProcessHasFrame)
		{
			return;
		}

		if(_frameParents.Count == 0)
		{
			Debug.LogWarning("KCCDebugger: Can't draw line without an event. Please ensure you are only drawing inside events.");
			return;
		}

		_frameParents.Peek().Renderables.Add(new Line()
		{
			StartPosition = start,
			EndPosition = end,
			OutlineColor = color,
			DepthTest = depthTest,
		});
	}

	/// <summary>
	/// Draw an oriented box. If fill color alpha is zero, the box will be outline only.
	/// </summary>
	/// <param name="obb"></param>
	/// <param name="fillColor"></param>
	/// <param name="outlineColor"></param>
	/// <param name="depthTest"></param>
	public static void DrawBox(OrientedBoundingBox obb, Color fillColor, Color outlineColor, bool depthTest)
	{
		if(!CanProcessHasFrame)
		{
			return;
		}

		if(_frameParents.Count == 0)
		{
			Debug.LogWarning("KCCDebugger: Can't draw box without an event. Please ensure you are only drawing inside events.");
			return;
		}

		_frameParents.Peek().Renderables.Add(new Box()
		{
			OrientedBoundingBox = obb,
			FillColor = fillColor,
			OutlineColor = outlineColor,
			DepthTest = depthTest,
		});
	}

	/// <summary>
	/// Draw an oriented capsule. If fill color alpha is zero, the capsule will be outline only.
	/// </summary>
	/// <param name="position">Position in world ppace.</param>
	/// <param name="orientation">Orientation in world space.</param>
	/// <param name="radius"></param>
	/// <param name="height"></param>
	/// <param name="fillColor"></param>
	/// <param name="outlineColor"></param>
	/// <param name="depthTest"></param>
	public static void DrawCapsule(Vector3 position, Quaternion orientation, float radius, float height, Color fillColor, Color outlineColor, bool depthTest)
	{
		if(!CanProcessHasFrame)
		{
			return;
		}

		if(_frameParents.Count == 0)
		{
			Debug.LogWarning("KCCDebugger: Can't draw capsule without an event. Please ensure you are only drawing inside events.");
			return;
		}

		_frameParents.Peek().Renderables.Add(new Capsule()
		{
			StartPosition = position,
			Orientation = orientation,
			Radius = radius,
			Height = height,
			FillColor = fillColor,
			OutlineColor = outlineColor,
			DepthTest = depthTest,
		});
	}

	/// <summary>
	/// Draw a sphere. If fill color alpha is zero, the sphere will be outline only.
	/// </summary>
	/// <param name="position">Position in world space.</param>
	/// <param name="radius"></param>
	/// <param name="fillColor"></param>
	/// <param name="outlineColor"></param>
	/// <param name="depthTest"></param>
	public static void DrawSphere(Vector3 position, float radius, Color fillColor, Color outlineColor, bool depthTest)
	{
		if(!CanProcessHasFrame)
		{
			return;
		}

		if(_frameParents.Count == 0)
		{
			Debug.LogWarning("KCCDebugger: Can't draw sphere without an event. Please ensure you are only drawing inside events.");
			return;
		}

		_frameParents.Peek().Renderables.Add(new Sphere()
		{
			StartPosition = position,
			Radius = radius,
			FillColor = fillColor,
			OutlineColor = outlineColor,
			DepthTest = depthTest,
		});
	}

	/// <summary>
	/// Draw an arrow.
	/// </summary>
	/// <param name="position">Position in world space.</param>
	/// <param name="orientation">Orientation in world space.</param>
	/// <param name="scale"></param>
	/// <param name="capScale"></param>
	/// <param name="color"></param>
	/// <param name="depthTest"></param>
	public static void DrawArrow(Vector3 position, Quaternion orientation, float scale, float capScale, Color color, bool depthTest)
	{
		if(!CanProcessHasFrame)
		{
			return;
		}

		if(_frameParents.Count == 0)
		{
			Debug.LogWarning("KCCDebugger: Can't draw arrow without an event. Please ensure you are only drawing inside events.");
			return;
		}

		_frameParents.Peek().Renderables.Add(new Arrow()
		{
			StartPosition = position,
			Orientation = orientation,
			Radius = scale,
			Height = capScale,
			OutlineColor = color,
			DepthTest = depthTest,
		});
	}

	/// <summary>
	/// Draw a quad.
	/// </summary>
	/// <param name="position">Position in world space.</param>
	/// <param name="orientation">Orientation in world space.</param>
	/// <param name="scale"></param>
	/// <param name="fillColor"></param>
	/// <param name="outlineColor"></param>
	/// <param name="depthTest"></param>
	public static void DrawQuad(Vector3 position, Quaternion orientation, float scale, Color fillColor, Color outlineColor, bool depthTest)
	{
		if(!CanProcessHasFrame)
		{
			return;
		}

		if(_frameParents.Count == 0)
		{
			Debug.LogWarning("KCCDebugger: Can't draw quad without an event. Please ensure you are only drawing inside events.");
			return;
		}

		_frameParents.Peek().Renderables.Add(new Quad()
		{
			StartPosition = position,
			Orientation = orientation,
			Radius = scale,
			FillColor = fillColor,
			OutlineColor = outlineColor,
			DepthTest = depthTest,
		});
	}
}
#endif