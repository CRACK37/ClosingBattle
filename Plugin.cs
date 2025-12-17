using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using UnityEngine;

using ClosingBattle.Patches;

namespace ClosingBattle;

[BepInPlugin(GUID, NAME, VERS)]
public class Plugin : BaseUnityPlugin
{
    public const string GUID = "Q2xvc2luZ0JhdHRsZV9f";
    public const string NAME = "ClosingBattle";
    public const string VERS = "1.0.1";
    
    internal new static ManualLogSource Logger { get; private set; } = null!;

    public static Harmony harmony;

    private void Awake()
    {
        if (harmony == null)
            harmony = new Harmony(GUID);

        harmony.PatchAll();

        Logger = base.Logger;
        Logger.LogInfo($"Plugin {GUID} is loaded!");
        gameObject.hideFlags = HideFlags.DontSaveInEditor;
    }
}