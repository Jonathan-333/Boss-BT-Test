using UnityEngine;

public class PlayerInAttackRange : BehaviourCondition
{
    private Transform enemy, player;
    private float attackRange;

    public PlayerInAttackRange(Transform enemy, Transform player, float range)
    {
        this.enemy = enemy;
        this.player = player;
        attackRange = range;
    }

    public override bool Evaluate()
    {
        return Vector3.Distance(enemy.position, player.position) < attackRange;
    }
}