using System.Collections;
using UnityEngine;

public class ScaleUpSpawn : MonoBehaviour
{
	[SerializeField] private float spawnDuration = 1.0f;
	[SerializeField] private MonoBehaviour[] scriptsToEnable;

    void Start()
    {
		StartCoroutine(Spawn_Coroutine());
    }

	private IEnumerator Spawn_Coroutine()
	{
		float time = 0.01f;

		while (time <= 1.0f)
		{
			transform.localScale = Vector3.one * time;

			time += GameManager.ScaledDeltaTime;
			yield return null;
		}

		foreach (MonoBehaviour script in scriptsToEnable)
		{
			script.enabled = true;
		}
	}
}
