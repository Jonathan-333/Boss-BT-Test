using System.Collections;
using UnityEngine;

public class ProjectilesLauncher : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;

    [SerializeField] private float cooldown_sec = 1.0f;


	Coroutine fireCoroutine;


    [ContextMenu("Fire On")]
    public void StartFiring()
    {
        if (fireCoroutine != null)
        {
            StopFiring();
        }

        fireCoroutine = StartCoroutine(Fire_Coroutine());
    }

    [ContextMenu("Fire Off")]
	public void StopFiring()
    {
        if (fireCoroutine == null) return;

        StopCoroutine(fireCoroutine);
    }

    private IEnumerator Fire_Coroutine()
    {
        while (true)
        {
            GameObject projectileInstance = Instantiate(projectilePrefab, transform.position, Quaternion.LookRotation(transform.forward));
            yield return new WaitForSeconds(cooldown_sec);
        }
    }
}
