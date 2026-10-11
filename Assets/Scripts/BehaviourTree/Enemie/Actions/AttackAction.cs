using UnityEngine;

public class AttackAction : BehaviourNode
{
    private Transform enemyTransform;
    private Transform playerTransform;
    private float attackCooldown = 1.5f;
    private float timer = 0f;

    public AttackAction(Transform enemy, Transform player, BehaviourCondition[] conditions) : base(conditions)
    {
        enemyTransform = enemy;
        playerTransform = player;
    }

    public override void Tick(float deltaTime)
    {
        timer += deltaTime;
        if (timer >= attackCooldown)
        {
            Object.Destroy(enemyTransform.gameObject);
            timer = 0f;
        }
    }
}
