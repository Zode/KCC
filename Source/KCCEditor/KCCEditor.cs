using FlaxEditor;
using FlaxEditor.Content;

namespace KCC;

/// <summary>
/// </summary>
public class KCCEditor : EditorPlugin
{
	private CustomSettingsProxy _settingsProxy;

	/// <inheritdoc />
    public override void InitializeEditor()
    {
        base.InitializeEditor();

		_settingsProxy = new CustomSettingsProxy(typeof(KCCSettings), "KCC Settings");
		Editor.ContentDatabase.AddProxy(_settingsProxy);
    }

	/// <inheritdoc />
    public override void DeinitializeEditor()
    {
		Editor.ContentDatabase.RemoveProxy(_settingsProxy);

        base.DeinitializeEditor();
    }
}