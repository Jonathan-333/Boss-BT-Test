using UnityEngine;

public class BehaviourComposite : BehaviourNode
{
    public enum CompositeType
    {
        Selector,
        Sequence,

		// Execute toute la séquence (peu importe les succès et les erreures), puis retourne un succès si le nombre de succès de
		// la séquence (`sequenceSuccessCount`) atteint ou dépasse le treshold (`sequenceSuccessTreshold`).
		TresholdSequence,
    };

    CompositeType compositeType;
    BehaviourNode[] nodes;
    public BehaviourTree behaviourTree;

    int compositeIndex = 0;
    int sequenceSuccessTreshold;
    int sequenceSuccessCount = 0;

    private bool SequenceCompleted => compositeIndex == nodes.Length - 1;


	public BehaviourComposite(CompositeType _compositeType, BehaviourNode[] _nodes, BehaviourTree _behaviourTree, BehaviourCondition[] conditions)
        : this(_compositeType, _nodes, _behaviourTree, conditions, 0) { }

    public BehaviourComposite(int _sequenceTreshold, BehaviourNode[] _nodes, BehaviourTree _behaviourTree, BehaviourCondition[] conditions)
         : this(CompositeType.TresholdSequence, _nodes, _behaviourTree, conditions, _sequenceTreshold) { }

    private BehaviourComposite(CompositeType _compositeType, BehaviourNode[] _nodes, BehaviourTree _behaviourTree, BehaviourCondition[] conditions, int _sequenceTreshold)
         : base(conditions)
    {
        compositeType = _compositeType;
        nodes = _nodes;
        behaviourTree = _behaviourTree;
        sequenceSuccessTreshold = _sequenceTreshold;
    }

    public override void ExecuteAction(BehaviourComposite parentComposite)
    {
        base.ExecuteAction(parentComposite);

        switch (compositeType)
        {
            case CompositeType.Selector:
                SelectorStart(0);
                break;
            case CompositeType.Sequence:
                SequenceContinue(0);
                break;
            case CompositeType.TresholdSequence:
                TresholdSequenceContinue(0);
                break;
        }
    }

    public override void FinishAction(bool result)
    {
		switch (compositeType)
		{
			case CompositeType.Selector:
				if (!result)
                {
					FinishAction_Selector();
					return;
				}
                break;

			case CompositeType.Sequence:
                if (!SequenceCompleted)
                {
					FinishAction_Sequence(result);
					return;
				}
				EndComposite(result);
				return;

			case CompositeType.TresholdSequence:
                if (!SequenceCompleted)
                {
					FinishAction_TresholdSequence(result);
					return;
				}

				if (result)
				{
					sequenceSuccessCount++;
				}

				EndComposite(sequenceSuccessCount >= sequenceSuccessTreshold);
				return;
		}

		EndComposite(true);
	}


    private void FinishAction_Selector()
    {
        if (!SequenceCompleted)
        {
            SelectorStart(compositeIndex + 1);
            return;
        }

        // End the composite with failure, do NOT call SelectorStart again
        EndComposite(false);
    }
	private void FinishAction_Sequence(bool result)
    {
        if (!result)
        {
            EndComposite(false);
            return;
        }

        SequenceContinue(compositeIndex + 1);
    }
    private void FinishAction_TresholdSequence(bool result)
    {
		if (result)
		{
            sequenceSuccessCount++;
		}

		TresholdSequenceContinue(compositeIndex + 1);
	}

	private void EndComposite(bool result)
    {
        if (parentComposite == null)
        {
            if (result)
            {
                behaviourTree.RunTree();
            }
            return;
        }
        parentComposite.FinishAction(result);
    }

    private void SelectorStart(int startIndex)
    {
        for (int i = startIndex; i < nodes.Length; i++)
        {
            if (!nodes[i].EvaluateConditions())
            {
                continue;
            }

            compositeIndex = i;
            nodes[i].ExecuteAction(this);
            return;
        }

        // If no valid node found, finish with failure (do NOT call SelectorStart again)
        FinishAction(false);
    }

    private void SequenceContinue(int index)
    {
        compositeIndex = index;

        if (!nodes[compositeIndex].EvaluateConditions())
        {
            FinishAction(false);
            return;
        }

        nodes[compositeIndex].ExecuteAction(this);
    }

    private void TresholdSequenceContinue(int index)
    {
        compositeIndex = index;
        if (index == 0)
        {
            sequenceSuccessCount = 0;
		}

		if (!nodes[compositeIndex].EvaluateConditions())
        {
            FinishAction(false);
            return;
        }

        nodes[compositeIndex].ExecuteAction(this);
    }
}