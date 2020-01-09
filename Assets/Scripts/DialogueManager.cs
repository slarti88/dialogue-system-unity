using UnityEngine;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    bool _begin = false;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!_begin)
            {
                begin();
                _begin = true;
            }
            else
            {
                DialogueBehaviour.onClick();
            }
        }
    }

    void begin()
    {
        var ds = DialogueModel.getInstance().dialogues;
        DialogueBehaviour.onShowDialogue(ds[0]);
    }
}
