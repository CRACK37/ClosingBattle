using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ClosingBattle.Weapons;

public class Fists : MonoBehaviour
{
    private CameraFrustumTargeter targeter;
    private InputManager inman;
    private Camera cam;
    private CameraController cc;
    private WeaponIdentifier wid;

    public GunControl gc;

    private void Start()
    {
        targeter = MonoSingleton<CameraFrustumTargeter>.Instance;
        inman = MonoSingleton<InputManager>.Instance;
        cam = MonoSingleton<CameraController>.Instance.GetComponent<Camera>();
        cc = MonoSingleton<CameraController>.Instance;
        wid = gameObject.GetOrAddComponent<WeaponIdentifier>();

        gc = GunControl.Instance;
        Plugin.Logger.LogInfo($"WE IN THE MATRIX");
    }

    private void Update()
    {
        if (!gc.activated)
            return;

        if (inman.PerformingCheatMenuCombo() || GameStateManager.Instance.PlayerInputLocked)
            return;

        if (inman.InputSource.Fire1.IsPressed)
        {
            if (!wid || wid.delay == 0f)
            {
                SwingFeedBacker();
            }
            else
            {
                base.Invoke("SwingFeedBacker", wid.delay);
            }
        }

        if (inman.InputSource.Fire2.IsPressed)
        {
            if (!wid || wid.delay == 0f)
            {
                SwingKnuckleBlaster();
            }
            else
            {
                base.Invoke("SwingKnuckleBlaster", wid.delay);
            }
        }
    }

    private void SwingFeedBacker()
    {
        
    }

    private void SwingKnuckleBlaster()
    {

    }
}