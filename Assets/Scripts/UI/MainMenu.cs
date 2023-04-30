using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Button StartBtn;

    // Start is called before the first frame update
    void Start()
    {
        StartBtn.onClick.AddListener(StartGame);
    }

    private void StartGame() 
    {
        ScenesManager.Instance.LoadScene(ScenesManager.Scene.Level01);
    }
}
