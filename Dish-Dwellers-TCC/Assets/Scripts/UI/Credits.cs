using UnityEngine;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    public RectTransform creditsContainer;
    public float rollSpeed = 2f;

    public void OnEnable(){
        creditsContainer.anchoredPosition = new Vector2(0, -1150);
    }

    public void FixedUpdate(){
        creditsContainer.anchoredPosition += new Vector2(0, rollSpeed * Time.deltaTime);
    }

}
