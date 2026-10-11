using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class EntityBodyDamage : MonoBehaviour
{
    [field: SerializeField] public int PowerValue { get; private set; } = 0;
    [SerializeField] private PlayerMana playerMana;

    [SerializeField] private float damage = 1.0f;
    [SerializeField] private bool hurtPlayerOnly = false;

    private Enemy_BT enemyBehaviourTree;

    private void Awake()
    {
        enemyBehaviourTree = GetComponentInParent<Enemy_BT>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!enabled) return;

        if (!other.CompareTag("Hurtbox")) return;
        if (other.gameObject == gameObject) return;
        
        if (other.gameObject.transform.parent.gameObject.CompareTag("Entity")  && !transform.parent.gameObject.CompareTag("Player")) return;

        EntityController entity = other.GetComponentInParent<EntityController>();

        if (entity?.BodyDamage != null)
        {
            // Si l'autre entitée à un power plus élevé, elle l'emporte sur l'attaque
            if (entity.BodyDamage.PowerValue > PowerValue)
            {
                return;
            }
        }

        if (entity?.Health != null)
        {
            if (entity.Health as PlayerHealth)
            {
                entity.Health.TakeDamage(damage);
            }
            else if (!hurtPlayerOnly)
            {
                entity.Health.TakeDamage(damage);
            }

            // Si il s'agit d'un ennemi, le détruire
            if (enemyBehaviourTree != null && enemyBehaviourTree)
            {
                Destroy(transform.parent.gameObject);
            }

            return;
        }

        throw new MissingComponentException($"Composant {typeof(EntityHealth)} sur l'entitée {gameObject.name}");
    }

    public void SetPowerValue(int powerValue)
    {
        if (powerValue == 0)
        {
            PowerValue = 0;
            return;
        }

        if(playerMana.GetCurrentMana() >= playerMana.Maxmana)
        {
            PowerValue = playerMana.FullManaPower;
        }
        else 
            PowerValue = powerValue;
    }


    public void ResetPowerOnEnemyKill(Component sender, object data)
    {
        if (PowerValue == playerMana.FullManaPower)
        {
            PowerValue = 0;
        }
    }
}
