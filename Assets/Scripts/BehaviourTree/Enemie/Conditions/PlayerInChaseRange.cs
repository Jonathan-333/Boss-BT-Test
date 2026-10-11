using UnityEngine;

public class PlayerInChaseRange : BehaviourCondition
{
    private Transform enemy, player;
    private float chaseRange;

    public PlayerInChaseRange(Transform enemy, Transform player, float range)
    {
        this.enemy = enemy;
        this.player = player;
        chaseRange = range;
    }

    public override bool Evaluate()
    {
        return Vector3.Distance(enemy.position, player.position) < chaseRange;
    }
}