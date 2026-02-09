using GameConsole.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ClosingBattle.Weapons;

[ConfigureSingleton(SingletonFlags.DestroyDuplicates | SingletonFlags.NoAutoInstance)]
public class MeleeHitbox : MonoSingleton<MeleeHitbox>
{
    public List<GameObject> targetList = new List<GameObject>();
    public List<GameObject> projectileList = new List<GameObject>();

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 14 
            && other.gameObject.GetComponentInChildren<Projectile>() != null)
        {
            projectileList.Add(other.gameObject);
            return;
        }

        if (other.gameObject.layer == 12
            && other.gameObject.GetComponentInChildren<EnemyIdentifier>() != null)
        {
            targetList.Add(other.gameObject);
            return;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 14
            && projectileList.Contains(other.gameObject))
        {
            projectileList.Remove(other.gameObject);
            return;
        }

        if (other.gameObject.layer == 12
            && targetList.Contains(other.gameObject))
        {
            targetList.Remove(other.gameObject);
            return;
        }
    }

    public Projectile CheckMeleeHitboxProjectiles()
    {
        Projectile projectile = null;
        List<GameObject> list = new List<GameObject>();

        if (projectileList.Count <= 0)
            return null;

        float num = 100f;
        foreach (GameObject proj in projectileList)
        {
            if (proj != null && proj.activeInHierarchy)
            {
                projectile = proj.GetComponentInChildren<Projectile>();
                if (projectile != null && !projectile.undeflectable)
                {
                    num = Vector3.Distance(base.transform.parent.position, proj.transform.position);
                }
                else
                {
                    list.Add(proj);
                }
            }
            else if (proj == null || !proj.activeInHierarchy)
            {
                list.Add(proj);
            }
        }

        if (list.Count > 0)
        {
            foreach (GameObject item in list)
            {
                projectileList.Remove(item);
            }
        }

        return projectile;
    }

    public List<EnemyIdentifier> CheckMeleeHitboxEnemies()
    {
        List<EnemyIdentifier> enemyIdentifiers = new List<EnemyIdentifier>();
        List<GameObject> list = new List<GameObject>();

        if (targetList.Count <= 0)
            return enemyIdentifiers;

        foreach (GameObject target in targetList)
        {
            if (target != null
                && target.activeInHierarchy)
            {
                var enemyIdentifier = target.GetComponentInChildren<EnemyIdentifier>();
                if (enemyIdentifier != null
                    && !enemyIdentifier.dead)
                {
                    enemyIdentifiers.Add(enemyIdentifier);
                }
            }
            else if (target == null || !target.activeInHierarchy)
            {
                list.Add(target);
            }
        }

        if (list.Count > 0)
        {
            foreach (GameObject item in list)
            {
                targetList.Remove(item);
            }
        }

        return enemyIdentifiers;
    }
}
