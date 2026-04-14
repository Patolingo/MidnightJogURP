using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnswerField : MonoBehaviour
{
    [SerializeField] private Button answerButtonPrefab;

    [SerializeField] private Transform answersPlacement;


    private void Start()
    {
        answersPlacement.DestroyChildren();
    }

    public void SetAnswers(string[] answers)
    {
        answersPlacement.DestroyChildren();

        for (int i = 0; i < answers.Length; i++)
        {
            Button newButton = Instantiate(answerButtonPrefab, answersPlacement);

            TextMeshProUGUI tmpBtn = newButton.GetComponentInChildren<TextMeshProUGUI>();

            tmpBtn.text = answers[i];

            int index = i;

            newButton.onClick.AddListener(() =>
            {
                DialogueManager.Instance.SelectAnswer(index);
            });
        }
    }
    
    public void ClearAnswers()
    {
        answersPlacement.DestroyChildren();
    }
}
