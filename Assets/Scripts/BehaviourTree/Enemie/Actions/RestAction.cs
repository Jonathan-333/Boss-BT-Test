using UnityEngine;

public class RestAction : BehaviourNode
{
    private float restTime = 4f;
    private float timer = 0f;
    private Transform enemyTransform;
    Rigidbody rigidBody;

    public RestAction(Transform enemy, BehaviourCondition[] conditions) : base(conditions)
    {
        enemyTransform = enemy;
        rigidBody = enemy.GetComponent<Rigidbody>();
    }

    public override void ExecuteAction(BehaviourComposite parentComposite)
    {
        timer = 0f;
        base.ExecuteAction(parentComposite);
    }

    public override void Tick(float deltaTime)
    {
        timer += deltaTime;
        if (rigidBody != null)
            rigidBody.linearVelocity = Vector3.zero;

        if (timer >= restTime)
        {
            FinishAction(true);
        }
    }

}
