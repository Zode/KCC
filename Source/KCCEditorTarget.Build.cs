using Flax.Build;

/// <inheritdoc />
public class KCCEditorTarget : GameProjectEditorTarget
{
    /// <inheritdoc />
    public override void Init()
    {
        base.Init();

        // Reference the modules for editor
        Modules.Add("KCC");
        Modules.Add("KCCEditor");
    }
}
