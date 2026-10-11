using UnityEngine;

public class StopFiringBulletAction : BehaviourNode
{
	ProjectilesLauncher[] projectileLaunchers;

	public StopFiringBulletAction(ProjectilesLauncher[] _projectilesLaunchers, BehaviourCondition[] _conditions) : base(_conditions)
	{
		projectileLaunchers = _projectilesLaunchers;
	}

	public override void ExecuteAction(BehaviourComposite parentComposite)
	{
		foreach (ProjectilesLauncher launcher in projectileLaunchers)
		{
			launcher.StopFiring();
		}

		base.ExecuteAction(parentComposite);
	}

	public override void Tick(float deltaTime)
	{
		FinishAction(true);
	}
}
