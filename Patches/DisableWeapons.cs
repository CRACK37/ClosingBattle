using ClosingBattle.Weapons;
using HarmonyLib;
using UnityEngine;

namespace ClosingBattle.Patches;

[HarmonyPatch]
public static class DisableWeapons
{
    [HarmonyPatch(typeof(GunSetter), "Start"), HarmonyPrefix]
    private static bool GunSetterStart(
        ref GunSetter __instance)
    {
        if (!ConfigManager.useModWeapons.value)
            return true;

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

        // Add our own weapons
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "MyCube";
        cube.GetComponent<BoxCollider>().enabled = false;
        cube.transform.position = new Vector3(-1.0f, -0.5f, 0f);
        int tempSlot = 1;
        
        GameObject gameObject = Object.Instantiate<GameObject>(cube, MonoSingleton<GunControl>.Instance.transform);
        gameObject.AddComponent<Fists>();
        MonoSingleton<GunControl>.Instance.slots[tempSlot].Add(gameObject);
        MonoSingleton<GunControl>.Instance.ForceWeapon(cube, true);
        MonoSingleton<GunControl>.Instance.UpdateWeaponList(false);


        return false;
    }
}