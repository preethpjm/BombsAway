using System.Collections;
using TMPro;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject tombPrefab;
    public GameObject gameOverMenu;
    public GameObject[] rockets;
    public GameObject[] explosionEffects;
    public GameObject fireEffectPrefab;
    private float[] rocketSpeed = { 20f, 15f, 10f };
    public float arcHeight = 5f;
    public MousePointer mousePointer;
    private int rocketCount;
    private float blastRadius = 1f;
    private int index =0;
    private int killCounter;
    private Camera mainCamera;

    private void Awake()
    {
        gameOverMenu.SetActive(false);
        mainCamera = Camera.main;
    }

    public void LaunchRocket(int rocketIndex)
    {
        index = rocketIndex;
        rocketCount = rocketIndex;
        blastRadius = mousePointer.SetRocketIndex(rocketIndex);
        Debug.Log(blastRadius); 
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("index is = " + index);
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    Vector3 targetPosition = hit.point;
                    Vector3 startPosition = mainCamera.transform.position + Vector3.up * 10f;
                    GameObject rocket = Instantiate(rockets[rocketCount], startPosition, Quaternion.identity);

                    StartCoroutine(MoveRocketInArc(rocket, targetPosition));
                }
            }
        }
    }
    private IEnumerator MoveRocketInArc(GameObject rocket, Vector3 targetPosition)
    {
        Vector3 startPosition = mainCamera.transform.position + Vector3.up * 2f;
        rocket.transform.position = startPosition;

        Rigidbody rocketRigidbody = rocket.GetComponent<Rigidbody>();
        if (rocketRigidbody == null)
        {
            rocketRigidbody = rocket.AddComponent<Rigidbody>();
        }

        rocketRigidbody.useGravity = false;
        rocketRigidbody.isKinematic = true;

        float journeyDuration = Vector3.Distance(startPosition, targetPosition) / rocketSpeed[index];
        float elapsedTime = 0f;

        float sideArcDirection = Random.value > 0.5f ? 1f : -1f;
        float sideArcMagnitude = Random.Range(3f, 5f);

        while (elapsedTime < journeyDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / journeyDuration;
            t = Mathf.Clamp01(t);
            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, t);
            currentPosition.x += Mathf.Sin(t * Mathf.PI) * sideArcDirection * sideArcMagnitude;
            rocket.transform.position = currentPosition;
            Vector3 directionToTarget = (targetPosition - rocket.transform.position).normalized;

            if (directionToTarget.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                targetRotation *= Quaternion.Euler(90f, 0f, 0f);
                rocket.transform.rotation = targetRotation;
            }

            yield return null;
        }
        if (rocketCount >= 0 && rocketCount < explosionEffects.Length)
        {
            GameObject explosion = Instantiate(explosionEffects[rocketCount], targetPosition, Quaternion.identity);
            float scaleFactor = 1f;
            if (blastRadius == 4f)
            {
                scaleFactor = 2f;
                TriggerFireEffect(targetPosition);
            }
            else if (blastRadius == 2f)
            {
                scaleFactor = 1f;
            }
            else
            {
                scaleFactor = .75f;
            }
            explosion.transform.localScale *= scaleFactor;
        }
        DestroyNearbyObjects(targetPosition);
        Destroy(rocket);
    }
    private void DestroyNearbyObjects(Vector3 explosionCenter)
    {
        if (blastRadius == 4f)
        {
            AudioManager.Instance.PlayExplosion3();
        }
        else if (blastRadius == 2f)
        {
            AudioManager.Instance.PlayExplosion2();
        }
        else
        {
            AudioManager.Instance.PlayExplosion1();
        }
        
        Collider[] hitColliders = Physics.OverlapSphere(explosionCenter, blastRadius);
        int playerLayer = LayerMask.NameToLayer("Player");

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.layer == playerLayer)
            {
                Vector3 tombPosition = hitCollider.transform.position;
                Quaternion tombRotation = hitCollider.transform.rotation;
                Instantiate(tombPrefab, tombPosition, tombRotation);
                Destroy(hitCollider.gameObject);

                killCounter++;
            }
        }

        if (killCounter >= 3)
        {
            Debug.Log("Won the game");
            gameOverMenu.SetActive(true);
        }
    }

    private void TriggerFireEffect(Vector3 explosionCenter)
    {
        int fireInstances = Random.Range(2, 5);
        for (int i = 0; i < fireInstances; i++)
        {
            Vector3 randomPoint = explosionCenter + Random.insideUnitSphere * blastRadius;
            randomPoint.y = explosionCenter.y;

            GameObject fire = Instantiate(fireEffectPrefab, randomPoint, Quaternion.identity);
            float scaleFactor = blastRadius > 2f ? .75f : .5f;
            fire.transform.localScale *= scaleFactor;
            Destroy(fire, 5f);
        }
    }

}
