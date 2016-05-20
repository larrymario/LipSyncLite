using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIListViewEntry : MonoBehaviour 
{
    public Text entryText;

    private string entryName;

    public string EntryName
    {
        get
        {
            return entryName;
        }
        set
        {
            entryName = value;
            entryText.text = entryName;
        }
    }

    public void PlayVoice()
    {
        LipSyncDemoManager.Instance.PlayVoice(entryName);
    }

}
