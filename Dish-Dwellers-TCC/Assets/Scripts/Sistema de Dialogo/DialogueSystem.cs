/*using TMPro;
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
        if(currentNode.backgroundSprite != null){
            background.sprite = currentNode.backgroundSprite;
        }

        if(isTyping){
            CompleteSentence();
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

        foreach(char letter in sentence.ToCharArray()){
            dialogueText.text += letter;
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
}*/
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
    
    [Header("Animation Settings")]
    [SerializeField] private float shakeIntensity = 2f;
    [SerializeField] private float scaleDuration = 0.3f;

    private DialogueContainer currentDialogue;
    private DialogueNodeData currentNode;
    private Queue<string> currentSentences;
    private bool isTyping = false;
    private string fullText;
    private int counter = -1;

    private Dictionary<string, DialogueNodeData> nodeLookup;
    private List<Coroutine> activeAnimations = new List<Coroutine>();

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
        if(currentNode.backgroundSprite != null){
            background.sprite = currentNode.backgroundSprite;
        }

        if(isTyping){
            CompleteSentence();
        }

        var localizedString = new LocalizedString(){
            TableReference = "LocalizationTable",
            TableEntryReference = currentNode.Guid
        };

        localizedString.StringChanged += (translatedText) => 
        {
            fullText = translatedText;
        };

        StartCoroutine(TypeSentence(fullText));
    }

    IEnumerator TypeSentence(string sentence){
        isTyping = true;
        dialogueText.text = "";
        
        // Processar palavras especiais
        string processedText = ProcessSpecialWords(sentence);
        
        foreach(char letter in processedText.ToCharArray()){
            dialogueText.text += letter;
            
            // Se for o início de uma palavra especial, iniciar animações
            if(letter == '{'){
                // Aguardar a palavra especial ser completamente digitada
                int endIndex = dialogueText.text.IndexOf('}', dialogueText.text.Length - 1);
                if(endIndex == -1) continue; // Ainda não terminou de digitar a palavra
                
                string specialWord = dialogueText.text.Substring(dialogueText.text.LastIndexOf('{'), 
                    endIndex - dialogueText.text.LastIndexOf('{') + 1);
                StartWordAnimations(specialWord);
            }
            
            yield return new WaitForSeconds(textSpeed);
        }
        isTyping = false;
    }

    private string ProcessSpecialWords(string text){
        // Adicionar tags de cor às palavras especiais
        return text.Replace("{", "<color=yellow>{").Replace("}", "}</color>");
    }

    private void StartWordAnimations(string wordWithBrackets){
        // Remover as chaves para pegar apenas a palavra
        string word = wordWithBrackets.Replace("{", "").Replace("}", "");
        
        // Encontrar a posição da palavra no texto
        int wordStartIndex = dialogueText.text.LastIndexOf(wordWithBrackets);
        if(wordStartIndex == -1) return;
        
        // Iniciar animações para cada caractere da palavra
        for(int i = 0; i < word.Length; i++){
            int charIndex = wordStartIndex + 1 + i; // +1 para pular o {
            if(charIndex < dialogueText.textInfo.characterCount){
                Coroutine scaleCoroutine = StartCoroutine(ScaleCharacter(charIndex));
                Coroutine shakeCoroutine = StartCoroutine(ShakeCharacter(charIndex));
                
                activeAnimations.Add(scaleCoroutine);
                activeAnimations.Add(shakeCoroutine);
            }
        }
    }

    IEnumerator ScaleCharacter(int charIndex){
        TMP_TextInfo textInfo = dialogueText.textInfo;
        
        if(charIndex >= textInfo.characterCount) yield break;
        
        TMP_CharacterInfo charInfo = textInfo.characterInfo[charIndex];
        if(!charInfo.isVisible) yield break;
        
        Matrix4x4 matrix;
        float elapsedTime = 0f;
        Vector3 originalScale = Vector3.one;
        
        while(elapsedTime < scaleDuration){
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / scaleDuration;
            
            // Scale de 1.2 para 1
            float currentScale = Mathf.Lerp(1.2f, 1f, progress);
            
            // Atualizar a mesh do caractere
            int meshIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;
            
            Vector3[] vertices = textInfo.meshInfo[meshIndex].vertices;
            
            // Aplicar scale no caractere
            Vector3 center = (vertices[vertexIndex] + vertices[vertexIndex + 2]) * 0.5f;
            
            vertices[vertexIndex] = center + (vertices[vertexIndex] - center) * currentScale;
            vertices[vertexIndex + 1] = center + (vertices[vertexIndex + 1] - center) * currentScale;
            vertices[vertexIndex + 2] = center + (vertices[vertexIndex + 2] - center) * currentScale;
            vertices[vertexIndex + 3] = center + (vertices[vertexIndex + 3] - center) * currentScale;
            
            dialogueText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
            
            yield return null;
        }
    }

    IEnumerator ShakeCharacter(int charIndex){
        TMP_TextInfo textInfo = dialogueText.textInfo;
        
        if(charIndex >= textInfo.characterCount) yield break;
        
        TMP_CharacterInfo charInfo = textInfo.characterInfo[charIndex];
        if(!charInfo.isVisible) yield break;
        
        int meshIndex = charInfo.materialReferenceIndex;
        int vertexIndex = charInfo.vertexIndex;
        Vector3[] originalVertices = textInfo.meshInfo[meshIndex].vertices.Clone() as Vector3[];
        
        float shakeDuration = scaleDuration; // mesma duração do scale
        float elapsedTime = 0f;
        
        while(elapsedTime < shakeDuration){
            elapsedTime += Time.deltaTime;
            
            Vector3[] vertices = textInfo.meshInfo[meshIndex].vertices;
            Vector2 shakeOffset = Random.insideUnitCircle * shakeIntensity;
            
            // Aplicar shake em todos os vértices do caractere
            for(int i = 0; i < 4; i++){
                vertices[vertexIndex + i] = originalVertices[vertexIndex + i] + (Vector3)shakeOffset;
            }
            
            dialogueText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
            
            yield return null;
        }
        
        // Restaurar posição original
        Vector3[] finalVertices = textInfo.meshInfo[meshIndex].vertices;
        for(int i = 0; i < 4; i++){
            finalVertices[vertexIndex + i] = originalVertices[vertexIndex + i];
        }
        dialogueText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
    }

    private void CompleteSentence(){
        StopAllCoroutines();
        
        // Parar todas as animações ativas
        foreach(Coroutine anim in activeAnimations){
            if(anim != null) StopCoroutine(anim);
        }
        activeAnimations.Clear();
        
        dialogueText.text = fullText;
        isTyping = false;
        
        // Processar animações para todas as palavras especiais no texto completo
        ProcessAllSpecialWordsInCompleteText();
    }

    private void ProcessAllSpecialWordsInCompleteText(){
        string text = dialogueText.text;
        int startIndex = 0;
        
        while(startIndex < text.Length){
            int openBracket = text.IndexOf('{', startIndex);
            if(openBracket == -1) break;
            
            int closeBracket = text.IndexOf('}', openBracket);
            if(closeBracket == -1) break;
            
            string specialWord = text.Substring(openBracket, closeBracket - openBracket + 1);
            StartWordAnimations(specialWord);
            
            startIndex = closeBracket + 1;
        }
    }

    public void AdvanceToNextNode(){
        // Limpar animações antes de avançar
        foreach(Coroutine anim in activeAnimations){
            if(anim != null) StopCoroutine(anim);
        }
        activeAnimations.Clear();
        
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