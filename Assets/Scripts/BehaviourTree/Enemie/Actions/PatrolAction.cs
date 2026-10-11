using UnityEngine;
using UnityEngine.AI;

public class PatrolAction : BehaviourNode
{
    private Vector3[] patrolPoints;
    private int currentPoint = 0;
    private Transform enemyTransform;
    private float speed = 2f;
    private int patrolPointCount = 3;
    private float patrolAreaRadius = 15f;

    public PatrolAction(Transform enemy, BehaviourCondition[] conditions) : base(conditions)
    {
        enemyTransform = enemy;
        GeneratePatrolPoints();
    }

    private void GeneratePatrolPoints()
    {
        patrolPoints = new Vector3[patrolPointCount];
        for (int i = 0; i < patrolPointCount; i++)
        {
            patrolPoints[i] = GetRandomNavMeshPoint(enemyTransform.position, patrolAreaRadius);
        }
    }

    private Vector3 GetRandomNavMeshPoint(Vector3 center, float radius)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPos = center + Random.insideUnitSphere * radius;
            randomPos.y = center.y;
            if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }
        return center; // fallback
    }

    public override void Tick(float deltaTime)
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            // No patrol points: just idle, do not finish the action
            return;
        }
        Vector3 target = patrolPoints[currentPoint];
        enemyTransform.position = Vector3.MoveTowards(enemyTransform.position, target, speed * deltaTime);

        if (Vector3.Distance(enemyTransform.position, target) < 0.2f)
        {
            currentPoint = (currentPoint + 1) % patrolPoints.Length;
        }
    }
}
