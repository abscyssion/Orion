using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NavScreen : MonoBehaviour
{
    private TextMeshProUGUI topText;
    private TextMeshProUGUI botText;

    private void Awake()
    {
        topText = gameObject.transform.Find("Plane/Canvas/Top Text").GetComponent<TextMeshProUGUI>();    
        botText = gameObject.transform.Find("Plane/Canvas/Bottom Text").GetComponent<TextMeshProUGUI>();    
    }

    public void SetTopText(string str)
    {
        topText.SetText(str);
    }

    public void SetBotText(string str)
    {
        botText.SetText(str);
    }
}
