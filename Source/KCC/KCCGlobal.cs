using FlaxEngine;

namespace KCC;
#nullable enable

/// <summary>
/// Evil static class for caching stuff on global level 
/// </summary>
public static class KCCGlobal
{
	/// <summary>
	/// Reference to the KCC Plugin
	/// </summary>
	public static KCC Plugin
	{
		get
		{
			if(_plugin is not null)
			{
				return _plugin;
			}
			else
			{
				_plugin = PluginManager.GetPlugin<KCC>();
				return _plugin;				
			}
		}
	}
	private static KCC? _plugin;
}