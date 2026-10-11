using System.Collections;
using UnityEngine;

public class EntityController : MonoBehaviour
{
	public EntityHealth Health { get; private set; }
	public EntityBodyDamage BodyDamage { get; private set; }


	private void Awake()
	{
		Health = GetComponentInChildren<EntityHealth>();
		BodyDamage = GetComponentInChildren<EntityBodyDamage>();
    }
    private void Start()
    {
        if (CompareTag("Boss"))
            EventManager.Instance.BossHealthInit?.Raise(this, (int)Health.Health);
    }
}
