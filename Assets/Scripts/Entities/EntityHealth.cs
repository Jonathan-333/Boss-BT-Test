using System.Collections;
using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    [field: SerializeField] public float Health { get; protected set; }

    [SerializeField] private float damageFlashDuration = 0.15f;

    private MeshRenderer bossMeshRenderer;
    private Color bossOriginalColor;
    private Coroutine flashCoroutine;


    public virtual bool TakeDamage(float damage)
    {
        Health -= damage;
        if (gameObject.transform.parent.CompareTag("Boss"))
        {
            EventManager.Instance.BossTakeDamage?.Raise(this, Health);
            FlashBossRed();
        }
        return OnDamageTaken();
	}

    private void FlashBossRed()
    {
        if (bossMeshRenderer == null)
        {
            bossMeshRenderer = transform.parent.GetComponent<MeshRenderer>();
            if (bossMeshRenderer == null) return;
            bossOriginalColor = bossMeshRenderer.material.color;
        }

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(Flash_Coroutine());
    }

    private IEnumerator Flash_Coroutine()
    {
        bossMeshRenderer.material.color = Color.red;
        yield return new WaitForSeconds(damageFlashDuration);
        bossMeshRenderer.material.color = bossOriginalColor;
        flashCoroutine = null;
    }

    protected virtual bool OnDamageTaken()
    {
        bool hasDied = false;

		if (Health <= 0)
        {
            Health = 0;

            hasDied = true;
            Die();
        }

        return hasDied;
    }

    protected virtual void Die()
    {
        EventManager.Instance.EntityDeath?.Raise(this);
        Destroy(gameObject);
	}
}
