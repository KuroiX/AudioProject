using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class ImmediatelyGoBackScript : MonoBehaviour
{
    private void Awake() {
        Debug.Log("Awake");
    }
    private void Start() {
        Debug.Log("Start");
        SceneManager.LoadScene("Player");
    }
}
