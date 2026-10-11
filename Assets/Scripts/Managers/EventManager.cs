using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

	[field: Header("EVENTS")]

	[field: Space(10)]

	[field: Header("Entities")]
	[field: SerializeField] public CustomEvent EntityDeath { get; private set; }

	[field: Header("Player")]
	[field: SerializeField] public CustomEvent PlayerTakeDamage { get; private set; }
	[field: SerializeField] public CustomEvent PlayerHealthChanged { get; private set; }
	[field: SerializeField] public CustomEvent PlayerDeath { get; private set; }

	[field: Header("Enemies")]
	[field: SerializeField] public CustomEvent EnemyTakeDamage { get; private set; }
	[field: SerializeField] public CustomEvent EnemyDeath { get; private set; }

    [field: Header("Boss")]
    [field: SerializeField] public CustomEvent BossTakeDamage { get; private set; }
    [field: SerializeField] public CustomEvent BossHealthInit { get; private set; }


    private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}
}
