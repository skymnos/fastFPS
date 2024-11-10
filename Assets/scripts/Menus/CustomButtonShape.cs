using UnityEngine;
using UnityEngine.UI;

public class CustomButtonShape : MonoBehaviour
{
    void Start()
    {
        GetComponent<Image>().alphaHitTestMinimumThreshold = 0.5f;
    }
}
