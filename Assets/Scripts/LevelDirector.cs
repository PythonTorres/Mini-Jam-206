using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelDirector : MonoBehaviour {
    public enum ActionType { GameObject, Shot }

    [System.Serializable]
    public class LevelAction {
        public string label;
        public ActionType type;
        public GameObject target;
        public float param;
        public float timestamp;
    }

    public List<LevelAction> actions = new List<LevelAction>();
    public MonoBehaviour shotHandler;

    float playbackTime = 0f;
    bool playing = false;

    void Start() {
        StartCoroutine(RunSequence());
    }

    IEnumerator RunSequence() {
        playing = true;
        playbackTime = 0f;

        var sorted = new List<LevelAction>(actions);
        sorted.Sort((a, b) => a.timestamp.CompareTo(b.timestamp));

        foreach (var action in sorted) {
            while (playbackTime < action.timestamp) {
                playbackTime += Time.deltaTime;
                yield return null;
            }

            ExecuteAction(action);
        }

        // keep timer running after sequence ends for timestamp capture
        while (true) {
            playbackTime += Time.deltaTime;
            yield return null;
        }
    }

    void ExecuteAction(LevelAction action) {
        switch (action.type) {
            case ActionType.GameObject:
                if (action.target == null) break;
                action.target.SetActive(true);
                foreach (var exec in action.target.GetComponents<IExecutable>())
                    exec.Exec(action.param);
                break;

            case ActionType.Shot:
                if (shotHandler is IShotHandler handler)
                    handler.OnShot();
                break;
        }
    }

    public void CaptureTimestamp(int index) {
        if (index < 0 || index >= actions.Count) return;
        actions[index].timestamp = playbackTime;
        Debug.Log($"Captured {playbackTime:F2}s → [{index}] {actions[index].label}");
    }

    public float GetPlaybackTime() => playbackTime;
    public bool IsPlaying() => playing;
}

public interface IExecutable {
    void Exec(float param);
}

public interface IShotHandler {
    void OnShot();
}

#if UNITY_EDITOR
[CustomEditor(typeof(LevelDirector))]
public class LevelDirectorEditor : Editor {
    void OnEnable() => EditorApplication.update += Repaint;
    void OnDisable() => EditorApplication.update -= Repaint;

    public override void OnInspectorGUI() {
        LevelDirector director = (LevelDirector)target;

        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("Level Director", EditorStyles.boldLabel);
        EditorGUILayout.Space(4);

        if (Application.isPlaying) {
            EditorGUILayout.LabelField(
                $"Playback time: {director.GetPlaybackTime():F2}s  |  Playing: {director.IsPlaying()}"
            );
            EditorGUILayout.Space(4);
        }

        DrawDefaultInspector();

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Timestamp Capture", EditorStyles.boldLabel);

        if (!Application.isPlaying) {
            EditorGUILayout.HelpBox("Enter Play Mode to capture timestamps.", MessageType.Info);
            return;
        }

        for (int i = 0; i < director.actions.Count; i++) {
            var action = director.actions[i];
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(
                $"[{i}] {action.label} ({action.type})", GUILayout.Width(220)
            );
            EditorGUILayout.LabelField($"t = {action.timestamp:F2}s", GUILayout.Width(80));

            if (GUILayout.Button("Capture Now", GUILayout.Width(110))) {
                Undo.RecordObject(director, "Capture Timestamp");
                director.CaptureTimestamp(i);
                EditorUtility.SetDirty(director);
            }

            float newTs = EditorGUILayout.Slider(action.timestamp, 0f, 120f);
            if (!Mathf.Approximately(newTs, action.timestamp)) {
                Undo.RecordObject(director, "Set Timestamp");
                action.timestamp = newTs;
                EditorUtility.SetDirty(director);
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif