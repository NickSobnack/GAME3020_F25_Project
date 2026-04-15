using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class Tooltip : MonoBehaviour
{
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI descriptionText;

    public LayoutElement layoutElement;

    public int characterWrapLimit;

    void Update()
    {
        int headerLength = headerText.text.Length;
        int descriptionLength = descriptionText.text.Length;

        // Enable its layout element if either header or description exceeds the character wrap limit.
        layoutElement.enabled = (headerLength > characterWrapLimit || descriptionLength > characterWrapLimit) ? true : false;
    }

    public void SetTooltip(string header, string description)
    {
        headerText.text = header;
        descriptionText.text = description;
    }
}
