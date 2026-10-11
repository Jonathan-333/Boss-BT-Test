using UnityEngine;

public class PlayerHealth : EntityHealth
{
	[SerializeField] private CustomEvent playerHealthInitEvent;

	[field: SerializeField] public bool IsInvincible { get; protected set; }

	// --- Player invincibility fields ---
	private float invincibleTimer = 0f;
	private readonly float invincibleDuration = 1f;
	private Color? originalColor = null;
	private MeshRenderer playerMeshRenderer;
	private BoxCollider playerBoxCollider;


	private void Awake()
	{
		playerMeshRenderer = transform.parent.GetComponentInChildren<MeshRenderer>();
		playerBoxCollider = GetComponent<BoxCollider>();
	}
	private void Start()
	{
		playerHealthInitEvent?.Raise(this, (int)Health);
	}
	private void Update()
	{
		// Only process invincibility for Player
		if (IsInvincible && transform.parent != null && transform.parent.CompareTag("Player"))
		{
			invincibleTimer -= GameManager.ScaledDeltaTime;
			if (invincibleTimer <= 0f)
			{
				playerBoxCollider.enabled = true;
				IsInvincible = false;
				invincibleTimer = 0f;
				if (playerMeshRenderer != null && originalColor.HasValue)
				{
					playerMeshRenderer.material.color = originalColor.Value;
				}
			}
		}
	}

	public override bool TakeDamage(float damage)
	{
		// If Player and invincible, ignore damage
		if (transform.parent != null && transform.parent.CompareTag("Player"))
		{
			if (IsInvincible)
				return false;

			// Set invincibility
			playerBoxCollider.enabled = false;
			IsInvincible = true;
			invincibleTimer = invincibleDuration;

			if (playerMeshRenderer != null)
			{
				if (!originalColor.HasValue)
					originalColor = playerMeshRenderer.material.color;
				playerMeshRenderer.material.color = Color.red;
			}
		}

		Health -= damage;
		return OnDamageTaken();
	}

	protected override bool OnDamageTaken()
	{
		EventManager.Instance.PlayerTakeDamage?.Raise(this, Health);
		EventManager.Instance.PlayerHealthChanged?.Raise(this, Health);
		return base.OnDamageTaken();
	}

	protected override void Die()
	{
		EventManager.Instance.PlayerDeath?.Raise(this);
		Destroy(transform.parent.gameObject);
	}

	public void GainHealth(float amount)
	{
		Health += amount;
		Health = Mathf.Clamp(Health, amount, 5.0f);

		EventManager.Instance.PlayerHealthChanged?.Raise(this, Health);
	}
}