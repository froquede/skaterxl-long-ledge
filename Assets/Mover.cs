using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class Mover : MonoBehaviour {
    private bool selected = false;
    
    private void OnDrawGizmos() {

    }
    
    void OnDrawGizmosSelected() {
        if (!selected) {
            selected = true;
            Vector3 pos = this.transform.position;
            Quaternion rot = this.transform.rotation;
            // Handles.TransformHandle(ref pos, ref rot);
            Debug.Log("Here");
            Debug.Log(this.gameObject.name);
        }
    }
}
#endif