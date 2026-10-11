using UnityEngine;

public class ChaseAction : BehaviourNode
{
    private Transform enemyTransform;
    private Transform playerTransform;
    private float speed = 4f;
    private float chaseTime = 0f;
    private float maxChaseTime;

    public float ChaseTime => chaseTime;

    public ChaseAction(Transform enemy, Transform player, float _maxChaseTime, BehaviourCondition[] conditions) : base(conditions)
    {
        maxChaseTime = _maxChaseTime;
        enemyTransform = enemy;
        playerTransform = player;
    }

    public override void Tick(float deltaTime)
    {
        chaseTime += deltaTime;

        // Garde la position Y de l'ennemi pour éviter qu'il ne vole ou s'enfonce dans le sol
        Vector3 targetPosition = playerTransform.position;
        targetPosition.y = enemyTransform.position.y;

        enemyTransform.position = Vector3.MoveTowards(
            enemyTransform.position,
            targetPosition,
            speed * deltaTime
        );

        if (chaseTime >= maxChaseTime)
        {
            FinishAction(true);
        }
    }

    public override void ExecuteAction(BehaviourComposite parentComposite)
    {
        if (!interupted)
            chaseTime = 0f;

        base.ExecuteAction(parentComposite);
    }
}
