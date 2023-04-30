using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class CharacterInfo : MonoBehaviour
{   
    private const float MAX_SLIME = 100f;
    public OverlayTile activeTile;
    public float SlimeReserve = MAX_SLIME;

    [SerializeField] private Image slimeBar;

    private void Update() {
        slimeBar.fillAmount = SlimeReserve / MAX_SLIME;
    }

}
