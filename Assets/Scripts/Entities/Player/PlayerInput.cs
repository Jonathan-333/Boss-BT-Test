using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
	private enum TargetMode
	{
		None,
		Free,
		Snap,
	}


	[Header("Movement")]
	[SerializeField] private InputActionReference moveInput;
	[SerializeField] private CustomEvent moveInputEvent;

	[Header("Dash")]
	[SerializeField] private InputActionReference dashInput;
	[SerializeField] private CustomEvent dashInputEvent;
	[SerializeField] private CustomEvent targetPositionUpdated;

	[Header("Dash with precision")]
	[SerializeField] private InputActionReference dashPreciseInput;
	[SerializeField] private CustomEvent targetFocus;
	[SerializeField] private CustomEvent targetLost;
	private bool preciseInputPerformedThisFrame = false;

	[Header("Dash with precision (gamepad)")]
	[Tooltip("Distance of the aim point when no target is snapped")]
	[SerializeField] private float gamepadAimDistance = 8.0f;
	[Tooltip("Max distance to snap onto a target")]
	[SerializeField] private float gamepadSnapRange = 12.0f;
	[Tooltip("Max angle between the aim direction and a target to snap onto it")]
	[SerializeField, Range(0.0f, 90.0f)] private float gamepadSnapAngle = 30.0f;
	private const float StickDeadZone = 0.3f;

	private bool usingGamepad = false;
	private Vector3 gamepadAimDirection = Vector3.forward;


	private TargetMode targetMode = TargetMode.Free;


	private void Awake()
	{
		moveInput.action.Enable();
		dashInput.action.Enable();
		dashPreciseInput.action.Enable();
	}

	private void OnEnable()
	{
		moveInput.action.performed += OnMoveInput;
		moveInput.action.canceled += OnMoveInput;

		dashInput.action.performed += OnDashInput;
		dashPreciseInput.action.performed += OnDashPreciseInput;
	}
	private void OnDisable()
	{
		moveInput.action.performed -= OnMoveInput;
		moveInput.action.canceled -= OnMoveInput;

		dashInput.action.performed -= OnDashInput;
		dashPreciseInput.action.performed -= OnDashPreciseInput;
	}

	public void OnMoveInput(InputAction.CallbackContext context)
	{
		if (context.performed || context.canceled)
		{
			moveInputEvent?.Raise(this, context.ReadValue<Vector2>());
		}
	}
	public void OnDashInput(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			dashInputEvent?.Raise(this, moveInput.action.ReadValue<Vector2>());
		}
	}
	public void OnDashPreciseInput(InputAction.CallbackContext context)
	{
		if (!context.performed) return;

		preciseInputPerformedThisFrame = true;
		usingGamepad = context.control.device is Gamepad;
	}

	private void Update()
	{
		UpdateActiveDevice();

		if (usingGamepad)
		{
			UpdateGamepadTarget();
		}
		else
		{
			UpdateMouseTarget();
		}

		ResetInputs();
	}

	// The last device used (gamepad or mouse) decides how the precise dash is aimed
	private void UpdateActiveDevice()
	{
		Gamepad gamepad = Gamepad.current;
		Mouse mouse = Mouse.current;

		if (gamepad == null)
		{
			usingGamepad = false;
			return;
		}

		if (gamepad.rightStick.ReadValue().magnitude > StickDeadZone
			|| gamepad.leftStick.ReadValue().magnitude > StickDeadZone
			|| gamepad.rightTrigger.isPressed
			|| gamepad.buttonSouth.isPressed)
		{
			usingGamepad = true;
		}
		else if (mouse != null && (mouse.delta.ReadValue().sqrMagnitude > 0.0f || mouse.leftButton.isPressed))
		{
			usingGamepad = false;
		}
	}

	// Aim with the right stick (or the left stick if the right one is idle): snap onto the closest
	// target in that direction, otherwise aim at a point in front of the player
	private void UpdateGamepadTarget()
	{
		if (GameManager.Instance.Player == null) return;

		Vector2 aim = Gamepad.current.rightStick.ReadValue();
		if (aim.magnitude < StickDeadZone)
		{
			aim = moveInput.action.ReadValue<Vector2>();
		}
		if (aim.magnitude >= StickDeadZone)
		{
			gamepadAimDirection = new Vector3(aim.x, 0.0f, aim.y).normalized;
		}

		Vector3 playerPosition = GameManager.Instance.Player.transform.position;
		Transform snapTarget = FindGamepadSnapTarget(playerPosition);

		if (snapTarget != null)
		{
			Vector3 data = new Vector3(snapTarget.position.x, 0.0f, snapTarget.position.z);

			if (targetMode != TargetMode.Snap)
			{
				targetMode = TargetMode.Snap;
				targetFocus?.Raise(this, data);
			}

			if (preciseInputPerformedThisFrame)
			{
				dashInputEvent?.Raise(this, data);
			}

			targetPositionUpdated?.Raise(this, snapTarget.gameObject);
		}
		else
		{
			Vector3 data = new Vector3(playerPosition.x, 0.0f, playerPosition.z) + gamepadAimDirection * gamepadAimDistance;

			targetPositionUpdated?.Raise(this, data);

			if (targetMode != TargetMode.Free)
			{
				targetMode = TargetMode.Free;
				targetLost?.Raise(this, data);
			}

			if (preciseInputPerformedThisFrame)
			{
				dashInputEvent?.Raise(this, data);
			}
		}
	}

	private Transform FindGamepadSnapTarget(Vector3 playerPosition)
	{
		Transform player = GameManager.Instance.Player.transform;
		Collider[] colliders = Physics.OverlapSphere(playerPosition, gamepadSnapRange, GameManager.Instance.MouseClickSnapLayerMask);

		Transform bestTarget = null;
		float bestAngle = gamepadSnapAngle;

		foreach (Collider hitCollider in colliders)
		{
			Transform target = hitCollider.gameObject.transform;
			if (target.IsChildOf(player)) continue;

			Vector3 toTarget = target.position - playerPosition;
			toTarget.y = 0.0f;
			if (toTarget.sqrMagnitude < 0.01f) continue;

			float angle = Vector3.Angle(gamepadAimDirection, toTarget);
			if (angle <= bestAngle)
			{
				bestAngle = angle;
				bestTarget = target;
			}
		}

		return bestTarget;
	}

	private void UpdateMouseTarget()
	{
		if (Mouse.current == null) return;

		Vector2 mousePosition = Mouse.current.position.ReadValue();
		Ray ray = Camera.main.ScreenPointToRay(mousePosition);

		RaycastHit hit;
		RaycastHit[] hits = Physics.RaycastAll(ray, 200.0f, GameManager.Instance.MouseClickSnapLayerMask);

		if (hits.Length != 0)
		{
			hit = hits.OrderBy(h => (GameManager.Instance.Player.transform.position - h.transform.position).magnitude).First();
			Vector3 data = hit.collider.gameObject.transform.position;
			data = new Vector3(data.x, 0.0f, data.z);

			if (targetMode != TargetMode.Snap)
			{
				targetMode = TargetMode.Snap;
				targetFocus?.Raise(this, data);
			}

			if (preciseInputPerformedThisFrame)
			{
				dashInputEvent?.Raise(this, data);
			}

			targetPositionUpdated?.Raise(this, hit.collider.gameObject);
		}
		else if (Physics.Raycast(ray, out hit, 200.0f, GameManager.Instance.MouseClickLayerMask))
		{
			Vector3 data = new Vector3(hit.point.x, 0.0f, hit.point.z);

			targetPositionUpdated?.Raise(this, data);

			if (targetMode != TargetMode.Free)
			{
				targetMode = TargetMode.Free;
				targetLost?.Raise(this, data);
			}

			if (preciseInputPerformedThisFrame)
			{
				dashInputEvent?.Raise(this, data);
			}
		}
	}

	private void ResetInputs()
	{
		preciseInputPerformedThisFrame = false;
	}
}
