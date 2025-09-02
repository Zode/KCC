#if FLAX_EDITOR
using System.ComponentModel;
using FlaxEngine;

namespace KCC;

/// <summary>
/// KCC Debugger options (wow what an amazing comment).
/// </summary>
[CustomEditor(typeof(KCCDebuggerOptionsEditor))]
[HideInEditor]
public class KCCDebuggerOptions
{
	/// <summary>
	/// Draw characters with flax's debug draw while selected.
	/// </summary>
	[DefaultValue(true)]
	[EditorDisplay("General")]
	[EditorOrder(0)]
	public bool FlaxDrawActorDebug = true;
	/// <summary>
	/// The color to use for the character using flax's debug draw while selected.
	/// </summary>
	[DefaultValue(typeof(Color), "0.6,0.8,0.19,1.0")]
	[EditorDisplay("General")]
	[EditorOrder(1)]
	public Color FlaxActorColor = new(0.6f, 0.8f, 0.19f, 1.0f);
	/// <summary>
	/// The color to use to indicate the forward orientation of the character.
	/// </summary>
	[DefaultValue(typeof(Color), "0.26,0.0,1.0,0.5")]
	[EditorDisplay("General")]
	[EditorOrder(2)]
	public Color ForwardArrowColor = new(0.26f, 0.0f, 1.0f, 0.5f);
	/// <summary>
	/// The color to use to draw the delta passed to the character.
	/// </summary>
	[DefaultValue(typeof(Color), "0.0,1.0,0.0,1.0")]
	[EditorDisplay("General")]
	[EditorOrder(3)]
	public Color DeltaArrowColor = new(0.0f, 1.0f, 0.0f, 1.0f);
	/// <summary>
	/// General text color.
	/// </summary>
	[DefaultValue(typeof(Color), "1.0,1.0,1.0,1.0")]
	[EditorDisplay("General")]
	[EditorOrder(4)]
	public Color TextColor = new(1.0f, 1.0f, 1.0f, 1.0f);
	/// <summary>
	/// General font size.
	/// </summary>
	[DefaultValue(24)]
	[EditorDisplay("General")]
	[EditorOrder(5)]
	public int TextFontSize = 24;
	/// <summary>
	/// General font scaling.
	/// </summary>
	[DefaultValue(0.5f)]
	[EditorDisplay("General")]
	[EditorOrder(6)]
	public float TextScale = 0.5f;

