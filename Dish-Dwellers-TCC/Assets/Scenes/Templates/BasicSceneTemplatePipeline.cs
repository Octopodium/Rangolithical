
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.SceneTemplate;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BasicSceneTemplatePipeline : ISceneTemplatePipeline
{
    public virtual bool IsValidTemplateForInstantiation(SceneTemplateAsset sceneTemplateAsset)
    {
        return true;
    }

    public virtual void BeforeTemplateInstantiation(SceneTemplateAsset sceneTemplateAsset, bool isAdditive, string sceneName)
    {
        
    }

    public virtual void AfterTemplateInstantiation(SceneTemplateAsset sceneTemplateAsset, Scene scene, bool isAdditive, string sceneName){
        if(!EditorUtility.DisplayDialog("Save", "Save the scene", "Yes", "No"))
            return;
        
        string path = EditorUtility.SaveFilePanel("Save Scene As", "Assets/Scenes",scene.name, "unity");

        if(string.IsNullOrEmpty(path))
            return;
        
        path = FileUtil.GetProjectRelativePath(path);
        //EditorSceneManager.SaveScene(scene, path);

        AssetDatabase.Refresh();

        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        scenes.Add(new EditorBuildSettingsScene(path, true));
        EditorBuildSettings.scenes = scenes.ToArray();
    }
}
