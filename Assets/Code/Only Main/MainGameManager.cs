using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.IO;
using UnityEditor.SceneManagement;

public class MainGameManager : MonoBehaviour
{
    
    void Awake()
    {
        MainAudioManager.instance.PLayermainBgm();
    }

    public void GoFIght()
    {
        MainAudioManager.instance.StoperBgm();
        SceneManager.LoadScene(1);
    }

    
}
