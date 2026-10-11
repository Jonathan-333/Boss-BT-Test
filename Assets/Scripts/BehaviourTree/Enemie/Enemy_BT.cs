using Unity.VisualScripting;
using UnityEngine;

public class Enemy_BT : BehaviourTree
{
    [SerializeField] private float chaseRange = 8f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float maxChaseTime = 20f;
    [SerializeField] private float maxChaseDistance = 15f;

    Transform player;
    ChaseAction chaseAction;
    PatrolAction patrolAction;
    RestAction restAction;

    BehaviourComposite sequencer1;

    public override void InitializeTree()
    {
        player = GameObject.FindWithTag("Player").transform;

        // Actions
        chaseAction = new ChaseAction(transform, player, maxChaseTime, new BehaviourCondition[] { new PlayerInChaseRange(transform, player, chaseRange)});
        restAction = new RestAction(transform, null);
        patrolAction = new PatrolAction(transform, null);

		// Interupts
		new BehaviourInterupt(
			_behaviourTree: this,
			_behaviourConditions: new BehaviourCondition[] {
				new PlayerInChaseRange(transform, player, chaseRange)
			}, 
            _attachedNodes: new BehaviourNode[] { patrolAction, chaseAction }
			);

		// Sequence: Chase -> Rest (rest only runs if hasRunTooLong is true)
		sequencer1 = new BehaviourComposite(
            _compositeType: BehaviourComposite.CompositeType.Sequence,
            _nodes: new BehaviourNode[] { 
                chaseAction, 
                restAction 
            },
            _behaviourTree: this,
            null 
        );

        // Root selector: (Chase+Rest) or Patrol (idle)
        root = new BehaviourComposite(
            _compositeType: BehaviourComposite.CompositeType.Selector,
            _nodes: new BehaviourNode[] { 
                sequencer1, 
                patrolAction 
            },
            _behaviourTree: this,
            null
        );
    }
}