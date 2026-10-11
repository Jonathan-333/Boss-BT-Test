using UnityEngine;

public abstract class BehaviourCondition
{
    protected bool reverseCondition;
    public abstract bool Evaluate();

    public bool CheckForReverseCondition(bool result) => reverseCondition ? !result : result;
}