using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManaControllerUI : MonoBehaviour
{
	[Header("Mana")]
	[SerializeField] private Slider manaSlider;
	[SerializeField] private Slider manaSliderAnimation;
	[SerializeField] private float manaAnimationSpeed = 1.0f;
	[SerializeField] private float maxMana;

	[Space]
	[SerializeField] private Image sliderImage;
	[SerializeField] private Color defaultColor;
	[SerializeField] private Color fullColor;


	private void Update()
	{
		float difference = manaSlider.value - manaSliderAnimation.value;

		if (difference > 0.0f)
		{
			manaSliderAnimation.value += difference * (manaAnimationSpeed * GameManager.ScaledDeltaTime);
		}
	}

	public void ResetMana(Component sender, object data)
	{
		manaSlider.value = 0.0f;
		manaSliderAnimation.value = 0.0f;
	}

	public void SetMana(Component sender, object data)
	{
		if (!(data is float mana)) return;

		manaSlider.value = mana / maxMana;

		if (manaSlider.value >= 1.0f)
		{
			sliderImage.color = fullColor;
		}
		else
		{
			sliderImage.color = defaultColor;
		}
	}
}
