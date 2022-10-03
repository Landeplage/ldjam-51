using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HexDirection
{
    Right,
    UpRight,
    UpLeft,
    Left,
    DownLeft,
    DownRight
};

static class HexDirectionUtils
{
    public static HexDirection GetDirectionTo(Vector3 from, Vector3 to)
    {
        var startToEnd = Vector3.Normalize(to - from);
        float angle = Vector3.SignedAngle(
            new Vector3(1.0f, 0.0f, 0.0f), 
            new Vector3(startToEnd.x, startToEnd.y, 0.0f),
            Vector3.forward
        );
        if (angle <= 30.0 && angle > -30.0)
        {
            return HexDirection.Right;
        }
        else if (angle <= 90.0 && angle > 30.0)
        {
            return HexDirection.UpRight;
        }
        else if (angle <= 150.0 && angle > 90.0)
        {
            return HexDirection.UpLeft;
        }
        else if (angle <= -30.0 && angle > -90.0f)
        {
            return HexDirection.DownRight;
        }
        else if (angle <= -90.0f && angle > -150.0f)
        {
            return HexDirection.DownLeft;
        }
        return HexDirection.Left;
    }
}

public class BoardEntity : MonoBehaviour
{
    public BoardSquareType type;
    private Animator animator;

    public GameObject friendlyMeleeVisuals;
    public GameObject friendlyRangedVisuals;
    public GameObject friendlyHealerVisuals;
    public GameObject enemyVisuals;
    public GameObject wellVisuals;
    public GameObject enemyWellVisuals;

    public GameObject friendlyRangedProjectile;
    public GameObject healAura;

    [System.NonSerialized]
    public int maxHealth;
    [System.NonSerialized]
    public int health;

    private GUI_Healthbar healthbar;

    public void Create()
    {
        if (type == BoardSquareType.Well)
        {
            Instantiate(enemyWellVisuals).transform.SetParent(transform, false);
        }
        else if (type == BoardSquareType.EnemyWell)
        {
            Instantiate(wellVisuals).transform.SetParent(transform, false);
        }
        else if (type == BoardSquareType.FriendlyMelee)
        {
            Instantiate(friendlyMeleeVisuals).transform.SetParent(transform, false);
        }
        else if (type == BoardSquareType.FriendlyRange)
        {
            Instantiate(friendlyRangedVisuals).transform.SetParent(transform, false);
        }
        else if (type == BoardSquareType.FriendlyHealer)
        {
            Instantiate(friendlyHealerVisuals).transform.SetParent(transform, false);
        }
        else if (type == BoardSquareType.Enemy)
        {
            Instantiate(enemyVisuals).transform.SetParent(transform, false);
        }

        healthbar = GetComponentInChildren<GUI_Healthbar>();
        if (healthbar)
        {
            healthbar.UpdateBar(health, maxHealth);
        }
        animator = GetComponentInChildren<Animator>();
    }

    public void Hurt(int amount)
    {
        health = (int)Mathf.Max(health - amount, 0.0f);
        if (healthbar)
        {
            healthbar.UpdateBar(health, maxHealth);
        }
    }

    public void Heal(int amount)
    {
        health = (int)Mathf.Min(health + amount, maxHealth);
        if (healthbar)
        {
            healthbar.UpdateBar(health, maxHealth);
        }
    }

    public bool Dead()
    {
        return health == 0;
    }
    
    public IEnumerator Play(string animation)
    {
        if (animator == null)
        {
            yield break;
        }

        animator.Play(animation);
        yield return null;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(animation))
        {
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length - 0.01f);
        }
        else
        {
            Debug.Log("Did not play " + animation);
        }
        animator.Play("Idle");
    }

    public IEnumerator Attack(Vector3 position, Vector3 targetPosition)
    {
        if (type == BoardSquareType.FriendlyMelee || type == BoardSquareType.Enemy)
        {
            HexDirection dir = HexDirectionUtils.GetDirectionTo(position, targetPosition);
            yield return Play("Attack" + dir.ToString());
        }
        else if (type == BoardSquareType.FriendlyRange)
        {
            var projectile = Instantiate(friendlyRangedProjectile);
            projectile.transform.position = position;
            projectile.GetComponent<Projectile>().target = new Vector3(targetPosition.x, targetPosition.y, friendlyRangedProjectile.transform.position.z);
            var angle = Mathf.Atan2((targetPosition - position).y, (targetPosition - position).x) * 180.0f / 3.14159f;
            projectile.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            yield return Play("Attack");
        }
        else if (type == BoardSquareType.FriendlyHealer)
        {
            var aura = Instantiate(healAura);
            aura.transform.position = targetPosition;
            yield return Play("Attack");
        }
    }
}
