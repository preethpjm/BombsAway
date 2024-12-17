using UnityEngine;
using UnityEngine.UI;

public class ButtonToggle : MonoBehaviour
{
    private void Start()
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            Transform child = gameObject.transform.GetChild(i);
            Button button = child.GetComponent<Button>();
            button.onClick.AddListener(() => OnButtonClick(child.gameObject));
            Transform tickMark = child.GetChild(0);
            tickMark.gameObject.SetActive(i == 0);
        }
    }

    private void OnButtonClick(GameObject clickedButton)
    {
        foreach (Transform child in gameObject.transform)
        {
            GameObject tickMark = child.GetChild(0).gameObject;

            if (child.gameObject == clickedButton)
            {
                tickMark.SetActive(true);
            }
            else
            {
                tickMark.SetActive(false);
            }
        }
    }
}
