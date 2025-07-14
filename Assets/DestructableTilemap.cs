using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections;

public class DestructableTilemap : MonoBehaviour
{
    [SerializeField] GridLayout grid;
    [SerializeField] GameObject miningHitBox;
    [SerializeField] GameObject hitTileGameObject;
    [SerializeField] LayerMask tilemapLayer;
    Player player;
    Vector3Int tilePos;
    Tilemap tilemap;

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        player = GameObject.Find("Player GameObject").GetComponent<Player>();
    }

    void Update()
    {
        RaycastHit2D hit = Physics2D.CircleCast(miningHitBox.transform.position, .01f, Vector2.zero, .01f, tilemapLayer); // See if touching a tile.
        tilePos = grid.WorldToCell(miningHitBox.transform.position); // Tile position that the hitTileGameObject is closest too.

        if (hit.collider && player.miningButtonPressed) // If hit Tile.
        {
            player.touchingTile = true;

            if (player.canMine == true)
            {
                Vector3Int hitTilePos = grid.WorldToCell(miningHitBox.transform.position); // Tile position that the hitTileGameObject is closest too.
                Instantiate(hitTileGameObject, new Vector2(hitTilePos.x + .5f, tilePos.y + .5f), Quaternion.identity); // Instantiate new Object.
                tilemap.SetTile(hitTilePos, null); // Destroy Tile.


                player.canMine = false;
                player.touchingTile = false;

                // Play SFX
                player.PlaySFX(player.minesfx);
                player.CallKnockback(player.miningDir, Vector2.up, -player.miningDir.x);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(miningHitBox.transform.position, .001f);
    }
}
