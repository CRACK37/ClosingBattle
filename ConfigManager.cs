using PluginConfig.API;
using PluginConfig.API.Fields;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace ClosingBattle;

public static class ConfigManager
{
    public static PluginConfigurator config = null;

    public static BoolField disableMainWeapons;

    public static void Initialize()
    {
        if (config != null)
            return;

        config = PluginConfigurator.Create(Plugin.NAME, Plugin.GUID);

        disableMainWeapons = new BoolField(config.rootPanel, "Disable base game weapons", "disableBaseGameWeapons", true);
    }
}
