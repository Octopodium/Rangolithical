using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System.Collections;
using System.Collections.Generic;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem instance {get; private set;}

    [Header("Ui Elements")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public Image background;
    [SerializeField] private float textSpeed = 0.05f;

    private DialogueContainer currentDialogue;
    private DialogueNodeData currentNode;
    private Queue<string> currentSentences;
    private bool isTyping = false;
    private string fullText;
    private int counter = -1;

    public MaybeWobble maybeWobble;

    private Dictionary<string, DialogueNodeData> nodeLookup;

    private void Awake(){
        if (instance == null){
            instance = this;
        }else{
            Destroy(gameObject);
        }
    }

    public void StartDialogue(DialogueContainer dialogue){
        currentDialogue = dialogue;

        BuildNodeLookup();
        var entryNode = dialogue.NodeLinks.Find(x => x.portName == "Next");
        currentNode = nodeLookup[entryNode.targetNodeGuid];

        dialoguePanel.SetActive(true);
        DisplayNextSentence();
    }

    private void BuildNodeLookup(){
        nodeLookup = new Dictionary<string, DialogueNodeData>();
        foreach(var node in currentDialogue.DialogueNodeData){
            nodeLookup[node.Guid] = node;
        }
    }

    private void DisplayNextSentence(){
        if(isTyping){
            CompleteSentence();
            return;
        }
        if(currentNode.backgroundSprite != null){
            background.sprite = currentNode.backgroundSprite;
        }

        var localizedString = new LocalizedString(){
            TableReference = "LocalizationTable",
            TableEntryReference = currentNode.Guid
        };

        localizedString.StringChanged += (translatedText) => //evento do localization
        {
            fullText = translatedText;
        };

        StartCoroutine(TypeSentence(fullText));
    }

    IEnumerator TypeSentence(string sentence){
        isTyping = true;
        dialogueText.text = "";
        int index = 0;

        foreach(char letter in sentence.ToCharArray()){
            dialogueText.text += letter;
            if(index > 0){
                yield return null;
                maybeWobble.StartCoroutine("AnimateWobbleChar", dialogueText);
            }
            index++;
            yield return new WaitForSeconds(textSpeed);
        }
        isTyping = false;
    }

    private void CompleteSentence(){
        StopAllCoroutines();
        dialogueText.text = fullText;
        isTyping = false;
    }

    public void AdvanceToNextNode(){
        var link = currentDialogue.NodeLinks.Find(x => x.baseNodeGuid == currentNode.Guid);
        if(link != null && nodeLookup.ContainsKey(link.targetNodeGuid)){
            currentNode = nodeLookup[link.targetNodeGuid];
            if(currentNode.nodeName == "End"){
                EndDialogue();
            }else{
                DisplayNextSentence();
            }
        }else{
            EndDialogue();
        }
    }

    private void EndDialogue(){
        var exitNode = currentDialogue.DialogueNodeData.Find(x => x.nodeName == "End");
        Debug.Log(exitNode);
        if(exitNode._loadScene){
            SceneManager.LoadScene(exitNode.sceneName);
            Debug.Log("Dialogo finalizado comm cena carregada");
        }else{
            Debug.Log("Dialogo finalizado sem carregar cena");
        }
    }
}