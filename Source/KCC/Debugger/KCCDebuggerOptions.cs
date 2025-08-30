#if FLAX_EDITOR
using System.ComponentModel;
using FlaxEngine;

namespace KCC;

/// <summary>
/// KCC Debugger options (wow what an amazing comment).
/// </summary>
[CustomEditor(typeof(KCCDebuggerOptionsEditor))]
public class KCCDebuggerOptions
{
	/// <summary>
	/// Draw characters with flax's debug draw while selected.
	/// </summary>
	[DefaultValue(true)]
	[EditorDisplay("General")]
	public bool FlaxDrawActorDebug = true;
	/// <summary>
	/// The color to use for the character using flax's debug draw while selected.
	/// </summary>
	[DefaultValue(typeof(Color), "0.6,0.8,0.19,1.0")]
	[EditorDisplay("General")]
	public Color FlaxActorColor = new(0.6f, 0.8f, 0.19f, 1.0f);
	/// <summary>
	/// The color to use to indicate the forward orientation of the character.
	/// </summary>
	[DefaultValue(typeof(Color), "0.26,0.0,1.0,0.5")]
	[EditorDisplay("General")]
	public Color ForwardArrowColor = new(0.26f, 0.0f, 1.0f, 0.5f);
	/// <summary>
	/// The color to use to draw the delta passed to the character.
	/// </summary>
	[DefaultValue(typeof(Color), "0.0,1.0,0.0,1.0")]
	[EditorDisplay("General")]
	public Color DeltaArrowColor = new(0.0f, 1.0f, 0.0f, 1.0f);
	/// <summary>
	/// General text color.
	/// </summary>
	[DefaultValue(typeof(Color), "1.0,1.0,1.0,1.0")]
	[EditorDisplay("General")]
	public Color TextColor = new(1.0f, 1.0f, 1.0f, 1.0f);
	/// <summary>
	/// General font size.
	/// </summary>
	[DefaultValue(24)]
	[EditorDisplay("General")]
	public int TextFontSize = 24;
	/// <summary>
	/// General font scaling.
	/// </summary>
	[DefaultValue(0.5f)]
	[EditorDisplay("General")]
	public float TextScale = 0.5f;

	/// <summary>
	/// The fill color to use to mark the character sweep pass start position.
	/// </summary>
	[DefaultValue(typeof(Color), "0.0,0.0,0.0,0.0")]
	[EditorDisplay("Sweep")]
	public Color SweepStartFillColor = new(0.0f, 0.0f, 0.0f, 0.0f);
	/// <summary>
	/// The outline color to use to mark the character sweep pass start position.
	/// </summary>
	[DefaultValue(typeof(Color), "0.0,1.0,0.4,0.5")]
	[EditorDisplay("Sweep")]
	public Color SweepStartOutlineColor = new(0.0f, 1.0f, 0.4f, 0.5f);
	/// <summary>
	/// The color to use to mark the character sweep pass arrow.
	/// </summary>
	[DefaultValue(typeof(Color), "0.0,1.0,0.4,0.5")]
	[EditorDisplay("Sweep")]
	public Color SweepArrowColor = new(0.0f, 1.0f, 0.4f, 0.5f);
	/// <summary>
	/// The fill color to use to mark the character sweep pass end position.
	/// </summary>
	[DefaultValue(typeof(Color), "0.0,0.0,0.0,0.0")]
	[EditorDisplay("Sweep")]
	public Color SweepEndFillColor = new(0.0f, 0.0f, 0.0f, 0.0f);
	/// <summary>
	/// The outline color to use to mark the character sweep pass end position.
	/// </summary>
	[DefaultValue(typeof(Color), "0.0,1.0,0.4,0.5")]
	[EditorDisplay("Sweep")]
	public Color SweepEndOutlineColor = new(0.0f, 1.0f, 0.4f, 0.5f);
	/// <summary>
	/// The color to use to mark contact normal made during sweeping.
	/// </summary>
	[DefaultValue(typeof(Color), "1.0,0.0,0.0,1.0")]
	[EditorDisplay("Sweep")]
	public Color ContactArrowColor = new(1.0f, 0.0f, 0.0f, 1.0f);
	/// <summary>
	/// The fill color to use to mark contact normal plane made during sweeping.
	/// </summary>
	[DefaultValue(typeof(Color), "1.0,0.0,0.0,0.15")]
	[EditorDisplay("Sweep")]
	public Color ContactQuadFillColor = new(1.0f, 0.0f, 0.0f, 0.15f);
	/// <summary>
	/// The outline color to use to mark contact normal plane made during sweeping.
	/// </summary>
	[DefaultValue(typeof(Color), "1.0,0.0,0.0,1.0")]
	[EditorDisplay("Sweep")]
	public Color ContactQuadOutlineColor = new(1.0f, 0.0f, 0.0f, 1.0f);
	/// <summary>
	/// The color to use to mark sliding plane normal used during sweeping.
	/// </summary>
	[DefaultValue(typeof(Color), "1.0,0.5,0.0,1.0")]
	[EditorDisplay("Sweep")]
	public Color SlidingPlaneArrowColor = new(1.0f, 0.5f, 0.0f, 1.0f);
	/// <summary>
	/// The fill color to use to mark sliding plane used during sweeping.
	/// </summary>
	[DefaultValue(typeof(Color), "1.0,0.5,0.0,0.10")]
	[EditorDisplay("Sweep")]
	public Color SlidingPlaneQuadFillColor = new(1.0f, 0.5f, 0.0f, 0.10f);
	/// <summary>
	/// The outline color to use to mark sliding plane used during sweeping.
	/// </summary>
	[DefaultValue(typeof(Color), "1.0,0.5,0.0,1.0")]
	[EditorDisplay("Sweep")]
	public Color SlidingPlaneQuadOutlineColor = new(1.0f, 0.5f, 0.0f, 1.0f);
	/// <summary>
	/// The  color to use to mark the crease used during second sweeping pass.
	/// </summary>
	[DefaultValue(typeof(Color), "1.0,0.0,1.0,1.0")]
	[EditorDisplay("Sweep")]
	public Color CreaseLineColor = new(1.0f, 0.0f, 1.0f, 1.0f);

