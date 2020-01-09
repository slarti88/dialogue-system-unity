using UnityEngine;
using System;

[Serializable]
public class DialogueModel
{
    public static DialogueModel _instance;

    public static DialogueModel getInstance()
    {
        if (_instance == null) {
            _instance = createFromFile();
        }
        return _instance;
    }

    public static DialogueModel createFromFile()
    {
        var textAsset = Resources.Load("dialogues") as TextAsset;
        return JsonUtility.FromJson<DialogueModel>(textAsset.text);
    }

    public DialogueHolder[] dialogues;

    public DialogueHolder getDialogueFor(string id)
    {
        foreach (var d in dialogues)
        {
            if (d.id.Equals(id)) {
                return d;
            }
        }
        return null;
    }

}

[Serializable]
public class DialogueHolder {

    public string id;
    public DialogueDetail[] dialogues;
}

[Serializable]
public class DialogueDetail {

    public string chara;
    public string[] dialogues;
    public string[] emotions;
    public string[] options;
    public bool auto;

}
