using AxieRescuer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartIntro : MonoBehaviour
{
    public GameObject IntroUI;
    public Image IntroText;
    public GameObject NextUI;

    void Awake()
    {
        StartCoroutine(FadeInAndOut());
    }
    IEnumerator FadeInAndOut()
    {
        var targetAlpha = 1.0f;
        Color curColor = IntroText.color;
        while (curColor.a < targetAlpha)
        {
            curColor.a += 0.04f;
            IntroText.transform.localScale *= 1.005f;
            IntroText.color = curColor;
            yield return new WaitForSeconds(0.08f);
        }
        yield return StartCoroutine(FadeOut());
    }
    IEnumerator FadeOut()
    {
        var targetAlpha = 0f;
        Color curColor = IntroText.color;
        while (curColor.a > targetAlpha)
        {
            curColor.a -= 0.03f;
            IntroText.color = curColor;
            yield return new WaitForSeconds(0.08f);
        }
        IntroUI.SetActive(false);
        NextUI.SetActive(true);
    }
}
