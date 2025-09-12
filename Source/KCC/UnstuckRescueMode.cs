namespace KCC;

/// <summary>
/// Unstuck rescue mode for KCC characters.
/// </summary>
public enum UnstuckRescueMode
{
	/// <summary>
	/// Don't do any sort of unstuck rescue, in exchange for some performance.
	/// </summary>
	Disabled,
	/// <summary>
	/// Do unstuck rescue, but with no pis-aller.
	/// </summary>
	NoPisAller,
	/// <summary>
	/// Do unstuck rescue, and move character forcibly to the relative forward direction as defined by orientation as pis-aller. 
	/// </summary>
	RelativeForward,
	/// <summary>
	/// Do unstuck rescue, and move character forcibly to the relative backward direction as defined by orientation as pis-aller. 
	/// </summary>
	RelativeBackward,
	/// <summary>
	/// Do unstuck rescue, and move character forcibly to the relative upward direction as defined by orientation as pis-aller. 
	/// </summary>
	RelativeUp,
	/// <summary>
	/// Do unstuck rescue, and move character forcibly to the relative downward direction as defined by orientation as pis-aller. 
	/// </summary>
	RelativeDown,
	/// <summary>
	/// Do unstuck rescue, and move character forcibly to the relative rightward direction as defined by orientation as pis-aller. 
	/// </summary>
	RelativeRight,
	/// <summary>
	/// Do unstuck rescue, and move character forcibly to the relative leftward direction as defined by orientation as pis-aller. 
	/// </summary>
	RelativeLeft,
	/// <summary>
	/// Do unstuck rescue, and move character forcibly to the world forward direction as pis-aller. 
	/// </summary>
	WorldForward,
	/// <summary>
	/// Do unstuck rescue, and move character forcibly to the world backward direction as pis-aller. 
	/// </summary>
	WorldBackward,
	/// <summary>
	/// Do unstuck rescue, and move character forcibly to the world upoward direction as pis-aller. 
	/// </summary>
	WorldUp,
	/// <summary>
	/// Do unstuck rescue, and move character forcibly to the world downward direction as pis-aller. 
	/// </summary>
	WorldDown,
	/// <summary>
	/// Do unstuck rescue, and move character forcibly to the world rightward direction as pis-aller. 
	/// </summary>
	WorldRight,
	/// <summary>
	/// Do unstuck rescue, and move character forcibly to the world leftward direction as pis-aller. 
	/// </summary>
	WorldLeft,
}