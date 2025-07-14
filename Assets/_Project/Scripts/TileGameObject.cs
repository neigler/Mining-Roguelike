using UnityEngine;

public class TileGameObject : MonoBehaviour
{

    Player player;
    [SerializeField] LayerMask gameObjectLayer;
    [SerializeField] int hpLeft = 4;

    public Sprite HpLeft1;
    public Sprite HpLeft2;
    public Sprite HpLeft3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player GameObject").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit = Physics2D.CircleCast(player.miningHitBox.transform.position, .005f, Vector2.zero, .005f, gameObjectLayer); // See if touching a tile.

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.transform.position == this.gameObject.transform.position && player.miningButtonPressed)
            {
                player.touchingTile = true;

                if (player.canMine == true)
                {
                    player.targetTime = player.miningCooldown;
                    this.gameObject.GetComponent<TileGameObject>().hpLeft -= 1;

                    player.touchingTile = false;
                    player.canMine = false;

                    //Play SFX
                    player.PlaySFX(player.minesfx);
                    player.CallKnockback(player.miningDir, Vector2.up, -player.miningDir.x);
                }
            }
            else if (hit.collider.gameObject != this.gameObject)
            {
                return;
            }
        }

        if (hpLeft == 0)
        {
            player.PlaySFX(player.breaksfx);
            Destroy(this.gameObject);
        }

        //Sprites
        SpriteManager();
    }

    private void SpriteManager()
    {
        if (this.gameObject.GetComponent<TileGameObject>().hpLeft == 3)
            this.gameObject.GetComponent<SpriteRenderer>().sprite = HpLeft3;
        if (this.gameObject.GetComponent<TileGameObject>().hpLeft == 2)
            this.gameObject.GetComponent<SpriteRenderer>().sprite = HpLeft2;
        if (this.gameObject.GetComponent<TileGameObject>().hpLeft == 1)
            this.gameObject.GetComponent<SpriteRenderer>().sprite = HpLeft1;
    }
}
