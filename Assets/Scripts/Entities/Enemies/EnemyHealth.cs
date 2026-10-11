using UnityEngine;

public class EnemyHealth : EntityHealth
{
	protected override bool OnDamageTaken()
	{
		EventManager.Instance.EnemyTakeDamage?.Raise(this);
		return base.OnDamageTaken();
	}

	protected override void Die()
	{
        EventManager.Instance.EnemyDeath?.Raise(this, Health);
		Destroy(transform.parent.gameObject);
	}
}
