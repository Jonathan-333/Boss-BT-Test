using System.Collections;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
	[Header("Speed")]
	[SerializeField] private float dashSpeed = 11.0f;
	[SerializeField] private AnimationCurve speedModulation = AnimationCurve.Constant(0.0f, 1.0f, 1.0f);

	[Space]
	[Header("Duration in Seconds")]
	[SerializeField] private float dashDuration = 1.0f;
	[SerializeField] private float dashCooldown = 4.0f;

	[Space]
	[Header("Events")]
	[SerializeField] private CustomEvent dashStartEvent;
	[SerializeField] private CustomEvent dashEndEvent;
	[SerializeField] private CustomEvent dashActiveEvent;
	[SerializeField] private CustomEvent dashInactiveEvent;


	private CharacterController controller;
	private Coroutine activeCoroutine;

	private Vector3 direction;

	private float dashCooldownTime = 0.0f;
	private bool dashActiveInvoked = false;


	private void Awake()
	{
		controller = GetComponentInParent<CharacterController>();
	}
	private void Update()
	{
		if (dashCooldownTime > 0.0f)
		{
			dashCooldownTime -= GameManager.ScaledDeltaTime / dashCooldown;
			dashCooldownTime = Mathf.Clamp01(dashCooldownTime);
		}
		else if (!dashActiveInvoked)
		{
			dashActiveEvent?.Raise(this);
			dashActiveInvoked = true;
		}
	}

	public void OnDashInput(Component sender, object data)
	{
		if (data is Vector2 input)
		{
			direction = new Vector3(input.x, 0.0f, input.y);
		}
		else if (data is Vector3 position)
		{
			direction = position - transform.position;
		}

		StartDashCoroutine();
	}

	private void StartDashCoroutine()
	{
		if (dashCooldownTime > 0.01f) return;

		dashCooldownTime = 1.0f;
		dashActiveInvoked = false;

		if (activeCoroutine != null)
		{
			StopCoroutine(activeCoroutine);
		}

		activeCoroutine = StartCoroutine(Dash_Coroutine());
	}

	private IEnumerator Dash_Coroutine()
	{
		DashStart();
		float time = 0.0f;

		while (time <= 1.0f)
		{
			DashUpdate(time);

			time += GameManager.ScaledDeltaTime / dashDuration;
			yield return null;
		}

		DashEnd();
	}

	private void DashStart()
	{
		dashInactiveEvent?.Raise(this);
		dashStartEvent?.Raise(this, transform.position);
	}
	private void DashUpdate(float time)
	{
		float speed = speedModulation.Evaluate(time) * dashSpeed;
		controller.Move(GameManager.ScaledDeltaTime * speed * direction.normalized);
	}
	private void DashEnd()
	{
		dashEndEvent?.Raise(this, transform.position);
	}

	public void ResetCooldown(Component sender, object data)
	{
		dashCooldownTime = 0.0f;
		dashActiveInvoked = false;
	}
}
