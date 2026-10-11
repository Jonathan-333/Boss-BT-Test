using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    [SerializeField] private GameObject self;
    [SerializeField] private List<GameObject> targets;

    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector3 smoothSpeed;

	private void Awake()
	{
		if (self == null)
        {
            self = gameObject;
        }
	}

	// stopped folowing disabled targets
	private Vector3? targetPosition
    {
        get
        {
            var activeTargets = targets.Where(t => t != null && t.activeInHierarchy).ToList();
            if (activeTargets.Count == 0)
                return null;

            return new Vector3(
                activeTargets.Average(t => t.transform.position.x),
                activeTargets.Average(t => t.transform.position.y),
                activeTargets.Average(t => t.transform.position.z)
            );
        }
    }

    private void LateUpdate()
    {
        if (targetPosition == null) return;

        Vector3 desiredPosition = targetPosition.Value + offset;
        Vector3 smoothPosition = new Vector3(
            Mathf.Lerp(self.transform.position.x, desiredPosition.x, smoothSpeed.x * GameManager.ScaledDeltaTime),
            Mathf.Lerp(self.transform.position.y, desiredPosition.y, smoothSpeed.y * GameManager.ScaledDeltaTime),
			Mathf.Lerp(self.transform.position.z, desiredPosition.z, smoothSpeed.z * GameManager.ScaledDeltaTime));

		self.transform.position = new Vector3(smoothPosition.x, smoothPosition.y, smoothPosition.z);
    }
}
