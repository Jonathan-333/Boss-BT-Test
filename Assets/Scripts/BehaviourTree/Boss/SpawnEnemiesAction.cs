using UnityEngine;

public class SpawnEnemiesAction : BehaviourNode
{
    private Transform bossTransform;
    private GameObject enemyPrefab;
    private int numberOfEnemies;
    private float spawnRadius;
    private SpawnCooldownCondition spawnCooldown;

    public SpawnEnemiesAction(Transform boss, GameObject enemyPrefab, int numEnemies, float radius, SpawnCooldownCondition cooldown, BehaviourCondition[] conditions) : base(conditions)
    {
        spawnCooldown = cooldown;
        bossTransform = boss;
        this.enemyPrefab = enemyPrefab;
        numberOfEnemies = numEnemies;
        spawnRadius = radius;
    }
    public override void ExecuteAction(BehaviourComposite parentComposite)
    {
        base.ExecuteAction(parentComposite);
        SpawnEnemies();
        spawnCooldown?.StartCooldown();
        FinishAction(true);
    }
    private void SpawnEnemies()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            Vector3 spawnPosition = bossTransform.position + Random.insideUnitSphere * spawnRadius;
            spawnPosition.y = bossTransform.position.y; // Keep the same height as the boss

            const float DISTANCE_MIN_FROM_PLAYER = 8.0f;
            if (Vector3.Distance(spawnPosition, GameManager.Instance.Player.transform.position) <= DISTANCE_MIN_FROM_PLAYER)
            {
                Vector2 offset = DISTANCE_MIN_FROM_PLAYER * Random.insideUnitCircle.normalized;
				spawnPosition += new Vector3(offset.x, 0.0f, offset.y);
            }

            Object.Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
    }
}