using System.Collections.Generic;
using FlaxEditor;
using FlaxEditor.Content;
using FlaxEditor.GUI.ContextMenu;
using FlaxEngine;
using FlaxEngine.GUI;

namespace KCC;

/// <summary>
/// </summary>
public class KCCEditor : EditorPlugin
{
    private const string DEBUGGER_NAME = "KCC Debugger";
	private CustomSettingsProxy _settingsProxy;
    private ContextMenuButton _debuggerButton;
    /// <summary>
    /// The KCC debugger window
    /// </summary>
    public KCCDebuggerWindow DebuggerWindow;

	/// <inheritdoc />
    public override void InitializeEditor()
    {
        base.InitializeEditor();

        Editor.Instance.Options.AddCustomSettings(DEBUGGER_NAME, () => new KCCDebuggerOptions());
        DebuggerWindow = new KCCDebuggerWindow();

		_settingsProxy = new CustomSettingsProxy(typeof(KCCSettings), DEBUGGER_NAME);
		Editor.ContentDatabase.AddProxy(_settingsProxy);

        _debuggerButton = Editor.UI.MenuWindow.ContextMenu.AddButton(DEBUGGER_NAME);
        _debuggerButton.Clicked += () => { DebuggerWindow.Show(); };

        //hack: swap position of the button and refresh the children
        List<Control> contextMenuChildren = Editor.UI.MenuWindow.ContextMenu.ItemsContainer.Children;
        int profilerIndex = contextMenuChildren.FindIndex(x => x is ContextMenuButton cb && cb.Text.Equals("Visual Script Debugger", System.StringComparison.Ordinal));
        if(profilerIndex != -1)
        {
            contextMenuChildren.Remove(_debuggerButton);
            contextMenuChildren.Insert(profilerIndex + 1, _debuggerButton);
            Editor.UI.MenuWindow.ContextMenu.SortButtons();
        }

        Scripting.Update += OnUpdate;
        Editor.Instance.SceneEditing.SelectionChanged += OnSceneEditingSelectionChanged;
        Editor.Instance.PlayModeBeginning += OnPlayModeBeginning;
        Editor.Instance.PlayModeEnd += OnPlayModeEnd;
    }

	/// <inheritdoc />
    public override void DeinitializeEditor()
    {
        Editor.Instance.PlayModeEnd -= OnPlayModeEnd;
        Editor.Instance.PlayModeBeginning -= OnPlayModeBeginning;
        Editor.Instance.SceneEditing.SelectionChanged -= OnSceneEditingSelectionChanged;
        Scripting.Update -= OnUpdate;

        _debuggerButton.Dispose();
        _debuggerButton = null;

		Editor.ContentDatabase.RemoveProxy(_settingsProxy);
        
        DebuggerWindow.Close();
        DebuggerWindow.Dispose();
        DebuggerWindow = null;
        Editor.Instance.Options.RemoveCustomSettings(DEBUGGER_NAME);

        base.DeinitializeEditor();
    }

    private void OnUpdate()
    {
        DebuggerWindow.DrawRenderables();
    }

    private void OnSceneEditingSelectionChanged()
    {
        DebuggerWindow.OnSceneEditingSelectionChanged();
    }

    private void OnPlayModeBeginning()
    {
        DebuggerWindow.OnPlayModeBeginning();
    }

    private void OnPlayModeEnd()
    {
        DebuggerWindow.OnPlayModeEnd();
    }
}