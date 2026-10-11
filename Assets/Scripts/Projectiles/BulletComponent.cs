using UnityEngine;

public class BulletComponent : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 10.0f;
    [SerializeField] private float lifeTime = 5.0f;


	private void Start()
	{
		Invoke(nameof(DestroySelf), lifeTime);
	}

	private void Update()
    {
        transform.position += GameManager.ScaledDeltaTime * bulletSpeed * transform.forward;
    }

	private void OnTriggerEnter(Collider other)
	{
        if (!other.CompareTag("Player")) return;
	}

	private void DestroySelf()
	{
		Destroy(gameObject);
	}
}
