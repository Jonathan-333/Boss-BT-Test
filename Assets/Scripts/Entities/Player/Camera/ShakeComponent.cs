using System;
using System.Collections;
using UnityEngine;

public class ShakeComponent : MonoBehaviour
{
    [SerializeField] private float intensity;
    [SerializeField] private float duration;
    [SerializeField] private int shakeCount;
    [SerializeField] private bool scaledTime = false;


    private Coroutine shakeCoroutine;


	public void Shake()
    {
        StopShaking();
        shakeCoroutine = StartCoroutine(Shake_Coroutine());
    }

    public void ShakeContinuous()
    {
        StopShaking();
		shakeCoroutine = StartCoroutine(Shake_Coroutine(true));
	}

	public void StopShaking()
    {
		if (shakeCoroutine != null)
			StopCoroutine(shakeCoroutine);
	}

    private IEnumerator Shake_Coroutine(bool continuous = false)
    {
        do
        {
            float t = 0.0f;
            Vector3[] shakes = new Vector3[shakeCount + 2];

            float lastX = 0.0f;
            float lastY = 0.0f;

            for (int i = 0 + 1; i < shakeCount - 1; i++)
            {
                // Sample a random point on the other side of the axis from the previous point
                bool lastXPositive = (int)Mathf.Sign(lastX) >= 0.0f;
                bool lastYPositive = (int)Mathf.Sign(lastY) >= 0.0f;

                float x = UnityEngine.Random.Range(-intensity + (lastXPositive ? 0.0f : intensity / 2), intensity - (lastXPositive ? intensity / 2 : 0.0f));
                float y = UnityEngine.Random.Range(-intensity + (lastYPositive ? 0.0f : intensity / 2), intensity - (lastYPositive ? intensity / 2 : 0.0f));

                shakes[i] = new Vector3(x, y, 0.0f);
            }

            // First & Last positions are the beginning position
            shakes[0] = shakes[shakeCount + 1] = transform.localPosition;

            float shakeTime = 1.0f / shakeCount;
            int shakeIndex = 0;

            while (t <= 1.0f)
            {
                shakeIndex = (int)Mathf.Floor(t / shakeTime);
                float localLerp = (t - shakeIndex * shakeTime) * shakeCount;

                transform.localPosition = Vector3.Lerp(shakes[shakeIndex], shakes[shakeIndex + 1], localLerp);

                t += (scaledTime ? GameManager.ScaledDeltaTime : Time.deltaTime) / duration;
                yield return null;
            }

            transform.localPosition = shakes[shakeCount + 1];
        }
        while (continuous);
    }
}
