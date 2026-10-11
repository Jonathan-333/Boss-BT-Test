using UnityEngine;

public class ChaseCooldownCondition : BehaviourCondition
{
    private float cooldownTime;
    private float lastRestEndTime = -Mathf.Infinity;

    public ChaseCooldownCondition(float cooldown)
    {
        cooldownTime = cooldown;
    }

    public void StartCooldown()
    {
        lastRestEndTime = Time.time;
    }

    public override bool Evaluate()
    {
        return (Time.time - lastRestEndTime) >= cooldownTime;
    }
}