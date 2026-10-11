public class HasRunTooLong : BehaviourCondition
{
    private ChaseAction chaseAction;
    private float maxChaseTime;

    public HasRunTooLong(ChaseAction chase, float maxTime)
    {
        chaseAction = chase;
        maxChaseTime = maxTime;
    }

    public override bool Evaluate()
    {
        return chaseAction.ChaseTime >= maxChaseTime;
    }
}