using UnityEngine;
using UnityEngine.UI;

public class HealthControllerUI : MonoBehaviour
{
    [SerializeField] private Transform healthContainer;
    [SerializeField] private GameObject healthPointPrefab;

    [SerializeField] private Color enabledColor;
    [SerializeField] private Color disabledColor;


	private Image[] healthPoints;


    public void SpawnHealth(Component sender, object data)
    {
        if (!(data is int health)) return;

        healthPoints = new Image[health];

        for (int i = 0; i < health; i++)
        {
            healthPoints[i] = Instantiate(healthPointPrefab, healthContainer).GetComponent<Image>();
        }
    }

    public void UpdateHealth(Component sender, object data)
    {
        if (!(data is float health)) return;

        for (int i = 0; i < healthPoints.Length; i++)
        {
            if (i >= health)
            {
                healthPoints[i].color = disabledColor;
			}
            else
            {
                healthPoints[i].color = enabledColor;
            }
        }
    }
}
