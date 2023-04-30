using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    [SerializeField] Button QuitBtn;

    // Start is called before the first frame update
    void Start()
    {
        QuitBtn.onClick.AddListener(Menu);
    }

    private void Menu() 
    {
        ScenesManager.Instance.LoadMainMenu();
    }
}
