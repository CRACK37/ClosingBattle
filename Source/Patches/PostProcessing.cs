using ClosingBattle.Classes;
using HarmonyLib;
using UnityEngine;

namespace ClosingBattle.Patches;

[HarmonyPatch]
public class PostProcessing
{
    [HarmonyPatch(typeof(CameraController), "Start"), HarmonyPrefix]
    private static void CameraControllerStart(
        ref CameraController __instance)
    {
        //Applies custom PostProcessEffects to the current camera
        Transform vCam = __instance.transform.Find("Virtual Camera");
        if (vCam == null)
            vCam = __instance.transform.GetComponentInChildren<Camera>().transform;

        if (vCam == null)
        {
            Debug.LogError("Camera not found!");
            return;
        }
        
        //Only apply custom PostProcessEffects if needed
        if(vCam.gameObject.GetComponentInChildren<PostProcessEffects>() == null)
            vCam.gameObject.AddComponent<PostProcessEffects>();
    }
}