	/// <summary>
	/// The fill color to use to mark the character sweep pass start position.
	/// </summary>
	[DefaultValue(typeof(Color), "0.0,0.0,0.0,0.0")]
	[EditorDisplay("Sweep")]
	[EditorOrder(100)]
	public Color SweepStartFillColor = new(0.0f, 0.0f, 0.0f, 0.0f);
	/// <summary>
	/// The outline color to use to mark the character sweep pass start position.
	/// </summary>
	[DefaultValue(typeof(Color), "0.0,1.0,0.4,0.5")]
	[EditorDisplay("Sweep")]
	[EditorOrder(101)]
	public Color SweepStartOutlineColor = new(0.0f, 1.0f, 0.4f, 0.5f);
	/// <summary>
	/// The color to use to mark the character sweep pass arrow.
	/// </summary>
	[DefaultValue(typeof(Color), "0.0,1.0,0.4,0.5")]
	[EditorDisplay("Sweep")]
	[EditorOrder(102)]
	public Color SweepArrowColor = new(0.0f, 1.0f, 0.4f, 0.5f);
	/// <summary>
	/// The fill color to use to mark the character sweep pass end position.
	/// </summary>
	[DefaultValue(typeof(Color), "0.0,0.0,0.0,0.0")]
	[EditorDisplay("Sweep")]
	[EditorOrder(103)]
	public Color SweepEndFillColor = new(0.0f, 0.0f, 0.0f, 0.0f);
	/// <summary>
	/// The outline color to use to mark the character sweep pass end position.
	/// </summary>
	[DefaultValue(typeof(Color), "0.0,1.0,0.4,0.5")]
	[EditorDisplay("Sweep")]
	[EditorOrder(104)]
	public Color SweepEndOutlineColor = new(0.0f, 1.0f, 0.4f, 0.5f);
	/// <summary>
	/// The color to use to mark contact normal made during sweeping.
	/// </summary>
	[DefaultValue(typeof(Color), "1.0,0.0,0.0,1.0")]
	[EditorDisplay("Sweep")]
	[EditorOrder(105)]
	public Color ContactArrowColor = new(1.0f, 0.0f, 0.0f, 1.0f);
	/// <summary>
	/// The fill color to use to mark contact normal plane made during sweeping.
	/// </summary>
	[DefaultValue(typeof(Color), "1.0,0.0,0.0,0.15")]
	[EditorDisplay("Sweep")]
	[EditorOrder(106)]
	public Color ContactQuadFillColor = new(1.0f, 0.0f, 0.0f, 0.15f);
	/// <summary>
	/// The outline color to use to mark contact normal plane made during sweeping.
	/// </summary>
	[DefaultValue(typeof(Color), "1.0,0.0,0.0,1.0")]
	[EditorDisplay("Sweep")]
	[EditorOrder(107)]
	public Color ContactQuadOutlineColor = new(1.0f, 0.0f, 0.0f, 1.0f);
	/// <summary>
	/// The color to use to mark sliding plane normal used during sweeping.
	/// </summary>
	[DefaultValue(typeof(Color), "1.0,0.5,0.0,1.0")]
	[EditorDisplay("Sweep")]
	[EditorOrder(108)]
	public Color SlidingPlaneArrowColor = new(1.0f, 0.5f, 0.0f, 1.0f);
	/// <summary>
	/// The fill color to use to mark sliding plane used during sweeping.
	/// </summary>
	[DefaultValue(typeof(Color), "1.0,0.5,0.0,0.10")]
	[EditorDisplay("Sweep")]
	[EditorOrder(109)]
	public Color SlidingPlaneQuadFillColor = new(1.0f, 0.5f, 0.0f, 0.10f);
	/// <summary>
	/// The outline color to use to mark sliding plane used during sweeping.
	/// </summary>
	[DefaultValue(typeof(Color), "1.0,0.5,0.0,1.0")]
	[EditorDisplay("Sweep")]
	[EditorOrder(110)]
	public Color SlidingPlaneQuadOutlineColor = new(1.0f, 0.5f, 0.0f, 1.0f);
	/// <summary>
	/// The  color to use to mark the crease used during second sweeping pass.
	/// </summary>
	[DefaultValue(typeof(Color), "1.0,0.0,1.0,1.0")]
	[EditorDisplay("Sweep")]
	[EditorOrder(111)]
	public Color CreaseLineColor = new(1.0f, 0.0f, 1.0f, 1.0f);

