using UnityEngine;

public class PlayerOutOfRange : BehaviourCondition
{
    private Transform enemy, player;
    private float maxRange;

    public PlayerOutOfRange(Transform enemy, Transform player, float range)
    {
        this.enemy = enemy;
        this.player = player;
        maxRange = range;
    }

    public override bool Evaluate()
    {
        return Vector3.Distance(enemy.position, player.position) > maxRange;
    }
}