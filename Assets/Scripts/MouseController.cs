using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class MouseController : MonoBehaviour
{
    [SerializeField] private GameObject slimeTilePrefab;
    [SerializeField] private GameObject slimeHitboxPrefab;
    [SerializeField] private GameObject slimeContainer;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private LayerMask slimeLayer;


    public float dist;
    public float speed;
    public GameObject characterPrefab;
    private CharacterInfo character;
    public GameObject mailBox;

    private PathFinder pathFinder;
    private List<OverlayTile> path = new List<OverlayTile>();
    public Sprite topLeftSprite;
    public Sprite topRightSprite;
    public Sprite bottomLeftSprite;
    public Sprite bottomRightSprite;

    private bool hasSlimed = false;
    public bool onSlime = false;

   private void Start()
   {
        pathFinder = new PathFinder();
        character = characterPrefab.GetComponent<CharacterInfo>();
   }
   void LateUpdate()
   {
       var focusedTileHit = GetFocusedOnTile();
       var currentTileHit = GetCurrentTile();
       var slimeTileHit = GetSlimeTile();

       if (focusedTileHit.HasValue)
       {
           OverlayTile overlayTile = focusedTileHit.Value.collider.gameObject.GetComponent<OverlayTile>();
           if (overlayTile != null)
           {
               transform.position = overlayTile.transform.position;
               gameObject.GetComponent<SpriteRenderer>().sortingOrder = overlayTile.GetComponent<SpriteRenderer>().sortingOrder;
               if (Input.GetMouseButtonDown(0))
               {
                   overlayTile.ShowTile();
                   path = pathFinder.FindPath(character.activeTile, overlayTile);
               }
           }
       }
       if(path.Count > 0)
       {
           MoveAlongPath();
       }

        if (currentTileHit)
       {
           if(currentTileHit.collider.gameObject.tag == "End"){
               ScenesManager.Instance.LoadNextScene();
               mailBox.GetComponent<SpriteRenderer>().enabled = true;
            } else if(currentTileHit.collider.gameObject.tag == "Water"){
                character.SlimeReserve = 100f;
            } else if(currentTileHit.collider.gameObject.tag == "Oil"){
                Debug.Log("Oil");
            }  else if(currentTileHit.collider.gameObject.tag == "Lever"){
                Debug.Log("Lever");
            }
       }

       if (slimeTileHit)
       {
        onSlime = true;
       } else {
        onSlime = false;
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

        if (Vector2.Distance(character.transform.position, targetPosition) < 0.0001f)
        {
            PositionCharacterOnTile(path[0]);
            path.RemoveAt(0);
        }

        if ((movementDirection.y != 0 || movementDirection.x != 0) && !hasSlimed)
        {   
            if(!onSlime)
            {
                hasSlimed = true;
                StartCoroutine(Slime());
            }
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

    IEnumerator Slime()
    {
        var slimeTile = Instantiate(slimeTilePrefab, slimeContainer.transform);
        var slimeBox = Instantiate(slimeHitboxPrefab, slimeContainer.transform);
        slimeTile.transform.position = new Vector3(characterPrefab.transform.position.x, characterPrefab.transform.position.y - 0.30f, characterPrefab.transform.position.z);
        slimeBox.transform.position = new Vector3(characterPrefab.transform.position.x, characterPrefab.transform.position.y + 1.25f, characterPrefab.transform.position.z);
        character.SlimeReserve -= 5f;
        yield return new WaitForSeconds(0.34f);
        hasSlimed = false;
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

    public RaycastHit2D GetCurrentTile()
   {
       Vector2 charPos2d = new Vector2(character.transform.position.x, character.transform.position.y);

       RaycastHit2D hit = Physics2D.Raycast(charPos2d, Vector2.down, 0.25f, interactLayer);

       return hit;
   }    

   public RaycastHit2D GetSlimeTile()
   {
       Vector2 charPos2d = new Vector2(character.transform.position.x, character.transform.position.y);
       RaycastHit2D hit = Physics2D.Raycast(charPos2d, Vector2.up, dist, slimeLayer);

       return hit;
   }    

   private void PositionCharacterOnTile(OverlayTile tile)
   {
       character.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y+0.3f, tile.transform.position.z);
       character.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder + 1;
       character.activeTile = tile;
   }
}