using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }


	[SerializeField] private CustomEvent timeScaleChangedEvent;


	[field: SerializeField] public LayerMask MouseClickLayerMask { get; private set; }
	[field: SerializeField] public LayerMask MouseClickSnapLayerMask { get; private set; }


	public EntityController Player { get; private set; }


	private float _timeScale = 1.0f;
	public float TimeScale => _timeScale;
	public static float ScaledDeltaTime => Time.deltaTime * Instance.TimeScale;


	private Dictionary<Guid, float> timeScales = new Dictionary<Guid, float>();

	private void UpdateTimeScale()
	{
		_timeScale = timeScales.Select(timeScale => timeScale.Value).Aggregate((a, b) => a * b);
		timeScaleChangedEvent?.Raise(this, _timeScale);
	}


	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
			SceneManager.sceneLoaded += OnSceneLoaded;
		}
		else
		{
			Destroy(gameObject);
			return;
		}

		AddTimeScaler(1.0f);

		Player = GameObject.FindWithTag("Player").GetComponent<EntityController>();
	}

	private void OnDestroy()
	{
		if (Instance == this)
			SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		timeScales.Clear();
		AddTimeScaler(1.0f);

		GameObject player = GameObject.FindWithTag("Player");
		Player = player != null ? player.GetComponent<EntityController>() : null;
	}

	public Guid AddTimeScaler(float timeScale)
	{
		Guid guid = Guid.NewGuid();

		timeScales.Add(guid, Mathf.Max(0.0f, timeScale));
		UpdateTimeScale();

		return guid;
	}
	public void SetTimeScaler(float timeScale, Guid guid)
	{
		timeScales[guid] = timeScale;
		UpdateTimeScale();
	}
	public void RemoveTimeScaler(Guid guid)
	{
		if (timeScales.ContainsKey(guid))
		{
			timeScales.Remove(guid);
			UpdateTimeScale();
		}
	}
}
