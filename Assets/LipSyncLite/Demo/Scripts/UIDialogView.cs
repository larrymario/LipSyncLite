using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIDialogView : MonoBehaviour 
{
    public Text dialogueText;

    private string currentDialogue;

    public string CurrentDialogue
    {
        get
        {
            return currentDialogue;
        }
        set
        {
            currentDialogue = value;
            dialogueText.text = currentDialogue;
        }
    }
}
