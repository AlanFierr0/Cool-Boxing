using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple helper to show score in a UI Text (or Debug if none assigned).
/// </summary>
public class MotionScoreDisplay : MonoBehaviour
{
    public Text uiText;

    public void ShowScore(float score)
    {
        string s = $"Score: {score:F1}";
        if (uiText != null)
        {
            uiText.text = s;
        }
        else
        {
            Debug.Log(s);
        }
    }
}

