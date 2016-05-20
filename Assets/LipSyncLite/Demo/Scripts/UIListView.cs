using UnityEngine;
using System;
using System.Collections.Generic;

public class UIListView : MonoBehaviour 
{
    public float entryHeight;
    
    public RectTransform listViewContent;

    public UIListViewEntry listViewEntryPrefab;

    private List<UIListViewEntry> listViewEntryList = new List<UIListViewEntry>();

    public UIListViewEntry AddEntry(string name, Action action)
    {
        UIListViewEntry entry = Instantiate(listViewEntryPrefab) as UIListViewEntry;
        entry.EntryName = name;
        listViewEntryList.Add(entry);
        entry.transform.SetParent(listViewContent);
        entry.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        listViewContent.sizeDelta = new Vector2(listViewContent.sizeDelta.x, listViewContent.sizeDelta.y + entryHeight);

        return entry;
    }
    
}
