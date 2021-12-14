using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(VirtualKey))]
public class VirtualKeyEditor : Editor
{
    public VirtualKey selected;

    private List<GUILayoutOption> mButtonLayout = new List<GUILayoutOption>();

    private void OnEnable()
    {
        if (target == null)
        {
            return;
        }

        if (AssetDatabase.Contains(target))
        {
            selected = null;
        }
        else
        {
            selected = (VirtualKey)target;
        }
        mButtonLayout.Add(GUILayout.Height(35));
    }

    public override void OnInspectorGUI()
    {
        if (selected == null)
        {
            return;
        }

        GUILayout.BeginVertical();
        {
            DrawDefaultInspector();
        }
        GUILayout.EndVertical();

        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("오브젝트 이름으로부터 캐릭터 값 가져오기", mButtonLayout.ToArray()))
        {
            foreach(var o in Selection.gameObjects)
            {
                o.GetComponent<VirtualKey>().GetKeyCharacterFromObjectName();
            }
            
        }

        GUILayout.EndHorizontal();
    }

    void OnSceneGUI()
    {
#if false
            // get the chosen game object
            TransferableElement t = selected as TransferableElement;

            if (t == null /*||  t.gameObjects == null*/)
                return;
            /*
            // grab the center of the parent
            Vector3 center = t.transform.position;

            // iterate over game objects added to the array...
            for (int i = 0; i < t.GameObjects.Length; i++)
            {
                // ... and draw a line between them
                if (t.GameObjects[i] != null)
                    Handles.DrawLine(center, t.GameObjects[i].transform.position);
            }
            */

            Handles.DrawLine(t.transform.position, t.transform.position);
#endif
    }
}
