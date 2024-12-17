using UnityEngine;

public class MousePointer: MonoBehaviour
    {
        public GameObject spherePrefab;
        public LayerMask groundLayer;
        private GameObject currentSphere;
        private float[] sphereSizes = { 1f, 2f, 4f };
        private int currentRocketIndex = 0;

        void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
            {
                if (currentSphere == null)
                {
                    currentSphere = Instantiate(spherePrefab);
                }

                Vector3 spherePosition = hit.point - hit.normal * (sphereSizes[currentRocketIndex] / 2)+new Vector3(0, sphereSizes[currentRocketIndex] / 2, 0);
                currentSphere.transform.position = spherePosition;
                float sphereRadius = sphereSizes[currentRocketIndex];
                currentSphere.transform.localScale = new Vector3(sphereRadius * 2, sphereRadius * 2, sphereRadius * 2);
            }
            else
            {
                if (currentSphere != null)
                {
                    Destroy(currentSphere);
                    currentSphere = null;
                }
            }
        }
        public float SetRocketIndex(int index)
        {
            if (index >= 0 && index < sphereSizes.Length)
            {
                currentRocketIndex = index;
                return sphereSizes[index];

            }
        return 0;
        }
    }

//currentSphere.transform.position = spherePosition - (hit.normal * (sphereRadius / 2))+ new Vector3(0,1,0);