	/// <summary>
	/// The fill color to use to mark the character cast start position.
	/// </summary>
	[DefaultValue(typeof(Color), "0.0,0.0,0.0,0.0")]
	[EditorDisplay("Casting")]
	[EditorOrder(200)]
	public Color CastStartFillColor = new(0.0f, 0.0f, 0.0f, 0.0f);
	/// <summary>
	/// The outline color to use to mark the character cast start position.
	/// </summary>
	[DefaultValue(typeof(Color), "0.0,0.0,1.0,0.5")]
	[EditorDisplay("Casting")]
	[EditorOrder(201)]
	public Color CastStartOutlineColor = new(0.0f, 0.0f, 1.0f, 0.5f);
	/// <summary>
	/// The color to use to mark the character cast line.
	/// </summary>
	[DefaultValue(typeof(Color), "0.0,0.5,1.0,0.5")]
	[EditorDisplay("Casting")]
	[EditorOrder(202)]
	public Color CastLineColor = new(0.0f, 0.5f, 1.0f, 0.5f);
	/// <summary>
	/// The fill color to use to mark the character cast end position.
	/// </summary>
	[DefaultValue(typeof(Color), "0.0,0.0,0.0,0.0")]
	[EditorDisplay("Casting")]
	[EditorOrder(203)]
	public Color CastEndFillColor = new(0.0f, 0.0f, 0.0f, 0.0f);
	/// <summary>
	/// The outline color to use to mark the character cast end position.
	/// </summary>
	[DefaultValue(typeof(Color), "0.0,0.5,1.0,0.5")]
	[EditorDisplay("Casting")]
	[EditorOrder(204)]
	public Color CastEndOutlineColor = new(0.0f, 0.5f, 1.0f, 0.5f);
	/// <summary>
	/// The fill color to use to mark the character cast end result position.
	/// </summary>
	[DefaultValue(typeof(Color), "0.0,0.0,0.0,0.0")]
	[EditorDisplay("Casting")]
	[EditorOrder(205)]
	public Color CastResultFillColor = new(0.0f, 0.0f, 0.0f, 0.0f);
	/// <summary>
	/// The outline color to use to mark the character cast end result position.
	/// </summary>
	[DefaultValue(typeof(Color), "0.0,0.8,1.0,0.8")]
	[EditorDisplay("Casting")]
	[EditorOrder(206)]
	public Color CastResultOutlineColor = new(0.0f, 0.8f, 1.0f, 0.8f);
	/// <summary>
	/// The color to use to mark the character cast end result line.
	/// </summary>
	[DefaultValue(typeof(Color), "0.0,0.8,1.0,1.0")]
	[EditorDisplay("Casting")]
	[EditorOrder(207)]
	public Color CastResultLineColor = new(0.0f, 0.8f, 1.0f, 1.0f);

	/// <summary>
	/// The fill color to use to mark the character during overlap.
	/// </summary>
	[DefaultValue(typeof(Color), "1.0,1.0,0.0,0.10")]
	[EditorDisplay("Overlapping")]
	[EditorOrder(300)]
	public Color OverlapFillColor = new(1.0f, 1.0f, 0.0f, 0.10f);
	/// <summary>
	/// The outline color to use to mark the character during overlap.
	/// </summary>
	[DefaultValue(typeof(Color), "1.0,1.0,0.0,1.0")]
	[EditorDisplay("Overlapping")]
	[EditorOrder(301)]
	public Color OverlapOutlineColor = new(1.0f, 1.0f, 0.0f, 1.00f);
	/// <summary>
	/// The fill color to use to mark the other collider during overlap.
	/// </summary>
	[DefaultValue(typeof(Color), "1.0,0.0,1.0,0.10")]
	[EditorDisplay("Overlapping")]
	[EditorOrder(302)]
	public Color OverlapOtherFillColor = new(1.0f, 0.0f, 1.0f, 0.10f);
	/// <summary>
	/// The outline color to use to mark the other collider during overlap.
	/// </summary>
	[DefaultValue(typeof(Color), "1.0,0.0,1.0,1.0")]
	[EditorDisplay("Overlapping")]
	[EditorOrder(303)]
	public Color OverlapOtherOutlineColor = new(1.0f, 0.0f, 1.0f, 1.00f);

	/// <summary>
	/// The fill color to use to mark the character during stairstep pass.
	/// </summary>
	[DefaultValue(typeof(Color), "0.0,0.0,0.0,0.0")]
	[EditorDisplay("Stairstepping")]
	[EditorOrder(400)]
	public Color StairstepFillColor = new(0.0f, 0.0f, 0.0f, 0.0f);
	/// <summary>
	/// The outline color to use to mark the character during stairstep pass.
	/// </summary>
	[DefaultValue(typeof(Color), "0.0,1.0,1.0,0.5")]
	[EditorDisplay("Stairstepping")]
	[EditorOrder(401)]
	public Color StairstepOutlineColor = new(0.0f, 1.0f, 1.0f, 0.5f);

