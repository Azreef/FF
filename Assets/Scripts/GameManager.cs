using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Object sceneDefault;

    void Start()
    {
        SceneManager.LoadScene(sceneDefault.name, LoadSceneMode.Additive);
    }

    void Update()
    {
        
    }
}
