using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[Header("Speed")]
    [SerializeField] private float movementSpeed = 11.0f;

	[Space]
	[Header("Accelerations Times in Seconds")]
    [SerializeField] private float accelerationDuration = 1.0f;
    [SerializeField] private float decelerationDuration = 1.0f;


    private CharacterController controller;

    private Vector3 speed = Vector2.zero;
    private Vector3 targetSpeed = Vector2.zero;
	private float speedDeltaMagnitude = 1.0f;


	private bool movementEnabled = true;


	private void Awake()
	{
		controller = GetComponentInParent<CharacterController>();
	}

	public void OnMoveInput(Component sender, object data)
    {
        if (!(data is Vector2 input)) return;

		targetSpeed = new Vector3(input.x, 0.0f, input.y);
		speedDeltaMagnitude = Vector3.Magnitude(targetSpeed - speed);
	}

	private void Update()
	{
		if (!movementEnabled) return;

        float maxDelta = GameManager.ScaledDeltaTime / (targetSpeed.magnitude >= 0.01f ? accelerationDuration : decelerationDuration);
		speed = Vector3.MoveTowards(speed, targetSpeed, speedDeltaMagnitude * maxDelta);

		controller.Move(GameManager.ScaledDeltaTime * movementSpeed * speed);
	}

	public void DisableInput()
	{
		movementEnabled = false;
	}

	public void EnableInput()
	{
		movementEnabled = true;
	}
}
