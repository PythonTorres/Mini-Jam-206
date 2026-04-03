#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class CopyLightingFromScene : MonoBehaviour
{
    public string sourceSceneName;

    [ContextMenu("Copy Lighting From Source Scene")]
    void CopyEnvironment()
    {
        string[] guids = AssetDatabase.FindAssets($"{sourceSceneName} t:Scene");
        if (guids.Length == 0)
        {
            Debug.LogWarning($"Scene '{sourceSceneName}' not found.");
            return;
        }

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        Scene current = SceneManager.GetActiveScene();

        Scene source = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);

        SceneManager.SetActiveScene(source);
        Material skybox             = RenderSettings.skybox;
        AmbientMode ambientMode     = RenderSettings.ambientMode;
        Color ambientSky            = RenderSettings.ambientSkyColor;
        Color ambientEquator        = RenderSettings.ambientEquatorColor;
        Color ambientGround         = RenderSettings.ambientGroundColor;
        Color ambientLight          = RenderSettings.ambientLight;
        float ambientIntensity      = RenderSettings.ambientIntensity;
        Color subtractive           = RenderSettings.subtractiveShadowColor;
        bool  fog                   = RenderSettings.fog;
        Color fogColor              = RenderSettings.fogColor;
        FogMode fogMode             = RenderSettings.fogMode;
        float fogDensity            = RenderSettings.fogDensity;
        float fogStart              = RenderSettings.fogStartDistance;
        float fogEnd                = RenderSettings.fogEndDistance;
        float reflIntensity         = RenderSettings.reflectionIntensity;
        int   reflBounces           = RenderSettings.reflectionBounces;
        Light sun                   = RenderSettings.sun;

        SceneManager.SetActiveScene(current);
        EditorSceneManager.CloseScene(source, true);

        RenderSettings.skybox                 = skybox;
        RenderSettings.ambientMode            = ambientMode;
        RenderSettings.ambientSkyColor        = ambientSky;
        RenderSettings.ambientEquatorColor    = ambientEquator;
        RenderSettings.ambientGroundColor     = ambientGround;
        RenderSettings.ambientLight           = ambientLight;
        RenderSettings.ambientIntensity       = ambientIntensity;
        RenderSettings.subtractiveShadowColor = subtractive;
        RenderSettings.fog                    = fog;
        RenderSettings.fogColor               = fogColor;
        RenderSettings.fogMode                = fogMode;
        RenderSettings.fogDensity             = fogDensity;
        RenderSettings.fogStartDistance       = fogStart;
        RenderSettings.fogEndDistance         = fogEnd;
        RenderSettings.reflectionIntensity    = reflIntensity;
        RenderSettings.reflectionBounces      = reflBounces;
        if (sun) RenderSettings.sun           = sun;

        EditorUtility.SetDirty(gameObject.scene.GetRootGameObjects()[0]);
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        Debug.Log($"Lighting copied from '{sourceSceneName}'");
    }
}
#endif