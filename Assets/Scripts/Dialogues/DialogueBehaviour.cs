using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using DG.Tweening;

public class DialogueBehaviour : MonoBehaviour
{

    public static System.Action onPlayDialogue = delegate {};
    public static System.Action onStopDialogue = delegate {};
    public static System.Action onDialogueClosed = delegate {};
    public static System.Action onDialogueOpened = delegate { };
    public static System.Action onClick = delegate { };
    public static System.Action<int> onAutoPlay = delegate { };

    public static System.Action<DialogueHolder> onShowDialogue = delegate { };

    public TextMeshProUGUI dialogueText;

    public GameObject dialogueTextObject;
    public GameObject dialogueCharaObject;

    //public GameObject optionsObject;
    //public GameObject optionsParent;
    public GameObject dialogueContainer;

    private bool _isAnimating;
    IEnumerator _dialogueRoutine;
    string _currentDialogue;
    DialogueHolder _currentHolder;

    int _currentCharaIndex;
    int _currentDialogueIndex;
    

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        onShowDialogue += showDialogue;
        onClick += onNextClicked;
    }

    private void OnDisable()
    {
        onShowDialogue -= showDialogue;
        onClick -= onNextClicked;
    }

    void showDialogue(DialogueHolder holder)
    {

        dialogueTextObject.SetActive(true);
        dialogueCharaObject.SetActive(true);

        _currentHolder = holder;
        _currentCharaIndex = 0;

        //drawDialogueText(model.dialogues)
        showCharaDialogues();

        onDialogueOpened();

    }


    void drawDialogueText(string dialogue)
    {
        //dialogueText.text = _dialogues[dialogueIndex];
        _dialogueRoutine = dialogueRoutine(dialogue);
        StartCoroutine(_dialogueRoutine);
    }


    IEnumerator dialogueRoutine(string dialogue)
    {
        dialogueText.text = "";
        dialogueContainer.transform.DOScale(Vector3.one, .25f);
        yield return new WaitForSeconds(.5f);

        onPlayDialogue();
        _isAnimating = true;
        var dialogStr = dialogue;

        for (int i = 0; i < dialogStr.Length - 1; i += 2)
        {
            if (dialogStr[i].Equals('<'))
            {
                int endTag = dialogStr.IndexOf('>', i);
                i = Mathf.Min(endTag + 1, dialogStr.Length - 1);
            }
            var d = dialogStr[i];
            var dp1 = dialogStr[i + 1];

            if (d.Equals(',') || dp1.Equals(','))
            {
                if (d.Equals(','))
                {
                    dialogueText.text = dialogStr.Substring(0, i);
                }
                else
                {
                    dialogueText.text = dialogStr.Substring(0, i + 1);
                }
                onStopDialogue();
                yield return new WaitForSeconds(.5f);
                onPlayDialogue();
            }

            dialogueText.text = dialogStr.Substring(0, i);
            yield return null;


        }
        dialogueText.text = dialogStr;

        yield return new WaitForSeconds(3);
        dialogueContainer.transform.DOScale(Vector3.zero, .25f);

        _isAnimating = false;

        onStopDialogue();

        var currChara = _currentHolder.dialogues[_currentCharaIndex];
        if (currChara.auto) {
            yield return new WaitForSeconds(1.5f);
            if (onAutoPlay != null) {
                var dialoguesLeft = getDialoguesLeft();
                onAutoPlay(dialoguesLeft);
            }
            _currentDialogueIndex++;
            showNextDialogue();
        }
    }

    void onNextClicked()
    {

        //if (optionsParent.activeSelf) return;

        var currChara = _currentHolder.dialogues[_currentCharaIndex];
        if (currChara.auto) return;

        if (_isAnimating)
        {
            StopCoroutine(_dialogueRoutine);
            onStopDialogue();
            dialogueText.text = _currentHolder.dialogues[_currentCharaIndex].dialogues[_currentDialogueIndex];
            _isAnimating = false;
        }
        else
        {
            _currentDialogueIndex++;
            showNextDialogue();
        }
    }

    void showNextDialogue()
    {
        var detail = _currentHolder.dialogues[_currentCharaIndex];
        if ((_currentDialogueIndex < detail.dialogues.Length) && 
            (detail.options == null || detail.options.Length == 0))
        {
            var dialogue = detail.dialogues[_currentDialogueIndex];
            dialogueCharaObject.GetComponentInChildren<Image>().sprite = getSpriteForChara(detail);
            drawDialogueText(dialogue);
        } else {
            _currentCharaIndex++;
            showCharaDialogues();
        }

    }

    void showCharaDialogues()
    {
        if (_currentCharaIndex < _currentHolder.dialogues.Length)
        {
            _currentDialogueIndex = 0;
            var detail = _currentHolder.dialogues[_currentCharaIndex];
            if (detail.options!= null && detail.options.Length > 0)
            {
                //showOptions(detail);
            }
            else
            {
                //dialogueCharaObject.GetComponentInChildren<TextMeshProUGUI>().text = detail.chara;
                showNextDialogue();
            }
        } else {
            finishDialogue();
        }
    }

    void finishDialogue()
    {
        dialogueTextObject.SetActive(false);
        dialogueCharaObject.SetActive(false);

        _currentHolder = null;
        _currentCharaIndex = 0;
        onDialogueClosed();
    }

    //void showOptions(DialogueDetail detail)
    //{
    //    optionsParent.SetActive(true);

    //    for (int i = 0; i < optionsParent.transform.childCount; ++i)
    //    {
    //        Destroy(optionsParent.transform.GetChild(i).gameObject);
    //    }


    //    int index = 0;
    //    foreach (var option in detail.options)
    //    {
    //        var optionsObj = Instantiate(optionsObject,optionsParent.transform);
    //        int tempIndex = index;
    //        optionsObj.AddComponent<Button>().onClick.AddListener(() =>
    //        {
    //            optionsParent.SetActive(false);
    //            _currentDialogueIndex = tempIndex;
    //            var dialogue = detail.dialogues[tempIndex];
    //            drawDialogueText(dialogue);
    //        });
    //        ++index;
    //        optionsObj.GetComponentInChildren<TextMeshProUGUI>().text = option;
    //    }
    //}

    Sprite getSpriteForChara(DialogueDetail detail)
    {
        var foldername = detail.chara;
        var em = detail.emotions[_currentDialogueIndex];

        var sprite = Resources.Load<Sprite>("Avatars/" + foldername + "/" + em);
        if (sprite == null) {
            sprite = Resources.Load<Sprite>("Avatars/" + foldername + "/normal");
        }

        return sprite;
    }

    int getDialoguesLeft()
    {
        int count = 0;
        for (int i = _currentCharaIndex; i < _currentHolder.dialogues.Length; ++i)
        {
            count += _currentHolder.dialogues[i].dialogues.Length;
        }

        return count;
    }

}
