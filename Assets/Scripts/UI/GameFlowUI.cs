using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameFlowUI : MonoBehaviour
{
	private enum State
	{
		Start,
		Playing,
		Victory,
		Defeat,
	}

	private static bool hasStartedOnce = false;


	private State state;

	private EntityController boss;
	private EntityController player;
	private PlayerInput playerInput;

	private GameObject startPanel;
	private GameObject victoryPanel;
	private GameObject defeatPanel;


	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Bootstrap()
	{
		hasStartedOnce = false;
		SceneManager.sceneLoaded -= OnSceneLoaded;
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (GameObject.FindWithTag("Boss") == null) return;
		if (FindAnyObjectByType<GameFlowUI>() != null) return;

		new GameObject("GameFlowUI").AddComponent<GameFlowUI>();
	}


	private void Start()
	{
		boss = GameObject.FindWithTag("Boss").GetComponent<EntityController>();

		GameObject playerObject = GameObject.FindWithTag("Player");
		player = playerObject.GetComponent<EntityController>();
		playerInput = playerObject.GetComponentInChildren<PlayerInput>();

		EnsureEventSystem();
		BuildUI();

		SetState(hasStartedOnce ? State.Playing : State.Start);
	}

	private void Update()
	{
		if (state != State.Playing) return;

		if (player == null)
		{
			SetState(State.Defeat);
		}
		else if (boss == null)
		{
			SetState(State.Victory);
		}
	}

	private void SetState(State newState)
	{
		state = newState;

		startPanel.SetActive(state == State.Start);
		victoryPanel.SetActive(state == State.Victory);
		defeatPanel.SetActive(state == State.Defeat);

		bool playing = state == State.Playing;
		Time.timeScale = playing ? 1.0f : 0.0f;

		if (playerInput != null)
			playerInput.enabled = playing;
	}

	private void OnStartClicked()
	{
		hasStartedOnce = true;
		SetState(State.Playing);
	}

	private void OnRestartClicked()
	{
		Time.timeScale = 1.0f;

		Scene scene = SceneManager.GetActiveScene();
		if (scene.buildIndex >= 0)
		{
			SceneManager.LoadScene(scene.buildIndex);
			return;
		}

#if UNITY_EDITOR
		UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(
			scene.path, new LoadSceneParameters(LoadSceneMode.Single));
#endif
	}

	private void OnDestroy()
	{
		Time.timeScale = 1.0f;
	}


	private void EnsureEventSystem()
	{
		if (FindAnyObjectByType<EventSystem>() != null) return;

		GameObject eventSystem = new GameObject("EventSystem");
		eventSystem.AddComponent<EventSystem>();
		eventSystem.AddComponent<InputSystemUIInputModule>();
	}

	private void BuildUI()
	{
		Canvas canvas = gameObject.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		canvas.sortingOrder = 100;

		CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
		scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		scaler.referenceResolution = new Vector2(1920, 1080);
		scaler.matchWidthOrHeight = 0.5f;

		gameObject.AddComponent<GraphicRaycaster>();

		startPanel = CreatePanel("StartPanel",
			"BOSS FIGHT",
			"WASD : se deplacer\nEspace : dash\nClic gauche : dash vers la cible\n\nVainquez le boss !",
			"Jouer", OnStartClicked);

		victoryPanel = CreatePanel("VictoryPanel",
			"VICTOIRE !",
			"Le boss est vaincu.",
			"Rejouer", OnRestartClicked);

		defeatPanel = CreatePanel("DefeatPanel",
			"DEFAITE",
			"Vous avez ete vaincu.",
			"Reessayer", OnRestartClicked);
	}

	private GameObject CreatePanel(string name, string title, string subtitle, string buttonLabel, UnityEngine.Events.UnityAction onClick)
	{
		GameObject panel = new GameObject(name, typeof(RectTransform), typeof(Image));
		panel.transform.SetParent(transform, false);
		Stretch(panel.GetComponent<RectTransform>());
		panel.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 0.75f);

		CreateText(panel.transform, title, 120, FontStyles.Bold, new Vector2(0, 220));
		CreateText(panel.transform, subtitle, 40, FontStyles.Normal, new Vector2(0, 20));

		GameObject button = new GameObject("Button", typeof(RectTransform), typeof(Image), typeof(Button));
		button.transform.SetParent(panel.transform, false);
		RectTransform buttonRect = button.GetComponent<RectTransform>();
		buttonRect.sizeDelta = new Vector2(360, 100);
		buttonRect.anchoredPosition = new Vector2(0, -220);
		button.GetComponent<Image>().color = new Color(0.68f, 0.36f, 0.9f, 1.0f);
		button.GetComponent<Button>().onClick.AddListener(onClick);

		TextMeshProUGUI label = CreateText(button.transform, buttonLabel, 48, FontStyles.Bold, Vector2.zero);
		Stretch(label.rectTransform);

		return panel;
	}

	private TextMeshProUGUI CreateText(Transform parent, string content, float size, FontStyles style, Vector2 position)
	{
		GameObject textObject = new GameObject("Text", typeof(RectTransform));
		textObject.transform.SetParent(parent, false);

		TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
		text.text = content;
		text.fontSize = size;
		text.fontStyle = style;
		text.alignment = TextAlignmentOptions.Center;
		text.color = Color.white;
		text.raycastTarget = false;

		RectTransform rect = text.rectTransform;
		rect.sizeDelta = new Vector2(1600, 300);
		rect.anchoredPosition = position;

		return text;
	}

	private static void Stretch(RectTransform rect)
	{
		rect.anchorMin = Vector2.zero;
		rect.anchorMax = Vector2.one;
		rect.offsetMin = Vector2.zero;
		rect.offsetMax = Vector2.zero;
	}
}
