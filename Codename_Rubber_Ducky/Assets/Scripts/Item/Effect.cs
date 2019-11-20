using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    public Item.ItemSub item;
    public Item.HotPotatoItem potato;
    public Vector3 center;
    private bool ineffect = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    private IEnumerator OnTriggerEnter2D(Collider2D other)
    {
        if (item != null)
        {
            if (other.CompareTag("Player"))
            {
                if (item.type == (int) Item.ItemSub.Type.Melee)
                {
                    if (other.gameObject != item.ignore)
                    {
                        Debug.Log(other.gameObject + " " + item.player);
                        if (item.GetType() == typeof(Item.Projectile))
                            gameObject.GetComponent<SpriteRenderer>().enabled =
                                !gameObject.GetComponent<SpriteRenderer>().enabled;
                        switch (item.effect)
                        {
                            case (int) Item.ItemSub.Effects.Knockback:
                            {
                                if (item.GetType() == typeof(Item.Projectile)) center = item.main.transform.position;
                                
                                other.GetComponent<PlayerPlatformerController>().startKnockback(center,item.strength);
                                break;
                            }
                            case (int) Item.ItemSub.Effects.Stun:
                            {
                                other.GetComponent<PlayerPlatformerController>().startStun(item.strength);
                                break;
                            }
                            case (int) Item.ItemSub.Effects.Slow:
                            {
                                if (!ineffect)
                                {
                                    ineffect = true;
                                    other.GetComponent<PlayerPlatformerController>().maxSpeed /= 2;
                                    yield return new WaitForSeconds(item.strength);
                                    other.GetComponent<PlayerPlatformerController>().maxSpeed *= 2;
                                    ineffect = false;
                                }

                                break;
                            }
                        }
                    }
                    
                }
                else if (item.GetType() == typeof(Item.HotPotatoItem))
                {
                    
                    potato = (Item.HotPotatoItem)item;
                    
                    GameObject potatoGame = GameObject.Find(other.name + "Potato");
                    ItemPickup itemGame = other.GetComponent<ItemPickup>();
                    
                    ItemPickup ownPickup = transform.parent.parent.parent.parent.parent.GetComponent<ItemPickup>();
                    GameObject ownPotato = GameObject.Find(ownPickup.name + "Potato");
                    
                    itemGame.potato = item;
                    potatoGame.GetComponent<Effect>().item = item;
                    potatoGame.GetComponent<SpriteRenderer>().sprite = item.sprite;
                    
                    ownPotato.GetComponent<SpriteRenderer>().sprite = null;
                    potato.unloadPotato(ownPotato.GetComponent<CircleCollider2D>());                    
                    ownPickup.potato = null;
                    item = null;
                    
                    itemGame.potato.setStrength((int)(itemGame.potato.strength - (Time.time - ownPickup.time)));
                    itemGame.potato.startExplode(potatoGame.GetComponent<CircleCollider2D>());

                    CircleCollider2D circle = transform.GetComponent<CircleCollider2D>();
                    BoxCollider2D box = transform.GetComponent<BoxCollider2D>();
                    
                    if (circle.IsTouching(other))
                    {
                        box.enabled = false;
                        if (!ineffect)
                        {
                            ineffect = true;
                            Vector3 direction = new Vector2(other.bounds.center.x - circle.offset.x,
                                other.bounds.center.y - circle.offset.y);

                            direction = direction.normalized * 2;
                            Vector3 end = other.transform.position + direction;
                            while (other.transform.position != end)
                            {
                                other.transform.position =
                                    Vector3.MoveTowards(other.transform.position, end, 10 * Time.deltaTime);
                                yield return new WaitForEndOfFrame();
                            }

                            ineffect = false;
                        }

                        box.enabled = true;
                    }
                }
            }
            
            if (item.GetType() == typeof(Item.Projectile))
            {
                if(other.CompareTag("Platform") || other.CompareTag("Item") || other.CompareTag("Player")) Destroy(gameObject);
                
            }
        }
    }
}