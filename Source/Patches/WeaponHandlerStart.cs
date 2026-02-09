using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using ClosingBattle.Core;
using ClosingBattle.Weapons;

namespace ClosingBattle.Patches;

[PatchOnEntry]
[HarmonyPatch]
public class WeaponHandlerStart
{
    [HarmonyPatch(typeof(GunControl), nameof(GunControl.Start)), HarmonyPrefix]
    private static void weaponHandlerStart()
    {
        WeaponHandler.Instance.Awake();
    }
}
