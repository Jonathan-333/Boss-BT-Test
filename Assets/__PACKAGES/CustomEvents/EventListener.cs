using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class CustomUnityEvent : UnityEvent<Component, object> { }

public class EventListener : MonoBehaviour
{
	[field: SerializeField] public CustomEvent GameEvent { get; set; }

	[field: Space]
	[field: SerializeField] public CustomUnityEvent Response { get; set; }

	private void OnEnable()
	{
		GameEvent.RegisterListeners(this);
	}
	private void OnDisable()
	{
		GameEvent.UnregisterListener(this);
	}

	public void OnEventRaised(Component sender, object data)
	{
		Response.Invoke(sender, data);
	}
}
