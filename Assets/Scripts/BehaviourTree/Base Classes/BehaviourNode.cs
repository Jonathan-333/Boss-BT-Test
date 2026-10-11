using UnityEngine;

public class BehaviourNode
{
    protected BehaviourComposite parentComposite;
    protected BehaviourCondition[] conditions;
    protected bool interupted = false;


    public BehaviourNode(BehaviourCondition[] _conditions)
    {
        conditions = _conditions;
    }

    public bool EvaluateConditions()
    {
        if (conditions == null || conditions.Length == 0) return true;

        foreach (BehaviourCondition condition in conditions)
        {
            if (!condition.Evaluate())
            {
                return false;
            }
        }

        return true;
    }

    public virtual void ExecuteAction(BehaviourComposite parentComposite)
    {
        interupted = false;
        this.parentComposite = parentComposite;
        SetNodeAsActive();
    }

    public virtual void FinishAction(bool result)
    {
        if (!interupted)
        {
            parentComposite.FinishAction(result);
        }
    }

    public virtual void Interupt()
    {
        interupted = true;
        FinishAction(false);
    }

    public virtual void Tick(float deltaTime) { }

    private void SetNodeAsActive()
    {
        if (parentComposite == null || this is BehaviourComposite) return;

        parentComposite.behaviourTree.activeNode = this;
    }
}
