using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class DrawSplines : MonoBehaviour {
    public Color color = Color.red;
    public bool enabled = true;
    public bool draw_points = true;
    public bool draw_lines = true;
    public bool draw_direction = true;
    public bool draw_arc = true;
    public float size = .05f;
    [Range(0f, 1f)]
    public float direction_opacity = .4f;    

    void Start() {
        // for(int i = 0; i < this.transform.childCount; i++) {
        //     Transform Children = this.transform.GetChild(i);
        //     for(int n = 0; n < Children.transform.childCount; n++) {
        //         Transform Sibling = Children.transform.GetChild(n);
        //         if (Sibling.gameObject.GetComponent<Mover>() == null) {
        //             Sibling.gameObject.AddComponent<Mover>();
        //         }
        //     }
        // }
    }

    private void OnDrawGizmos() {
        if(enabled == true) {
            for(int i = 0; i < this.transform.childCount; i++)
            {
                Transform Children = this.transform.GetChild(i);
                Vector3 lastSibling_position = new Vector3();
                Quaternion lastSibling_rotation = new Quaternion();
                for(int n = 0; n < Children.transform.childCount; n++)
                {
                    Transform Sibling = Children.transform.GetChild(n);
                    if (lastSibling_position.x != 0) {
                        if(draw_lines) DrawLine(lastSibling_position, Sibling.transform.position);
                    }

                    if(n + 1 < Children.transform.childCount) {
                        Transform Next = Children.transform.GetChild(n + 1);
                        Sibling.transform.LookAt(Next.position);
                    }
                    else {
                        if(n + 1 == Children.transform.childCount) {
                            Sibling.transform.rotation = lastSibling_rotation;
                        }
                    }

                    if(draw_points) DrawBox(Sibling, n);
                    if(draw_direction) DrawLine(Sibling.transform.position, Sibling.transform.position + .3f * Sibling.transform.forward, direction_opacity);
                    lastSibling_position = Sibling.transform.position;
                    lastSibling_rotation = Sibling.transform.rotation;
                }
            }
        }
    }

    private void DrawBox(Transform obj, int i, float alpha = 1f) {
        Vector3 pos = obj.transform.position;
        color.a = alpha;
        Handles.color = color;
        Gizmos.color = color;
        Handles.DrawWireCube(pos, new Vector3(size, size, size));
        Gizmos.DrawCube(pos, new Vector3(size + .001f, size + .001f, size + .001f));

        if (draw_arc) {
            Handles.DrawWireArc(obj.transform.position, obj.transform.up, -obj.transform.right, 360, size * 1.5f);
            color.a = .1f;
            Handles.color = color;
            Handles.DrawSolidDisc(obj.transform.position, obj.transform.up, size * 1.5f);
        }
    }

    private void DrawLine(Vector3 start, Vector3 end, float alpha = 1f) {
        color.a = alpha;
        Handles.color = color;
        Handles.DrawLine(start, end);
    }

    private void DrawIcon(GameObject gameObject, int idx) {
        var largeIcons = GetTextures("sv_icon_dot", "_pix16_gizmo", 0, 16);
        var icon = largeIcons[idx];
        var egu = typeof(EditorGUIUtility);
        var flags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
        var args = new object[] { gameObject, icon.image };
        var setIcon = egu.GetMethod("SetIconForObject", flags, null, new Type[]{typeof(UnityEngine.Object), typeof(Texture2D)}, null);
        setIcon.Invoke(null, args);
    }

    private GUIContent[] GetTextures(string baseName, string postFix, int startIndex, int count) {
        GUIContent[] array = new GUIContent[count];
        for (int i = 0; i < count; i++)
        {
            EditorGUIUtility.SetIconSize(new Vector2(1f, 1f));
            array[i] = EditorGUIUtility.IconContent(baseName + (startIndex + i) + postFix);
        }
        return array;
    }
}
#endif