	/// <summary>
	/// The color to use to mark unstuck solve amount for a single overlapping collider.
	/// </summary>
	[DefaultValue(typeof(Color), "0.5,0.0,1.0,0.5")]
	[EditorDisplay("Unstuck")]
	[EditorOrder(500)]
	public Color UnstuckSingularArrowColor = new(0.5f, 0.0f, 1.0f, 0.5f);
	/// <summary>
	/// The color to use to mark unstuck solve total amount.
	/// </summary>
	[DefaultValue(typeof(Color), "0.5,0.0,1.0,1.0")]
	[EditorDisplay("Unstuck")]
	[EditorOrder(501)]
	public Color UnstuckTotalArrowColor = new(0.5f, 0.0f, 1.0f, 1.0f);
	/// <summary>
	/// The fill color to use to mark the character during unstuck rescue.
	/// </summary>
	[DefaultValue(typeof(Color), "0.0,0.0,0.0,0.0")]
	[EditorDisplay("Unstuck")]
	[EditorOrder(502)]
	public Color UnstuckRescueFillColor = new(0.0f, 0.0f, 0.0f, 0.0f);
	/// <summary>
	/// The outline color to use to mark the character during unstuck rescue.
	/// </summary>
	[DefaultValue(typeof(Color), "1.0,0.0,0.0,1.0")]
	[EditorDisplay("Unstuck")]
	[EditorOrder(503)]
	public Color UnstuckRescueOutlineColor = new(1.0f, 0.0f, 0.0f, 1.0f);
	/// <summary>
	/// The fill color to use to mark the other collider during compute penetration triangles pass.
	/// </summary>
	[DefaultValue(typeof(Color), "0.5,0.0,1.0,0.15")]
	[EditorDisplay("Unstuck")]
	[EditorOrder(504)]
	public Color PenetrationFillColor = new(0.5f, 0.0f, 1.0f, 0.15f);
	/// <summary>
	/// The outline color to use to mark the other collider during compute penetration triangles pass.
	/// </summary>
	[DefaultValue(typeof(Color), "0.5,0.0,1.0,1.0")]
	[EditorDisplay("Unstuck")]
	[EditorOrder(505)]
	public Color PenetrationOutlineColor = new(0.5f, 0.0f, 1.0f, 1.0f);
	/// <summary>
	/// The color to use to mark the character's linecast during compute penetration triangles pass or ground snap pass.
	/// </summary>
	[DefaultValue(typeof(Color), "1.0,0.0,0.0,1.0")]
	[EditorDisplay("Unstuck")]
	[EditorOrder(506)]
	public Color PenetrationTraceColor = new(1.0f, 0.0f, 0.0f, 1.0f);
	/// <summary>
	/// The color to use to mark the other collider's linecast during compute penetration triangles pass or ground snap pass.
	/// </summary>
	[DefaultValue(typeof(Color), "0.5,0.0,0.0,1.0")]
	[EditorDisplay("Unstuck")]
	[EditorOrder(507)]
	public Color PenetrationTraceOtherColor = new(0.5f, 0.0f, 0.0f, 1.0f);

	/// <summary>
	/// The fill color to use to mark the character onion skinning position.
	/// </summary>
	[DefaultValue(typeof(Color), "0.0,0.0,0.0,0.0")]
	[EditorDisplay("Onion skinning")]
	[EditorOrder(600)]
	public Color OnionSkinFillColor = new(0.0f, 0.0f, 0.0f, 0.0f);
	/// <summary>
	/// The outline color to use to mark the character onion skinning position.
	/// </summary>
	[DefaultValue(typeof(Color), "0.75,0.75,0.75,0.3")]
	[EditorDisplay("Onion skinning")]
	[EditorOrder(601)]
	public Color OnionSkinOutlineColor = new(0.75f, 0.75f, 0.75f, 0.3f);
	/// <summary>
	/// The color to use to mark the onion skinning motion path arrow.
	/// </summary>
	[DefaultValue(typeof(Color), "1.0,1.0,1.0,0.75")]
	[EditorDisplay("Onion skinning")]
	[EditorOrder(602)]
	public Color OnionSkinArrowColor = new(1.0f, 1.0f, 1.0f, 0.75f);
}
#endif