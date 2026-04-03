using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class PositionAnimation
{
    public string name;
    public Vector3 startPosition;
    public Quaternion startRotation;
    public Vector3 endPosition;
    public Quaternion endRotation;
    public float duration = 1f;
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
}

public class PositionAnimator : MonoBehaviour
{
    public PositionAnimation[] animations;

    private float t = 0f;
    private bool playing = false;
    private PositionAnimation current;
    private string[] queue = new string[0];
    private int queueIndex = 0;

    void Start()
    {
        if (animations.Length > 0)
            Play(0);
    }

    void Update()
    {
        if (!playing || current == null) return;

        t += Time.deltaTime / current.duration;
        t = Mathf.Clamp01(t);

        float evaluated = current.curve.Evaluate(t);
        transform.localPosition = Vector3.Lerp(current.startPosition, current.endPosition, evaluated);
        transform.localRotation = Quaternion.Lerp(current.startRotation, current.endRotation, evaluated);

        if (t >= 1f)
        {
            playing = false;
            PlayNextInQueue();
        }
    }

    private void PlayNextInQueue()
    {
        queueIndex++;
        if (queueIndex < queue.Length)
            PlayImmediate(queue[queueIndex]);
    }

    private void PlayImmediate(string animName)
    {
        for (int i = 0; i < animations.Length; i++)
        {
            if (animations[i].name == animName)
            {
                current = animations[i];
                transform.localPosition = current.startPosition;
                transform.localRotation = current.startRotation;
                t = 0f;
                playing = true;
                return;
            }
        }
    }

    public void Play(int index)
    {
        if (index < 0 || index >= animations.Length) return;
        queue = new string[0];
        queueIndex = 0;
        current = animations[index];
        transform.localPosition = current.startPosition;
        transform.localRotation = current.startRotation;
        t = 0f;
        playing = true;
    }

    public void Play(string animName)
    {
        queue = new string[0];
        queueIndex = 0;
        PlayImmediate(animName);
    }

    public void PlayQueue(params string[] animNames)
    {
        if (animNames.Length == 0) return;
        queue = animNames;
        queueIndex = 0;
        PlayImmediate(queue[0]);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PositionAnimator))]
public class PositionAnimatorEditor : Editor
{
    private int selectedIndex = 0;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        PositionAnimator script = (PositionAnimator)target;

        if (script.animations == null || script.animations.Length == 0) return;

        GUILayout.Space(10);

        string[] names = new string[script.animations.Length];
        for (int i = 0; i < names.Length; i++)
            names[i] = string.IsNullOrEmpty(script.animations[i].name) ? $"Animation {i}" : script.animations[i].name;

        selectedIndex = GUILayout.SelectionGrid(selectedIndex, names, 3);

        GUILayout.Space(5);

        if (GUILayout.Button("Fix start position"))
        {
            script.animations[selectedIndex].startPosition = script.transform.localPosition;
            script.animations[selectedIndex].startRotation = script.transform.localRotation;
            EditorUtility.SetDirty(script);
        }

        if (GUILayout.Button("Fix end position"))
        {
            script.animations[selectedIndex].endPosition = script.transform.localPosition;
            script.animations[selectedIndex].endRotation = script.transform.localRotation;
            EditorUtility.SetDirty(script);
        }

        GUILayout.Space(5);

        if (GUILayout.Button("Preview start"))
        {
            script.transform.localPosition = script.animations[selectedIndex].startPosition;
            script.transform.localRotation = script.animations[selectedIndex].startRotation;
        }

        if (GUILayout.Button("Preview end"))
        {
            script.transform.localPosition = script.animations[selectedIndex].endPosition;
            script.transform.localRotation = script.animations[selectedIndex].endRotation;
        }
    }
}
#endif