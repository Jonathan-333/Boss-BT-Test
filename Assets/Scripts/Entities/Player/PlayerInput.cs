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
	}

	private void Update()
	{
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

		ResetInputs();
	}

	private void ResetInputs()
	{
		preciseInputPerformedThisFrame = false;
	}
}
