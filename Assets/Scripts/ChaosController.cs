using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChaosController : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public RawImage fill;

    public Text chaosName;
    public Text chaosAmount;
    public Text totalScoreText;

    int totalScore = 0;
    Vector3 originalPos;

    #region singleton
    private static ChaosController _instance;

    public static ChaosController Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    private void Start()
    {
        totalScore = 0;
        totalScoreText.text = totalScore.ToString();

        originalPos = chaosName.transform.localPosition;
    }

    public void SetChaosText(ChaosArea area)
    {
        AudioController.Instance.PlayScoreSound();

        totalScore += area.chaosAmount;
        StartCoroutine(BumpText(.15f, totalScoreText));
        totalScoreText.text = totalScore.ToString();

        var tempColor = gradient.Evaluate(area.chaosAmount / 100f);
        chaosName.color = tempColor;
        chaosAmount.color = tempColor;

        StartCoroutine(FadeTextToFullAlpha(.1f, chaosName));
        StartCoroutine(FadeTextToFullAlpha(.1f, chaosAmount));

        chaosName.transform.localPosition = new Vector3(originalPos.x + Random.Range(-120, 120), originalPos.y + Random.Range(-40, 40));
        chaosName.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(-10, 10));

        chaosName.text = area.chaosName;
        chaosAmount.text = "+" + area.chaosAmount.ToString();

        StartCoroutine(FadeTextToZeroAlpha(1f, chaosName));
        StartCoroutine(FadeTextToZeroAlpha(1f, chaosAmount));
    }

    public void SetChaosText(string name, int amount)
    {
        AudioController.Instance.PlayScoreSound();

        totalScore += amount;
        StartCoroutine(BumpText(.15f, totalScoreText));
        totalScoreText.text = totalScore.ToString();

        var tempColor = gradient.Evaluate(amount / 100f);
        chaosName.color = tempColor;
        chaosAmount.color = tempColor;

        StartCoroutine(FadeTextToFullAlpha(.1f, chaosName));
        StartCoroutine(FadeTextToFullAlpha(.1f, chaosAmount));

        chaosName.transform.localPosition = new Vector3(originalPos.x + Random.Range(-120, 120), originalPos.y + Random.Range(-40, 40));
        chaosName.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(-10, 10));

        chaosName.text = name;
        chaosAmount.text = "+" + amount.ToString();

        StartCoroutine(FadeTextToZeroAlpha(1f, chaosName));
        StartCoroutine(FadeTextToZeroAlpha(1f, chaosAmount));
    }

    public IEnumerator FadeTextToFullAlpha(float t, Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
    }

    public IEnumerator FadeTextToZeroAlpha(float t, Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }

    public IEnumerator BumpText(float t, Text i)
    {
        while (i.transform.localScale.x < 1.5f)
        {
            i.transform.localScale = new Vector2(i.transform.localScale.x + (Time.deltaTime / t), i.transform.localScale.y + (Time.deltaTime / t));
            yield return null;
        }

        while (i.transform.localScale.x > 1)
        {
            i.transform.localScale = new Vector2(i.transform.localScale.x - (Time.deltaTime / t), i.transform.localScale.y - (Time.deltaTime / t));
            yield return null;
        }
    }
}
