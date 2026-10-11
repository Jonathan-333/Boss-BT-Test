using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingController : MonoBehaviour
{
	Volume volume;
	ChromaticAberration chromaticAberration;

	private void Awake()
	{
		volume = GetComponent<Volume>();
		bool res = volume.profile.TryGet(out chromaticAberration);
	}

	public void UpdatePostProcessing(Component sender, object data)
    {
		if (!(data is float speed)) return;

		speed = Mathf.Clamp01(speed);
		chromaticAberration.intensity.value = (1.0f - speed) / 2.0f + 0.2f;
    }
}
