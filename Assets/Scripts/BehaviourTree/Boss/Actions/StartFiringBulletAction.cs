using UnityEngine;

public class StartFiringBulletAction : BehaviourNode
{
	ProjectilesLauncher[] projectileLaunchers;

	public StartFiringBulletAction(ProjectilesLauncher[] _projectilesLaunchers, BehaviourCondition[] _conditions) : base(_conditions)
	{
		projectileLaunchers = _projectilesLaunchers;
	}

	public override void ExecuteAction(BehaviourComposite parentComposite)
	{
		foreach (ProjectilesLauncher launcher in projectileLaunchers)
		{
			launcher.StartFiring();
		}

		base.ExecuteAction(parentComposite);
	}

	public override void Tick(float deltaTime)
	{
		FinishAction(true);
	}
}