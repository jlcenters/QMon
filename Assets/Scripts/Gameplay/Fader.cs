using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    Image image;
    public float fadeTime;
    public static Fader instance { get; private set; }

    private void Awake()
    {
        instance = this;
        image = GetComponent<Image>();
    }

    public IEnumerator FadeIn()
    {
        yield return image.DOFade(1f, fadeTime).WaitForCompletion();
    }
    public IEnumerator FadeOut()
    {
        yield return image.DOFade(0, fadeTime).WaitForCompletion();

    }
}
