#if FLAX_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using FlaxEditor;
using FlaxEngine;
using KCC.Debugger;
using KCC.Debugger.Renderables;

namespace KCC;

/// <summary>
/// </summary>
public static class KCCDebugger
{
	private const string DEBUGGER_NAME = "KCC Debugger";

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
	private static readonly Stack<Event> _frameParents = [];
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
	/// KCC Debugger options instance.
	/// </summary>
	public static KCCDebuggerOptions Options => Editor.Instance.Options.Options.GetCustomSettings<KCCDebuggerOptions>(DEBUGGER_NAME);

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
			FlaxEngine.Debug.LogWarning("KCCDebugger: Ended frame with event parents in stack. Please ensure you are ending events properly.");
			_frameParents.Clear();
		}

		Frames[Frame].CalculateTime();
	}

	/// <summary>
	/// Clear all frames from the debugger.
	/// </summary>
	public static void ClearFrames()
	{
		Frames.Clear();
		_frameParents.Clear();
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

		if(actor is null)
		{
			FlaxEngine.Debug.LogWarning("KCCDebugger: Can't begin actor event without non null actor. Please ensure you are starting events properly.");
			return;
		}

		Event @event = new(actor.ID, name);
		if(_frameParents.Count == 0)
		{
			Frames[_internalFrame].Events.Add(@event);
			_frameParents.Push(@event);
			@event.Timer.Start();
			return;
		}

		_frameParents.Peek().Events.Add(@event);
		_frameParents.Push(@event);
		@event.Timer.Start();
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
			FlaxEngine.Debug.LogWarning("KCCDebugger: Can't begin actorless event without parent event. Please ensure you are starting events properly.");
			return;
		}

		Event @event = new(null, name);
		_frameParents.Peek().Events.Add(@event);
		_frameParents.Push(@event);
		@event.Timer.Start();
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
			_frameParents.Peek().Timer.Stop();
			_frameParents.Pop();
		}
		else
		{
			FlaxEngine.Debug.LogWarning("KCCDebugger: Can't stop event without ever starting an event. Please ensure you are starting events properly.");
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
			FlaxEngine.Debug.LogWarning("KCCDebugger: Can't draw line without an event. Please ensure you are only drawing inside events.");
			return;
		}

		_frameParents.Peek().Renderables.Add(new Line()
		{
			Position = start,
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
			FlaxEngine.Debug.LogWarning("KCCDebugger: Can't draw box without an event. Please ensure you are only drawing inside events.");
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
			FlaxEngine.Debug.LogWarning("KCCDebugger: Can't draw capsule without an event. Please ensure you are only drawing inside events.");
			return;
		}

		_frameParents.Peek().Renderables.Add(new Capsule()
		{
			Position = position,
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
			FlaxEngine.Debug.LogWarning("KCCDebugger: Can't draw sphere without an event. Please ensure you are only drawing inside events.");
			return;
		}

		_frameParents.Peek().Renderables.Add(new Sphere()
		{
			Position = position,
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
			FlaxEngine.Debug.LogWarning("KCCDebugger: Can't draw arrow without an event. Please ensure you are only drawing inside events.");
			return;
		}

		_frameParents.Peek().Renderables.Add(new Arrow()
		{
			Position = position,
			Orientation = orientation,
			Scale = scale,
			CapScale = capScale,
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
			FlaxEngine.Debug.LogWarning("KCCDebugger: Can't draw quad without an event. Please ensure you are only drawing inside events.");
			return;
		}
		
		Float3[] quadVerts = [
			new Float3(-0.5f, 0.5f, 0.0f),
			new Float3(0.5f, 0.5f, 0.0f),
			new Float3(-0.5f, -0.5f, 0.0f),
			new Float3(0.5f, -0.5f, 0.0f),
		];

		Matrix world = Matrix.Scaling(scale) * Matrix.RotationQuaternion(orientation) * Matrix.Translation(new Float3((float)position.X, (float)position.Y, (float)position.Z));

		for(int i = 0; i < 4; i++)
		{
			quadVerts[i] = Float3.Transform(quadVerts[i], world);
		}

		_frameParents.Peek().Renderables.Add(new Quad()
		{
			Vertices = quadVerts,
			FillColor = fillColor,
			OutlineColor = outlineColor,
			DepthTest = depthTest,
		});
	}

	/// <summary>
	/// Draw a mesh.
	/// </summary>
	/// <param name="vertices"></param>
	/// <param name="indices"></param>
	/// <param name="position">Position in world space.</param>
	/// <param name="orientation">Orientation in world space.</param>
	/// <param name="fillColor"></param>
	/// <param name="outlineColor"></param>
	/// <param name="depthTest"></param>
	public static void DrawMesh(Float3[] vertices, int[] indices, Vector3 position, Quaternion orientation, Color fillColor, Color outlineColor, bool depthTest)
	{
		if(!CanProcessHasFrame)
		{
			return;
		}

		if(_frameParents.Count == 0)
		{
			FlaxEngine.Debug.LogWarning("KCCDebugger: Can't draw quad without an event. Please ensure you are only drawing inside events.");
			return;
		}

		Matrix world = Matrix.RotationQuaternion(orientation) * Matrix.Translation(new Float3((float)position.X, (float)position.Y, (float)position.Z));

		for(int i = 0; i < vertices.Length; i++)
		{
			vertices[i] = Float3.Transform(vertices[i], world);
		}

		_frameParents.Peek().Renderables.Add(new Debugger.Renderables.Mesh()
		{
			Vertices = vertices,
			Indices = indices,
			FillColor = fillColor,
			OutlineColor = outlineColor,
			DepthTest = depthTest,
		});
	}

	/// <summary>
	/// Draw a collider.
	/// </summary>
	/// <param name="collider"></param>
	/// <param name="fillColor"></param>
	/// <param name="outlineColor"></param>
	/// <param name="depthTest"></param>
	public static void DrawCollider(PhysicsColliderActor collider, Color fillColor, Color outlineColor, bool depthTest)
	{
		if(!CanProcessHasFrame)
		{
			return;
		}

		if(_frameParents.Count == 0)
		{
			FlaxEngine.Debug.LogWarning("KCCDebugger: Can't draw collider without an event. Please ensure you are only drawing inside events.");
			return;
		}

		switch(collider)
		{
			case BoxCollider box:
				DrawBox(box.OrientedBox, fillColor, outlineColor, depthTest);
				break;

			case SphereCollider sphere:
				DrawSphere(sphere.Position, sphere.Radius, fillColor, outlineColor, depthTest);
				break;

			case CapsuleCollider capsule:
				DrawCapsule(capsule.Position, capsule.Orientation, capsule.Radius, capsule.Height, fillColor, outlineColor, depthTest);
				break;

			case MeshCollider mesh:
			{
				mesh.CollisionData.ExtractGeometry(out Float3[] verts, out int[] indices);
				DrawMesh(verts, indices, mesh.Position, mesh.Orientation, fillColor, outlineColor, depthTest);
				break;
			}

			case SplineCollider spline:
			{
				spline.CollisionData.ExtractGeometry(out Float3[] verts, out int[] indices);
				DrawMesh(verts, indices, spline.Position, spline.Orientation, fillColor, outlineColor, depthTest);
				break;
			}

			case Terrain terrain:
			{
				Vector3 relativePosition = collider.Position - terrain.Position;
				float size = terrain.ChunkSize * Terrain.UnitsPerVertex * Terrain.PatchEdgeChunksCount;
				Int2 patchCoord = new((int)(relativePosition.X / size), (int)(relativePosition.Z / size));
				if(!terrain.HasPatch(ref patchCoord))
				{
					FlaxEngine.Debug.LogWarning($"KCCDebugger: Can't draw terrain. Somehow have a position ({relativePosition} -> {patchCoord}) that is not inside any patch.");
					break;
				}

				TerrainPatch patch = terrain.GetPatch(ref patchCoord);
				patch.ExtractCollisionGeometry(out Float3[] verts, out int[] indices);
				DrawMesh(verts, indices, Vector3.Zero, Quaternion.Identity, fillColor, outlineColor, depthTest);
				break;
			}

			default:
				throw new NotImplementedException($"Unsupported collider: {collider.GetType()}");
		}
	}

	/// <summary>
	/// Draw some text.
	/// </summary>
	/// <param name="position">Position in world space</param>
	/// <param name="text"></param>
	/// <param name="size">Font size to use</param>
	/// <param name="scale">Font scale to use</param>
	/// <param name="color"></param>
	/// <param name="depthTest"></param>
	public static void DrawText(Vector3 position, string text, int size, float scale, Color color, bool depthTest)
	{
		if(!CanProcessHasFrame)
		{
			return;
		}

		if(_frameParents.Count == 0)
		{
			FlaxEngine.Debug.LogWarning("KCCDebugger: Can't draw text without an event. Please ensure you are only drawing inside events.");
			return;
		}

		_frameParents.Peek().Renderables.Add(new Text()
		{
			Position = position,
			Content = text,
			Size = size,
			Scale = scale,
			OutlineColor = color,
			DepthTest = depthTest,
		});
	}

	/// <summary>
	/// Draw some text using KCC Debugger font settings.
	/// </summary>
	/// <param name="position">Position in world space</param>
	/// <param name="text"></param>
	/// <param name="depthTest"></param>
	public static void DrawText(Vector3 position, string text, bool depthTest)
	{
		DrawText(position, text, Options.TextFontSize, Options.TextScale, Options.TextColor, depthTest);
	}
}
#endif