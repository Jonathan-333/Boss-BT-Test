
// Scripts basés sur la vidéo suivante:
//
// This Is GameDev - ULTIMATE Event System for Unity Beginners
// https://www.youtube.com/watch?v=7_dyDmF0Ktw


using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(fileName = "GameEvent", menuName = "CustomEvents/GameEvent", order = 0)]
[System.Serializable]
public class CustomEvent : ScriptableObject
{
	protected readonly List<EventListener> listeners = new List<EventListener>();

	public void Raise()
	{
		Raise(null, null);
	}
	public void Raise(Component sender)
	{
		Raise(sender, null);
	}
	public void Raise(object data)
	{
		Raise(null, data);
	}
	public void Raise(Component sender, object data)
	{
		for (int i = 0; i < listeners.Count; i++)
			listeners[i].OnEventRaised(sender, data);
	}

	public void RegisterListeners(EventListener listener)
	{
		if (!listeners.Contains(listener))
			listeners.Add(listener);
	}

	public void UnregisterListener(EventListener listener)
	{
		if (listeners.Contains(listener))
			listeners.Remove(listener);
	}
}
