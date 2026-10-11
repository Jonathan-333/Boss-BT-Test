using System.Collections;
using UnityEditor;
using UnityEngine;

public class TimeScaleController : MonoBehaviour
{
    public bool IsTimeScaled { get; private set; } = false;


    [SerializeField] private float slowDownDuration = 0.5f;
    [SerializeField] private float slowDownScale = 0.5f;
    [SerializeField] private AnimationCurve slowDownScaleModulation = AnimationCurve.Constant(0.0f, 1.0f, 1.0f);

    Coroutine coroutine;
    GUID guid;

	private void Start()
	{
        guid = GameManager.Instance.AddTimeScaler(1.0f);
	}

	public void SlowDown()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        coroutine = StartCoroutine(SlowDown_Coroutine());
    }

    private IEnumerator SlowDown_Coroutine()
    {
        IsTimeScaled = true;
        float time = 0.0f;

        while (time <= 1.0f)
        {
            float scale = slowDownScaleModulation.Evaluate(time) * slowDownScale;
			GameManager.Instance.SetTimeScaler(scale, guid);

            time += Time.deltaTime / slowDownDuration;
            yield return null;
        }

        GameManager.Instance.SetTimeScaler(1.0f, guid);
		IsTimeScaled = false;
	}

    public void ClearSlowDown()
    {
		if (coroutine != null)
		{
			StopCoroutine(coroutine);
		}

		GameManager.Instance.SetTimeScaler(1.0f, guid);
	}
}
