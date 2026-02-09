using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using UnityEngine;

namespace ClosingBattle.Weapons;

public class Fists : MonoBehaviour
{
    public float FistDamage = 5f;
    public float FistDelay = 0.2f;

    private float CurrentDelayKB = 0f;
    private float CurrentDelayFB = 0f;

    private Animation AnimationFB;
    private Animation AnimationKB;

    private AudioSource AudioSourceFB;
    private AudioSource AudioSourceKB;

    private GameObject KB;
    private GameObject FB;

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

        // Yes this makes quick switching a real ability
        CurrentDelayKB = FistDelay;
        CurrentDelayFB = FistDelay;

        FB = transform.Find("Arm Blue").gameObject;
        KB = transform.Find("Arm Red").gameObject;

        AnimationFB = FB.GetComponent<Animation>();
        AnimationKB = KB.GetComponent<Animation>();

        AudioSourceFB = FB.GetComponent<AudioSource>();
        AudioSourceKB = KB.GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!gc.activated)
            return;

        if (inman.PerformingCheatMenuCombo() || GameStateManager.Instance.PlayerInputLocked)
            return;

        CurrentDelayKB += Time.deltaTime;
        CurrentDelayFB += Time.deltaTime;

        if (inman.InputSource.Fire1.WasPerformedThisFrame)
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

        if (inman.InputSource.Fire2.WasPerformedThisFrame)
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

            EnemyIdentifier enemyIdentifier = null;
            EnemyIdentifierIdentifier enemyIdentifierIdentifier = null;
            if (target.TryGetComponent<EnemyIdentifierIdentifier>(out enemyIdentifierIdentifier))
            {
                enemyIdentifier = enemyIdentifierIdentifier.eid;
            }

            if (enemyIdentifier)
            {
                enemyIdentifier.hitter = "this.hitter";
                enemyIdentifier.DeliverDamage(target.gameObject, cam.transform.forward * 1000f, point, FistDamage, false, 0f, null, false, false);
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
        if (CurrentDelayFB < FistDelay)
            return;  

        CurrentDelayFB = 0f;

        PlayFBPunchAnimation();

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
        if (CurrentDelayKB < FistDelay)
            return;

        CurrentDelayKB = 0f;

        PlayKBPunchAnimation();

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

    private Coroutine KBPunch;

    public void PlayKBPunchAnimation()
    {
        if (KBPunch != null)
        {
            StopCoroutine(KBPunch);
            AnimationKB.Play("KBIdle");
            KB.gameObject.SetActive(false);
        }

        KBPunch = StartCoroutine(PlayKBPunchAnim());
    }

    IEnumerator PlayKBPunchAnim()
    {
        AudioSourceKB.Play();
        AnimationKB.Play("KBPunch");
        KB.gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(
            AnimationKB.GetClip("KBPunch").length
        );

        KB.gameObject.SetActive(false);
    }

    private Coroutine FBPunch;

    public void PlayFBPunchAnimation()
    {
        if (FBPunch != null)
        {
            StopCoroutine(FBPunch);
            AnimationFB.Play("FBIdle");
            FB.gameObject.SetActive(false);
        }

        FBPunch = StartCoroutine(PlayFBPunchAnim());
    }

    IEnumerator PlayFBPunchAnim()
    {
        AudioSourceFB.Play();
        AnimationFB.Play("FBPunch");
        FB.gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(
            AnimationFB.GetClip("FBPunch").length
        );

        AnimationFB.Play("FBIdle");

        FB.gameObject.SetActive(false);
    }
}