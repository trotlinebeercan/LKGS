using System;
using System.Collections.Generic;
using System.Diagnostics;

using BC = BepInEx.Configuration;

namespace LKGS;

public class ConfigManager
{
    protected Dictionary<string, BC.ConfigDefinition> Entries { get; } = new Dictionary<string, BC.ConfigDefinition>();

    private string sCurrentSection = "";

    private BC.ConfigFile Config;
    public ConfigManager(BC.ConfigFile config)
    {
        Config = config;
    }

    public ConfigManager StartSection(string section)
    {
        Debug.Assert(sCurrentSection == "", "StartSection failed, current section was non-null");
        sCurrentSection = section;
        return this;
    }

    public void EndSection(string section)
    {
        Debug.Assert(section == sCurrentSection, "EndSection failed, current section was not equal to argument");
        sCurrentSection = "";
    }

    public ConfigManager Create<T>(
        string uuid, string text, T defaultValue, string description,
        BC.AcceptableValueBase acceptableValues,
        ConfigurationManagerAttributes tags,
        System.EventHandler onSettingChanged = null
    )
    {
        Debug.Assert(sCurrentSection != "", "Create failed, current section was null");

        BC.ConfigDefinition newConfDef = new BC.ConfigDefinition(sCurrentSection, uuid);
        Entries.Add(uuid, newConfDef);

        // order them in the same order they are initialized
        tags.Order = Config.Count;
        tags.DispName = text;

        BC.ConfigDescription newConfDesc = new BC.ConfigDescription(description, acceptableValues, tags);
        BC.ConfigEntry<T> newConf = Config.Bind(newConfDef, defaultValue, newConfDesc);
        newConf.SettingChanged += onSettingChanged;
        return this;
    }

    public T Get<T>(string key)
    {
        return (T)Config[Entries[key]].BoxedValue;
    }
}
