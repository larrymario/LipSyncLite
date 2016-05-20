using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour 
{
    #region Singleton
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<UIManager>();
            }
            return instance;
        }
    }

    private static UIManager instance;
    #endregion

    public UIListView voiceFilelistView;
    public UIDialogView dialogView;
}
