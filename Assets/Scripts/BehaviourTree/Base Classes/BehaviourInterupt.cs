using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class BehaviourInterupt
{
    BehaviourTree behaviourTree;
    BehaviourCondition[] behaviourConditions;
    bool[] conditionStates;
    bool running = true;
    BehaviourNode[] attachedNodes;

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
        _ = CheckInteruptLoop();
    }

    public void Stop()
    {
        running = false;
    }

    private async Task CheckInteruptLoop()
    {
		while (running)
		{
			if (behaviourTree == null)
			{
				Stop();
				return;
			}

			if (attachedNodes != null && attachedNodes.Length > 0 && !attachedNodes.Any(node => node == behaviourTree.activeNode))
			{
				await Task.Delay(100);
				continue;
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

			await Task.Delay(100);
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
