using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[Serializable]
public class DialogueNode : Node
{
    public string GUID;
    public string NodeName;
    public string DialogueText;
    public bool _entryPoint = false;
    public bool _exitPoint = false;

    public string sceneName;
    public bool _loadScene = false;

    public Sprite backgroundSprite;
    public ObjectField backgroundSpriteField;
}
