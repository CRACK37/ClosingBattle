using HarmonyLib;

namespace ClosingBattle.Patches;

[HarmonyPatch]
public static class DisableWeapons
{
    [HarmonyPatch(typeof(GunSetter), "Start"), HarmonyPrefix]
    private static void GunSetterStart(
        ref GunSetter __instance)
    {
        //Reset all weapons so we can never equip any of them
        __instance.revolverPierce = [];
        __instance.revolverRicochet = [];
        __instance.revolverTwirl = [];

        __instance.shotgunGrenade = [];
        __instance.shotgunPump = [];
        __instance.shotgunRed = [];

        __instance.nailMagnet = [];
        __instance.nailOverheat = [];
        __instance.nailRed = [];

        __instance.railCannon = [];
        __instance.railHarpoon = [];
        __instance.railMalicious = [];

        __instance.rocketBlue = [];
        __instance.rocketGreen = [];
        __instance.rocketRed = [];
    }
    
    
}