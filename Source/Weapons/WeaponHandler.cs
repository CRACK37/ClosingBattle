using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem.HID;

namespace ClosingBattle.Weapons;

[ConfigureSingleton(SingletonFlags.DestroyDuplicates)]
public class WeaponHandler : MonoSingleton<WeaponHandler>
{
    public float MeleeHitboxSize = 10f;

    public void Awake()
    {
        base.Awake();
        CreateMeleeHitbox();
        CreateBigMeleeHitbox();
    }

    public void CreateMeleeHitbox()
    {
        var size = MeleeHitboxSize;

        GameObject meleeHitbox = new GameObject("MeleeHitbox");
        meleeHitbox.transform.SetParent(FistControl.Instance.transform, false);

        meleeHitbox.transform.localPosition = new Vector3();
        meleeHitbox.transform.localRotation = Quaternion.identity;
        meleeHitbox.transform.localScale = Vector3.one;

        var bc = meleeHitbox.AddComponent<BoxCollider>();
        bc.isTrigger = true;

        meleeHitbox.transform.localScale = new Vector3(size, size, size);
        meleeHitbox.transform.localPosition = new Vector3(0f, 0f, size / 2f);

        meleeHitbox.AddComponent<MeleeHitbox>();
    }

    public void CreateBigMeleeHitbox()
    {
        var size = MeleeHitboxSize;

        GameObject meleeHitbox = new GameObject("BigMeleeHitbox");
        meleeHitbox.transform.SetParent(FistControl.Instance.transform, false);

        meleeHitbox.transform.localPosition = new Vector3();
        meleeHitbox.transform.localRotation = Quaternion.identity;
        meleeHitbox.transform.localScale = Vector3.one;

        var bc = meleeHitbox.AddComponent<BoxCollider>();
        bc.isTrigger = true;

        meleeHitbox.transform.localScale = new Vector3(size * 2f, size, size * 2f);
        meleeHitbox.transform.localPosition = new Vector3(0f, 0f, size);

        meleeHitbox.AddComponent<BigMeleeHitbox>();
    }
}
