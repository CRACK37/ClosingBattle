using System;
using BepInEx;
using BepInEx.Logging;
using UnityEngine;

namespace ClosingBattle;

[BepInPlugin(GUID, NAME, VERS)]
public class Plugin : BaseUnityPlugin
{
    public const string GUID = "Q2xvc2luZ0JhdHRsZV9f";
    public const string NAME = "ClosingBattle";
    public const string VERS = "1.0.0";
    
    internal new static ManualLogSource Logger { get; private set; } = null!;
    
    private void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {GUID} is loaded!");
        gameObject.hideFlags = HideFlags.DontSaveInEditor;
    }
}