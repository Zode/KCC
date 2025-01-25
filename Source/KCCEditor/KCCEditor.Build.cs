using Flax.Build;
using Flax.Build.NativeCpp;

/// <inheritdoc />
public class KCCEditor : GameEditorModule
{
    /// <inheritdoc />
    public override void Setup(BuildOptions options)
    {
        base.Setup(options);

        // Reference game source module to access game code types
        options.PublicDependencies.Add("KCC");

        // Here you can modify the build options for your game editor module
        // To reference another module use: options.PublicDependencies.Add("Audio");
        // To add C++ define use: options.PublicDefinitions.Add("COMPILE_WITH_FLAX");
        // To learn more see scripting documentation.
        BuildNativeCode = false;
    }
}
