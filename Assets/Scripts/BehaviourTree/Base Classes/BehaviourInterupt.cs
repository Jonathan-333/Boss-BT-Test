using System.Linq;
using UnityEngine;

public class BehaviourInterupt
{
    BehaviourTree behaviourTree;
    BehaviourCondition[] behaviourConditions;
    bool[] conditionStates;
    bool running = true;
    BehaviourNode[] attachedNodes;

    // Checked every 0.1 s (real time) from BehaviourTree.Update, instead of an async loop with
    // Task.Delay: no background tasks, so it works on WebGL and stops with its tree
    const float CheckInterval = 0.1f;
    float timeUntilCheck = CheckInterval;

    public BehaviourInterupt(BehaviourTree _behaviourTree, BehaviourCondition[] _behaviourConditions, BehaviourNode[] _attachedNodes = null)
    {
        behaviourTree = _behaviourTree;
        behaviourConditions = _behaviourConditions;
        conditionStates = new bool[behaviourConditions.Length];

        if (_attachedNodes != null)
        {
            attachedNodes = _attachedNodes;
        }

        behaviourTree.RegisterInterupt(this);

        UpdateState();
    }

    public void Stop()
    {
        running = false;
    }

    public void Tick(float unscaledDeltaTime)
    {
		if (!running) return;

		timeUntilCheck -= unscaledDeltaTime;
		if (timeUntilCheck > 0.0f) return;
		timeUntilCheck = CheckInterval;

		if (attachedNodes != null && attachedNodes.Length > 0 && !attachedNodes.Any(node => node == behaviourTree.activeNode))
		{
			return;
		}

		for (int i = 0; i < behaviourConditions.Length; i++)
		{
			if (!running) return;

			if (behaviourConditions[i].Evaluate() != conditionStates[i])
			{
				behaviourTree.Interupt();
				UpdateState();
			}
		}
	}

    private void UpdateState()
    {
        for (int i = 0; i < behaviourConditions.Length; i++)
        {
            conditionStates[i] = behaviourConditions[i].Evaluate();
        }
    }
}