	/// <summary>
	/// The fill color to use to mark the character cast start position.
	/// </summary>
	[DefaultValue(typeof(Color), "0.0,0.0,0.0,0.0")]
	[EditorDisplay("Casting")]
	public Color CastStartFillColor = new(0.0f, 0.0f, 0.0f, 0.0f);
	/// <summary>
	/// The outline color to use to mark the character cast start position.
	/// </summary>
	[DefaultValue(typeof(Color), "0.0,0.0,1.0,0.5")]
	[EditorDisplay("Casting")]
	public Color CastStartOutlineColor = new(0.0f, 0.0f, 1.0f, 0.5f);
	/// <summary>
	/// The color to use to mark the character cast line.
	/// </summary>
	[DefaultValue(typeof(Color), "0.0,0.5,1.0,0.5")]
	[EditorDisplay("Casting")]
	public Color CastLineColor = new(0.0f, 0.5f, 1.0f, 0.5f);
	/// <summary>
	/// The fill color to use to mark the character cast end position.
	/// </summary>
	[DefaultValue(typeof(Color), "0.0,0.0,0.0,0.0")]
	[EditorDisplay("Casting")]
	public Color CastEndFillColor = new(0.0f, 0.0f, 0.0f, 0.0f);
	/// <summary>
	/// The outline color to use to mark the character cast end position.
	/// </summary>
	[DefaultValue(typeof(Color), "0.0,0.5,1.0,0.5")]
	[EditorDisplay("Casting")]
	public Color CastEndOutlineColor = new(0.0f, 0.5f, 1.0f, 0.5f);
	/// <summary>
	/// The fill color to use to mark the character cast end result position.
	/// </summary>
	[DefaultValue(typeof(Color), "0.0,0.0,0.0,0.0")]
	[EditorDisplay("Casting")]
	public Color CastResultFillColor = new(0.0f, 0.0f, 0.0f, 0.0f);
	/// <summary>
	/// The outline color to use to mark the character cast end result position.
	/// </summary>
	[DefaultValue(typeof(Color), "0.0,0.8,1.0,0.8")]
	[EditorDisplay("Casting")]
	public Color CastResultOutlineColor = new(0.0f, 0.8f, 1.0f, 0.8f);
	/// <summary>
	/// The color to use to mark the character cast end result line.
	/// </summary>
	[DefaultValue(typeof(Color), "0.0,0.8,1.0,1.0")]
	[EditorDisplay("Casting")]
	public Color CastResultLineColor = new(0.0f, 0.8f, 1.0f, 1.0f);

	/// <summary>
	/// The fill color to use to mark the character during overlap.
	/// </summary>
	[DefaultValue(typeof(Color), "1.0,1.0,0.0,0.10")]
	[EditorDisplay("Overlapping")]
	public Color OverlapFillColor = new(1.0f, 1.0f, 0.0f, 0.10f);
	/// <summary>
	/// The outline color to use to mark the character during overlap.
	/// </summary>
	[DefaultValue(typeof(Color), "1.0,1.0,0.0,1.0")]
	[EditorDisplay("Overlapping")]
	public Color OverlapOutlineColor = new(1.0f, 1.0f, 0.0f, 1.00f);
	/// <summary>
	/// The fill color to use to mark the other collider during overlap.
	/// </summary>
	[DefaultValue(typeof(Color), "1.0,0.0,1.0,0.10")]
	[EditorDisplay("Overlapping")]
	public Color OverlapOtherFillColor = new(1.0f, 0.0f, 1.0f, 0.10f);
	/// <summary>
	/// The outline color to use to mark the other collider during overlap.
	/// </summary>
	[DefaultValue(typeof(Color), "1.0,0.0,1.0,1.0")]
	[EditorDisplay("Overlapping")]
	public Color OverlapOtherOutlineColor = new(1.0f, 0.0f, 1.0f, 1.00f);

