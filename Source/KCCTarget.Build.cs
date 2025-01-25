using Flax.Build;

/// <inheritdoc />
public class KCCTarget : GameProjectTarget
{
    /// <inheritdoc />
    public override void Init()
    {
        base.Init();

        // Reference the modules for game
        Modules.Add("KCC");
    }
}
