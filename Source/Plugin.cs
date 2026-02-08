using BepInEx;
using BepInEx.Logging;
using ClosingBattle.Core;
using ClosingBattle.Effects;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using static ClosingBattle.Core.LogHelper;

namespace ClosingBattle;

[BepInPlugin(GUID, NAME, VERS)]
[BepInDependency("com.eternalUnion.pluginConfigurator")]
public class Plugin : BaseUnityPlugin
{
    public const string GUID = "Q2xvc2luZ0JhdHRsZV9f";
    public const string NAME = "ClosingBattle";
    public const string VERS = "1.0.3";
    
    private static List<Action> _assetBinds = new List<Action>();
    
    internal new static ManualLogSource Logger { get; private set; } = null!;

    public static string AddressableAssetPath { get; private set; } = "";
    public static bool HasLoaded { get; private set; }

    private void Awake()
    {
        Logger = base.Logger;
        LogInfo($"Plugin {GUID} is loaded!");
        gameObject.hideFlags = HideFlags.DontSaveInEditor;

        var harmony = new Harmony(GUID);

        ConfigManager.Initialize();

        foreach (var type in typeof(Plugin).Assembly.GetTypes())
        {
            if (type.GetCustomAttribute<PatchOnEntryAttribute>() != null) 
                harmony.PatchAll(type);
        }

        LogInfo("Patches applied");

        SceneManager.sceneLoaded += (s, m) =>
        {
            if (!HasLoaded && s.name == "Bootstrap")
            {
                string assemblyPath = Assembly.GetExecutingAssembly().Location;
                string folder = Path.GetDirectoryName(assemblyPath);
                string catalogPath = Path.Combine(folder, "Assets", "catalog.json");
                if (!File.Exists(catalogPath))
                {
                    LogError("No assets!");
                    return;
                }
                
                //TODO: Find out why Unity will not export with this in editor
                // Also this code does not even work
                string text = File.ReadAllText(catalogPath);
                string newText = text.Replace("{UnityEngine.AddressableAssets.Addressables.RuntimePath}\\\\StandaloneWindows64\\\\",
                    "{ClosingBattle.Plugin.AddressableAssetPath}\\\\StandaloneWindows64\\\\");
                
                if(text != newText)
                    File.WriteAllText(catalogPath, text);
                
                AddressableAssetPath = Path.Combine(folder, "Assets");
                LogInfo($"Loading assets at {catalogPath}...");
                Addressables.LoadContentCatalogAsync(catalogPath, true, "").Completed += OnAssetsLoaded;
            }
        };
    }

    public static void BindToAssetsLoaded(Action callback) //Might remove if not useful
    {
        if(HasLoaded)
        {
            callback();
            return;
        }

        _assetBinds.Add(callback);
    }

    private void OnAssetsLoaded(AsyncOperationHandle<IResourceLocator> rLocator)
    {
        HasLoaded = true;

        SlashEffectManager.Instance.Awake();

        foreach (Action action in _assetBinds)
        {
            action();
        }
    }
}