	/// <summary>
	/// The fill color to use to mark the character during stairstep pass.
	/// </summary>
	[DefaultValue(typeof(Color), "0.0,0.0,0.0,0.0")]
	[EditorDisplay("Stairstepping")]
	public Color StairstepFillColor = new(0.0f, 0.0f, 0.0f, 0.0f);
	/// <summary>
	/// The outline color to use to mark the character during stairstep pass.
	/// </summary>
	[DefaultValue(typeof(Color), "0.0,1.0,1.0,0.5")]
	[EditorDisplay("Stairstepping")]
	public Color StairstepOutlineColor = new(0.0f, 1.0f, 1.0f, 0.5f);

	/// <summary>
	/// The color to use to mark unstuck solve amount for a single overlapping collider.
	/// </summary>
	[DefaultValue(typeof(Color), "0.5,0.0,1.0,0.5")]
	[EditorDisplay("Unstuck")]
	public Color UnstuckSingularArrowColor = new(0.5f, 0.0f, 1.0f, 0.5f);
	/// <summary>
	/// The color to use to mark unstuck solve total amount.
	/// </summary>
	[DefaultValue(typeof(Color), "0.5,0.0,1.0,1.0")]
	[EditorDisplay("Unstuck")]
	public Color UnstuckTotalArrowColor = new(0.5f, 0.0f, 1.0f, 1.0f);
	/// <summary>
	/// The fill color to use to mark the character during unstuck rescue.
	/// </summary>
	[DefaultValue(typeof(Color), "0.0,0.0,0.0,0.0")]
	[EditorDisplay("Unstuck")]
	public Color UnstuckRescueFillColor = new(0.0f, 0.0f, 0.0f, 0.0f);
	/// <summary>
	/// The outline color to use to mark the character during unstuck rescue.
	/// </summary>
	[DefaultValue(typeof(Color), "1.0,0.0,0.0,1.0")]
	[EditorDisplay("Unstuck")]
	public Color UnstuckRescueOutlineColor = new(1.0f, 0.0f, 0.0f, 1.0f);
	/// <summary>
	/// The fill color to use to mark the other collider during compute penetration triangles pass.
	/// </summary>
	[DefaultValue(typeof(Color), "0.5,0.0,1.0,0.15")]
	[EditorDisplay("Unstuck")]
	public Color PenetrationFillColor = new(0.5f, 0.0f, 1.0f, 0.15f);
	/// <summary>
	/// The outline color to use to mark the other collider during compute penetration triangles pass
	/// </summary>
	[DefaultValue(typeof(Color), "0.5,0.0,1.0,1.0")]
	[EditorDisplay("Unstuck")]
	public Color PenetrationOutlineColor = new(0.5f, 0.0f, 1.0f, 1.0f);
	/// <summary>
	/// The color to use to mark the character's linecast during compute penetration triangles pass
	/// </summary>
	[DefaultValue(typeof(Color), "1.0,0.0,0.0,1.0")]
	[EditorDisplay("Unstuck")]
	public Color PenetrationTraceColor = new(1.0f, 0.0f, 0.0f, 1.0f);
	/// <summary>
	/// The color to use to mark the other collider's linecast during compute penetration triangles pass
	/// </summary>
	[DefaultValue(typeof(Color), "0.5,0.0,0.0,1.0")]
	[EditorDisplay("Unstuck")]
	public Color PenetrationTraceOtherColor = new(0.5f, 0.0f, 0.0f, 1.0f);

	/// <summary>
	/// The fill color to use to mark the character onion skinning position.
	/// </summary>
	[DefaultValue(typeof(Color), "0.0,0.0,0.0,0.0")]
	[EditorDisplay("Onion skinning")]
	public Color OnionSkinFillColor = new(0.0f, 0.0f, 0.0f, 0.0f);
	/// <summary>
	/// The outline color to use to mark the character onion skinning position.
	/// </summary>
	[DefaultValue(typeof(Color), "0.5,0.5,0.5,1.0")]
	[EditorDisplay("Onion skinning")]
	public Color OnionSkinOutlineColor = new(0.5f, 0.5f, 0.5f, 1.0f);
	/// <summary>
	/// The color to use to mark the onion skinning motion path arrow.
	/// </summary>
	[DefaultValue(typeof(Color), "1.0,1.0,1.0,1.0")]
	[EditorDisplay("Onion skinning")]
	public Color OnionSkinArrowColor = new(1.0f, 1.0f, 1.0f, 1.0f);
}
#endif