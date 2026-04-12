using System.Collections;
using TMPro;
using UnityEngine;

public class TMP_TypewriterEffect : MonoBehaviour
{
    private TextMeshProUGUI textMeshPro;
    private string lastText;
    private string targetText;
    private bool checkingEnabled = false;

    public bool isTypewriting = false;

    [Header("Typewriter Config")]
    public float delayPerChar;

    private Coroutine typewritingRoutine;

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        checkingEnabled = true;

        lastText = textMeshPro.text;

        StartCoroutine(ETextChangeChecker());
    }

    private IEnumerator ETextChangeChecker()
    {
        while(true)
        {
            if (checkingEnabled == false)
            {
                yield return null;
                continue;
            }

            if(string.IsNullOrEmpty(textMeshPro.text))
            {
                lastText = textMeshPro.text;
            }

            if(textMeshPro.text != lastText)
            {
                SetText(textMeshPro.text);
                textMeshPro.text = "";
            }

            yield return null;
        }
    }


    [ContextMenu("Test Text")]
    public void TestText()
    {

        SetText("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer id ipsum feugiat, vehicula justo ac, suscipit ipsum. Nunc ligula tellus, ullamcorper vitae ultricies in, bibendum et lacus. Nam id ultrices turpis, non consectetur nunc. ");
    }

    private void SetText(string text)
    {

        targetText = text;

        if (typewritingRoutine != null)
        {
            StopCoroutine(typewritingRoutine);
            typewritingRoutine = null;
        }

        typewritingRoutine = StartCoroutine(ETypewriting(text));
    }

    public void EndTypewriting()
    {
        StopCoroutine(typewritingRoutine);
        
        lastText = targetText;
        textMeshPro.text = targetText;

        checkingEnabled = true;
        isTypewriting = false;
    }

    private IEnumerator ETypewriting(string displayText)
    {
        checkingEnabled = false;
        isTypewriting = true;

        string currentDisplay = "";

        int currentChar = 0;

        while(currentChar < displayText.Length)
        {
            currentDisplay += displayText[currentChar];

            currentChar++;

            textMeshPro.text = currentDisplay;

            yield return new WaitForSeconds(delayPerChar);
        }

        textMeshPro.text = displayText;
        lastText = textMeshPro.text;

        checkingEnabled = true;
        isTypewriting = false;
    }

}
