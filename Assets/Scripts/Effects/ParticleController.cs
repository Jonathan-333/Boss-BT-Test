using UnityEngine;

public class ParticleController : MonoBehaviour
{
	[SerializeField] private float defaultSpeed = 4.0f;

    ParticleSystem ps;

	private void Awake()
	{
		ps = GetComponent<ParticleSystem>();

		var velocityOverLifetime = ps.velocityOverLifetime;
		velocityOverLifetime.speedModifier = defaultSpeed;
	}

	public void UpdateParticleSpeed(Component sender, object data)
    {
		if (!(data is float speed)) return;

		var velocityOverLifetime = ps.velocityOverLifetime;
		velocityOverLifetime.speedModifier = defaultSpeed * speed;
    }
}
