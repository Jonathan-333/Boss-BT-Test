using Unity.VisualScripting;
using UnityEngine;

public class Boss_BT : BehaviourTree
{
    [SerializeField] private float chaseRange = 12f;
    [SerializeField] private float patrolAreaRadius = 20f;
    [SerializeField] private float maxChaseTime = 7;
    [SerializeField] private GameObject enemiPrefab;
    [SerializeField] private int numberOfEnemiesToSpawn = 4;
    [SerializeField] private float spawnRadius = 5f;
    [SerializeField] private float spinDuration = 5.0f;
    [SerializeField] private float spawnCooldown = 15.0f;

    Transform player;
    ProjectilesLauncher[] projectilesLaunchers;


    // BehaviourTree
    ChaseAction chaseAction;
    PatrolAction patrolAction;
    BossSpinAction spinAction;
    SpawnEnemiesAction spawnEnemiesAction;
    SpawnEnemiesAction idleSpawnEnemiesAction;
    StartFiringBulletAction startFiringBulletAction;
    StopFiringBulletAction stopFiringBulletAction;

    BehaviourComposite sequencer;
	// END BehaviourTree

	private void Awake()
	{
		projectilesLaunchers = GetComponentsInChildren<ProjectilesLauncher>();
	}

	public override void InitializeTree()
    {
        player = GameObject.FindWithTag("Player").transform;
        Vector3 center = Vector3.zero; // Center of the map

        // Actions
        chaseAction = new ChaseAction(
            transform,
            player,
            maxChaseTime,
            new BehaviourCondition[] { new PlayerInChaseRange(transform, player, chaseRange) ,}
        );
        spinAction = new BossSpinAction(
            transform,
            center,
            spinDuration,
            new BehaviourCondition[] { new BossHasChasedTooLong(chaseAction, maxChaseTime) }
        );
        patrolAction = new PatrolAction(transform, null);
		
        SpawnCooldownCondition spawnCooldownCondition = new SpawnCooldownCondition(spawnCooldown);

        spawnEnemiesAction = new SpawnEnemiesAction(transform, enemiPrefab, numberOfEnemiesToSpawn, spawnRadius, null, null);
        idleSpawnEnemiesAction = new SpawnEnemiesAction(
            transform,
            enemiPrefab,
            numberOfEnemiesToSpawn,
            spawnRadius,
            spawnCooldownCondition,
            new BehaviourCondition[] { spawnCooldownCondition }
        );

        new BehaviourInterupt(
            _behaviourTree: this,
            _behaviourConditions: new BehaviourCondition[] { spawnCooldownCondition },
            _attachedNodes: new BehaviourNode[] { patrolAction, chaseAction }
            );

        new BehaviourInterupt(
            _behaviourTree: this,
            _behaviourConditions: new BehaviourCondition[] {
                new PlayerInChaseRange(transform, player, chaseRange),
            },
            _attachedNodes: new BehaviourNode[] { patrolAction, chaseAction }
            );

        startFiringBulletAction = new StartFiringBulletAction(
                _projectilesLaunchers: projectilesLaunchers,
                _conditions: null
            );

		stopFiringBulletAction = new StopFiringBulletAction(
				_projectilesLaunchers: projectilesLaunchers,
				_conditions: null
			);

		// Sequence: Chase -> Spin -> Patrol
		sequencer = new BehaviourComposite(
            _compositeType: BehaviourComposite.CompositeType.Sequence,
            _nodes: new BehaviourNode[] {
                chaseAction,
                spawnEnemiesAction,
				startFiringBulletAction,
                spinAction,
				stopFiringBulletAction
			},
            _behaviourTree: this,
            null
        );

        root = new BehaviourComposite(
            _compositeType: BehaviourComposite.CompositeType.Selector,
            _nodes: new BehaviourNode[] {
                idleSpawnEnemiesAction,
                sequencer,
                patrolAction
            },
            _behaviourTree: this,
            null
        );
    }
};