using UnityEngine;

public class SpawnCooldownCondition : BehaviourCondition
{
    private float cooldownTime;
    private float lastSpawnTime;

    public SpawnCooldownCondition(float cooldown)
    {
        cooldownTime = cooldown;
        StartCooldown();
    }

    public void StartCooldown()
    {
        lastSpawnTime = Time.time;
    }

    public override bool Evaluate()
    {
        return (Time.time - lastSpawnTime) >= cooldownTime;
    }
}
