using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class MouseController : MonoBehaviour
{

    public float speed;

    [SerializeField] private MapManager map;
    private CharacterInfo character;

    private PathFinder pathFinder;
    private List<OverlayTile> path = new List<OverlayTile>();
    public Sprite topLeftSprite;
    public Sprite topRightSprite;
    public Sprite bottomLeftSprite;
    public Sprite bottomRightSprite;

   private void Start()
   {
        pathFinder = new PathFinder();
        character = map.snailCharacter;
   }
   void LateUpdate()
   {
        character = map.snailCharacter;

       var focusedTileHit = GetFocusedOnTile();

        if (focusedTileHit.HasValue)
       {
           OverlayTile overlayTile = focusedTileHit.Value.collider.gameObject.GetComponent<OverlayTile>();
           transform.position = overlayTile.transform.position;
           gameObject.GetComponent<SpriteRenderer>().sortingOrder = overlayTile.GetComponent<SpriteRenderer>().sortingOrder;
            
           if (Input.GetMouseButtonDown(0))
           {
                overlayTile.ShowTile();
                path = pathFinder.FindPath(character.activeTile, overlayTile);
               
           }
       }
       if(path.Count > 0)
       {
           MoveAlongPath();
       }
   }
   private void MoveAlongPath()
    {
        var step = speed * Time.deltaTime;

        float zIndex = path[0].transform.position.z;
        Vector3 targetPosition = path[0].transform.position + new Vector3(0, 0.3f, 0);
        Vector3 movementDirection = targetPosition - character.transform.position;
        movementDirection.z = 0;
        movementDirection.Normalize();
        character.transform.position = Vector2.MoveTowards(character.transform.position, targetPosition, step);
        character.transform.position = new Vector3(character.transform.position.x, character.transform.position.y, zIndex);

        if (Vector2.Distance(character.transform.position, targetPosition) < 0.00000001f)
        {
            PositionCharacterOnTile(path[0]);
            path.RemoveAt(0);
        }

        if (movementDirection.y > 0)
        {
            if (movementDirection.x > 0)
            {
                character.GetComponent<SpriteRenderer>().sprite = topRightSprite;

            }
            else
            {
                character.GetComponent<SpriteRenderer>().sprite = topLeftSprite;
            }
        }
        else
        {
            if (movementDirection.x > 0)
            {
                character.GetComponent<SpriteRenderer>().sprite = bottomRightSprite;
            }
            else
            {
                character.GetComponent<SpriteRenderer>().sprite = bottomLeftSprite;
            }
        }
    }


   public RaycastHit2D? GetFocusedOnTile()
   {
       Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
       Vector2 mousePos2d = new Vector2(mousePos.x, mousePos.y);

       RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2d, Vector2.zero);

       if(hits.Length > 0)
       {
           return hits.OrderByDescending(i => i.collider.transform.position.z).First();
       }
       return null;
   }

   private void PositionCharacterOnTile(OverlayTile tile)
   {
       character.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y+0.3f, tile.transform.position.z);
       character.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder + 1;
       character.activeTile = tile;
   }
}