using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using static Item.ItemSub;
using UnityEngine;
using Type = System.Type;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class ItemPickup : MonoBehaviour
{
    public Item.ItemSub item;
    public Item.ItemSub potato;
    public string name;

    private GameObject player;
    private Effect eff;
    
    public GameObject game;
    public float time;

    private void Start()
    {        
        Item.SlowMotionItem.parseMono(this);
        Item.HotPotatoItem.parseMono(this);
        Item.ControlSwitchItem.parseMono(this);
        Item.RangedItemSub.parseMono(this);
        Item.MeleeItemSub.parseMono(this);
    }

    private void Update()
    {        
        if(item != null) item.flipped = player.GetComponent<PlayerPlatformerController>().flipped; 
        
        if(item != null) eff.center = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item") && item == null)
        {
            
            if(collision.gameObject.GetComponent<ItemScript>().item.GetType() == typeof(Item.HotPotatoItem))
            {                
                string temp = name + "Potato";
                game = GameObject.Find(temp);
                potato = collision.gameObject.GetComponent<ItemScript>().item;
                game.GetComponent<Effect>().item = potato;
                game.GetComponent<SpriteRenderer>().sprite = collision.gameObject.GetComponent<SpriteRenderer>().sprite;
                potato = (Item.HotPotatoItem) potato;
                time = Time.time;                
                potato.startExplode(game.GetComponent<CircleCollider2D>());
                game.GetComponent<Effect>().center =
                    transform.position;        
            
                Destroy(collision.gameObject);
            }
            else
            {
                string temp = name + "ItemObject";
                
                game = GameObject.Find(temp);
                item = collision.gameObject.GetComponent<ItemScript>().item;
                eff = game.GetComponent<Effect>();
                eff.item = item;
                game.GetComponent<SpriteRenderer>().sprite = collision.gameObject.GetComponent<SpriteRenderer>().sprite;
                //game.transform.localPosition += (UnityEngine.Vector3)pivot.pivot - game.transform.localPosition;
                game.GetComponent<BoxCollider2D>().size = GameObject.Find(temp)
                    .GetComponent<SpriteRenderer>().sprite.bounds.size;

                game.GetComponent<BoxCollider2D>().offset = item.offset;
                eff.center = transform.position;
                item.player = game;
                item.pickup = this;
                
                Destroy(collision.gameObject);
                player = transform.gameObject;
                item.ignore = player;
                //updates Item Icon in the UI Widget of the player
                player.GetComponent<PlayerPlatformerController>().updateUI();
                item.flipped = player.GetComponent<PlayerPlatformerController>().flipped;
                
            }
          
            
        }

    }

    public void ThrowAway()
    {
        if (item != null)
        {
            int flipped = item.flipped;
            Vector2 velo;
            Vector2 pos = game.transform.position;
            float dist = 1.5f*item.sprite.bounds.size.x;
            if (flipped == 1)
            {
                velo = new Vector2(8, 3);                
                pos += new Vector2(dist,0);
            }
            else
            {
                velo = new Vector2(-8, 3);
                pos += new Vector2(-dist,0);
            }
            
            makeItem(item.Name,item.Uses,pos,velo);
            removeItem();
        }
        
    }

    public void removeItem()
    {
        game.GetComponent<SpriteRenderer>().sprite = null;
        item = null;
        game.GetComponent<Effect>().item = null;
        game.GetComponent<BoxCollider2D>().size = new Vector3(1,1,1);

        //updates Item Icon in the UI Widget of the player
        player.GetComponent<PlayerPlatformerController>().updateUI();
    }

    public void makeItem(string itemName,int itemUses,Vector2 position,Vector2 velocity)
    {
        if (itemUses == -1)
        {
            switch (itemName)
            {
                case "Sword":
                {
                    new Item.SwordItem(velocity,position);
                    break;
                }
                case "Hammer":
                {
                    new Item.HammerItem(velocity,position);
                    break;
                }
                case "Pistol":
                {
                    new Item.PistolItem(velocity,position);
                    break;
                }
                case "Shotgun":
                {
                    new Item.ShotgunItem(velocity,position);
                    break;
                }
                case "Sniper":
                {
                    new Item.SniperItem(velocity,position);
                    break;
                }
                case "Freezing Pistol":
                {
                    new Item.FreezingPistolItem(velocity,position);
                    break;
                }
                case "Boost" :
                {
                    new Item.BoostItem(velocity,position);
                    break;
                }
                case "Great Sword" :
                {
                    new Item.GreatSwordItem(velocity,position);
                    break;
                }
                case "Minigun" :
                {
                    new Item.MinigunItem(velocity,position);
                    break;
                }
                case "Teleporter":
                {
                    new Item.TeleportItem(velocity,position);
                    break;
                }
                case "MassTeleporter":
                {
                    new Item.MassTeleportItem(velocity,position);
                    break;
                }
                case "SlowMotion":
                {
                    new Item.SlowMotionItem(velocity,position);
                    break;
                }
                case "HotPotato":
                {
                    new Item.HotPotatoItem(velocity,position);
                    break;
                }
                case "ControlSwitch":
                {
                    new Item.ControlSwitchItem(velocity,position);
                    break;
                }
                default:
                {
                    Debug.LogError(itemName + " not found");
                    break;
                }
            }
        }
        else
        {
            switch (itemName)
            {
                case "Sword":
                {
                    new Item.SwordItem(velocity,position,itemUses);
                    break;
                }
                case "Hammer":
                {
                    new Item.HammerItem(velocity,position,itemUses);
                    break;
                }
                case "Pistol":
                {
                    new Item.PistolItem(velocity,position,itemUses);
                    break;
                }
                case "Shotgun":
                {
                    new Item.ShotgunItem(velocity,position,itemUses);
                    break;
                }
                case "Sniper":
                {
                    new Item.SniperItem(velocity,position,itemUses);
                    break;
                }
                case "Freezing Pistol":
                {
                    new Item.FreezingPistolItem(velocity,position,itemUses);
                    break;
                }
                case "Boost" :
                {
                    new Item.BoostItem(velocity,position,itemUses);
                    break;
                }
                case "Great Sword" :
                {
                    new Item.GreatSwordItem(velocity,position,itemUses);
                    break;
                }
                case "Minigun" :
                {
                    new Item.MinigunItem(velocity,position,itemUses);
                    break;
                }
                case "Teleporter":
                {
                    new Item.TeleportItem(velocity,position,itemUses);
                    break;
                }
                case "MassTeleporter":
                {
                    new Item.MassTeleportItem(velocity,position,itemUses);
                    break;
                }
                case "SlowMotion":
                {
                    new Item.SlowMotionItem(velocity,position,itemUses);
                    break;
                }
                case "HotPotato":
                {
                    new Item.HotPotatoItem(velocity,position,itemUses);
                    break;
                }
                case "ControlSwitch":
                {
                    new Item.ControlSwitchItem(velocity,position,itemUses);
                    break;
                }
                default:
                {
                    Debug.LogError(itemName + " not found");
                    break;
                }
            }
        }
        
    }
}

    
