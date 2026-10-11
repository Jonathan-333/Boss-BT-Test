using UnityEngine;

public class PlayerMana : MonoBehaviour
{
    [SerializeField] private int maxMana = 100;
    [SerializeField] private int manaPerKill = 10;
    [SerializeField] private int fullManaPower = 15;

    [Space]
    [SerializeField] private CustomEvent manaChangedEvent;
    [SerializeField] private CustomEvent manaFullEvent;
    [SerializeField] private CustomEvent manaUsedEvent;

	public int FullManaPower => fullManaPower;
    public int Maxmana => maxMana;
    private float currentMana = 0;

    public void OnEnemyDeath(Component sender, object data)
    {
        OnPlayerAttack();
        GainMana(manaPerKill);
    }

    private void GainMana(int amount)
    {
        float previousMana = currentMana;
        currentMana = Mathf.Min(currentMana + amount, maxMana);

        manaChangedEvent?.Raise(this, currentMana);

        if (currentMana >= maxMana && Mathf.Abs(currentMana - previousMana) >= 0.01f)
        {
            manaFullEvent?.Raise(this, currentMana);
		}
	}

    public void OnPlayerAttack()
    {
        
	}

    public void OnBossTakeDamage(Component sender, object data)
    {
		if (currentMana >= maxMana)
		{
			currentMana = 0;
			manaChangedEvent?.Raise(this, currentMana);
			manaUsedEvent?.Raise(this);
		}
	}

    // Optional: expose current mana for UI
    public float GetCurrentMana() => currentMana;
}
