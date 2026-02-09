using System;
using System.Collections.Generic;
using System.Drawing;
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
    private GunControl gc;

    private void Start()
    {
        targeter = CameraFrustumTargeter.Instance;
        inman = InputManager.Instance;
        cam = CameraController.Instance.GetComponent<Camera>();
        cc = CameraController.Instance;
        wid = gameObject.GetOrAddComponent<WeaponIdentifier>();

        gc = GunControl.Instance;
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

    private void FeedBackerPunch(Vector3 point, Transform target)
    {
        EnemyIdentifier enemyIdentifier2;
        if (target.gameObject.CompareTag("Enemy") 
            || target.gameObject.CompareTag("Armor") 
            || target.gameObject.CompareTag("Head") 
            || target.gameObject.CompareTag("Body") 
            || target.gameObject.CompareTag("Limb") 
            || target.gameObject.CompareTag("EndLimb"))
        {
            UnityEngine.Object.Instantiate<AudioSource>(FistControl.Instance.currentPunch.heavyHit, base.transform.position, Quaternion.identity);
            MonoSingleton<TimeController>.Instance.HitStop(0.1f);

            EnemyIdentifier enemyIdentifier = null;
            EnemyIdentifierIdentifier enemyIdentifierIdentifier = null;
            if (target.TryGetComponent<EnemyIdentifierIdentifier>(out enemyIdentifierIdentifier))
            {
                enemyIdentifier = enemyIdentifierIdentifier.eid;
            }

            if (enemyIdentifier)
            {
                enemyIdentifier.hitter = "this.hitter";
                enemyIdentifier.DeliverDamage(target.gameObject, cam.transform.forward * 1000f, point, 2.0f, false, 0f, null, false, false);
            }
        }
        else if (target.TryGetComponent<EnemyIdentifier>(out enemyIdentifier2) && enemyIdentifier2.enemyType == EnemyType.Idol)
        {
            enemyIdentifier2.hitter = "this.hitter";
            enemyIdentifier2.DeliverDamage(target.gameObject, cam.transform.forward * 1000f, point, 1000.0f, false, 0f, null, false, false);
        }
    }

    private void SwingFeedBacker()
    {
        var enemies = MeleeHitbox.Instance.CheckMeleeHitboxEnemies();
        foreach (var enemy in enemies)
        {
            var limbs = enemy.GetComponentsInChildren<EnemyIdentifierIdentifier>();
            var currentDistance = float.MaxValue;
            Transform bestTarget = null;
            foreach (var limb in limbs)
            {
                float distance = Vector3.Distance(CameraController.Instance.transform.parent.position, limb.transform.position);
                if (distance < currentDistance)
                {
                    bestTarget = limb.transform;
                }
            }

            if (!bestTarget)
                continue;

            FeedBackerPunch(bestTarget!.GetComponent<Collider>().ClosestPoint(CameraController.Instance.transform.position), bestTarget); 
        }
    }

    private void SwingKnuckleBlaster()
    {
        var enemies = BigMeleeHitbox.Instance.CheckMeleeHitboxEnemies();
        foreach (var enemy in enemies)
        {
            var limbs = enemy.GetComponentsInChildren<EnemyIdentifierIdentifier>();
            var currentDistance = float.MaxValue;
            Transform bestTarget = null;
            foreach (var limb in limbs)
            {
                float distance = Vector3.Distance(CameraController.Instance.transform.parent.position, limb.transform.position);
                if (distance < currentDistance)
                {
                    bestTarget = limb.transform;
                }
            }

            if (!bestTarget)
                continue;

            FeedBackerPunch(bestTarget!.GetComponent<Collider>().ClosestPoint(CameraController.Instance.transform.position), bestTarget);
        }
    }
}