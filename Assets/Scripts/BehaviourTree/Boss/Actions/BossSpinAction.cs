using UnityEngine;

public class BossSpinAction : BehaviourNode
{
    private Transform bossTransform;
    private Vector3 centerPosition;
    private float spinDuration = 10f;
    private float timer = 0f;
    private float moveSpeed = 6f;
    private float spinSpeed = 360f; // degrees per second
    private bool atCenter = false;


    public BossSpinAction(Transform boss, Vector3 center, float _spinDuration, BehaviourCondition[] conditions) : base(conditions)
    {
        bossTransform = boss;
        Vector3 targetPosition = center;
        spinDuration = _spinDuration;
        targetPosition.y = boss.position.y;
        centerPosition = targetPosition;
    }

    public override void ExecuteAction(BehaviourComposite parentComposite)
    {
        timer = 0f;
        atCenter = false;
        base.ExecuteAction(parentComposite);
    }

    public override void Tick(float deltaTime)
    {
        if (interupted)
            return;
        if (!atCenter)
        {
            bossTransform.position = Vector3.MoveTowards(bossTransform.position, centerPosition, moveSpeed * deltaTime);
            if (Vector3.Distance(bossTransform.position, centerPosition) < 0.1f)
            {
                atCenter = true;
            }
            return;
        }

        timer += deltaTime;
        bossTransform.Rotate(Vector3.up, spinSpeed * deltaTime);

        if (timer >= spinDuration)
        {
            FinishAction(true);
        }
    }
}