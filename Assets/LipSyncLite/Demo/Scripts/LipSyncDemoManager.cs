using UnityEngine;
using System.Collections.Generic;

public class LipSyncDemoManager : MonoBehaviour
{
    #region Singleton
    public static LipSyncDemoManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<LipSyncDemoManager>();
            }
            return instance;
        }
    }

    private static LipSyncDemoManager instance;
    #endregion

    public AudioSource targetAudioSource;
    public string voiceFileDirectory;

    private Dictionary<string, AudioClip> voiceClipDict;
    private Dictionary<string, string> dialogueDict;

    public void PlayVoice(string filename)
    {
        AudioClip voiceToPlay;
        if (voiceClipDict.TryGetValue(filename, out voiceToPlay) == false)
        {
            voiceToPlay = Resources.Load<AudioClip>(voiceFileDirectory + "/" + filename);
        }

        targetAudioSource.Stop();
        targetAudioSource.clip = voiceToPlay;
        targetAudioSource.Play();

        SetDialogue(dialogueDict[filename]);
    }

    public void SetDialogue(string dialogue)
    {
        UIManager.Instance.dialogView.CurrentDialogue = dialogue;
    }

    void Start()
    {
        TextAsset voiceListFile = Resources.Load("UnitychanVoiceFileNameList") as TextAsset;
        string[] tupleList = voiceListFile.text.Split('\n');

        dialogueDict = new Dictionary<string, string>();
        foreach (string s in tupleList)
        {
            string[] tupleEntry = s.Split(',');
            UIManager.Instance.voiceFilelistView.AddEntry(tupleEntry[0], null);
            dialogueDict[tupleEntry[0]] = tupleEntry[1];
        }

        voiceClipDict = new Dictionary<string, AudioClip>();
    }
}
