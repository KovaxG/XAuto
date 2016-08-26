using UnityEngine;
using System.Collections;

public class LockCursore : MonoBehaviour {

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false;
    }
    void Update() { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }
}