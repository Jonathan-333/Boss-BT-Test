public class BossHasChasedTooLong : BehaviourCondition
{
    private ChaseAction chaseAction;
    private float maxChaseTime;

    public BossHasChasedTooLong(ChaseAction chase, float maxTime)
    {
        chaseAction = chase;
        maxChaseTime = maxTime;
    }

    public override bool Evaluate()
    {
        return chaseAction.ChaseTime >= maxChaseTime;
    }
}