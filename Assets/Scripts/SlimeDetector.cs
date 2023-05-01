using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeDetector : MonoBehaviour
{   
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private bool New = true;
    private bool slimeChecked = false;
    private GameObject Cursor;

    void Update()
    {

        Vector2 slimePos2d = new Vector2(transform.position.x, transform.position.y);
        RaycastHit2D hit = Physics2D.Raycast(slimePos2d, Vector2.up, 0.25f, playerLayer);

        if (cursor == null)
        {
            cursor = GameObject.FindGameObjectsWithTag("Cursor");
        }

        if (!hit)
        {   
            if(!slimeChecked){
                if(New){
                    New = false;
                } else if (!New) {
                    Cursor.GetComponent<MouseController>().onSlime = false;
                    Destroy(gameObject);
                }
                slimeChecked = true;
            }
        } else if (hit && !New) {
            Debug.Log("Entering old slime");
            Cursor.GetComponent<MouseController>().onSlime = true;
            slimeChecked = false;
        }
    }
}
