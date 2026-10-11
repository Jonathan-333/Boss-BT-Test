using UnityEngine;

public class PointerController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Animator targetScalerAnimator;
    [SerializeField] private Transform spriteContainer;

    [SerializeField] private Color dashActiveColor;
    [SerializeField] private Color dashInactiveColor;

	SpriteRenderer[] sprites;


	private void Awake()
	{
        sprites = new SpriteRenderer[spriteContainer.childCount];

        int i = 0;
		foreach (Transform child in spriteContainer)
        {
            sprites[i++] = child.GetComponent<SpriteRenderer>();
        }
	}


	public void OnTargetLost(Component sender, object data)
    {
        if (!(data is Vector3 position)) return;

		targetScalerAnimator.SetBool("isFree", true);

		animator.SetBool("idleRunning", false);
		animator.SetFloat("idleSpeed", 10.0f);
    }

    public void OnTargetFocus(Component sender, object data)
    {
        if (!(data is Vector3 position)) return;

		transform.position = position;

        targetScalerAnimator.SetBool("isFree", false);

		animator.SetFloat("idleSpeed", 1.0f);
		animator.SetBool("idleRunning", true);
	}

    public void OnTargetPositionUpdate(Component sender, object data)
    {
		if (data is Vector3 position)
		{
			transform.position = position;
		}

		if (data is GameObject gameObject)
		{
			transform.position = new Vector3(gameObject.transform.position.x, 0.0f, gameObject.transform.position.z);
		}
	}

    public void OnDashAvailable(Component sender, object data)
    {
		foreach (var sprite in sprites)
		{
            sprite.color = dashActiveColor;
		}
	}

    public void OnDashUsed(Component sender, object data)
    {
		foreach (var sprite in sprites)
		{
			sprite.color = dashInactiveColor;
		}
	}
}
