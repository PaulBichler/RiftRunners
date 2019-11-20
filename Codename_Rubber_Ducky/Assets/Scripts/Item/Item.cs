using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Item : MonoBehaviour
{
    public abstract class ItemSub
    {
        public string Name;
        public int Uses;
        public int type;
        public int effect;
        public int strength;
        public int amount;
        public int flipped;

        public Vector2 offset = new Vector2(0,0);

        public GameObject player;
        public ItemPickup pickup;
        public GameObject ignore;

        public enum Type
        {
            Melee,
            Ranged,
            Special
        }

        public enum Effects
        {
            Knockback,
            Slow,
            Stun
        }


        public void setStrength(int strength)
        {
            this.strength = strength;
        }

        public readonly GameObject main = new GameObject();
        public Sprite sprite;

        protected void init()
        {
            main.tag = "Item";
            main.name = Name + "ItemObject";

            var spriteRenderer = main.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
            spriteRenderer.sprite = sprite;
            Vector2 s = spriteRenderer.sprite.bounds.size;

            

            var trigger = main.AddComponent(typeof(BoxCollider2D)) as BoxCollider2D;
            trigger.isTrigger = true;
            //trigger.size = s;
            //trigger.offset = new Vector2(s.x/2,0);

            var collision = main.AddComponent(typeof(BoxCollider2D)) as BoxCollider2D;
            //collision.size = s;
            //collision.offset = new Vector2(s.x/2,0);

            main.AddComponent(typeof(Rigidbody2D));

            var script = main.AddComponent(typeof(ItemScript)) as ItemScript;
            script.item = this;
        }

        protected ItemSub(int Uses, Vector2 velocity, Vector2 position)
        {
            this.Uses = Uses;

            main.transform.position = position;
        }

        public abstract void action(GameObject game);

        public abstract void startExplode(CircleCollider2D circle);
    }

    public class RangedItemSub : ItemSub
    {
        private bool shot = false;
        private static MonoBehaviour monoBe;

        private bool able = true;

        protected RangedItemSub(int Uses, Vector2 velocity, Vector2 position) : base(Uses, velocity, position)
        {
            type = (int) Type.Ranged;
        }

        public static void parseMono(MonoBehaviour mono)
        {
            monoBe = mono;
        }

        private void shoot(GameObject game)
        {
            if (!shot)
            {
                shot = true;
                Vector2 direction = new Vector2(1, 0);
                for (int i = 0; i < amount; i++)
                {
                    if (i == 1) direction = new Vector2(1, 1);
                    if (i == 2) direction = new Vector2(1, -1);

                    Vector3 speed = new Vector3(20, 0);
                    Vector3 additional;
                    if (flipped == 1) additional = new Vector2(0.5f, 0);
                    else additional = new Vector2(-.5f, 0);
                    new Projectile(effect, strength, game.GetComponent<Transform>().position + additional,
                        flipped * direction * speed, new Vector2());
                }
            }
        }

        public override void action(GameObject game)
        {
            Debug.Log(Uses);
            if (Uses > 0)
            {
                shoot(game);
                if (able) monoBe.StartCoroutine(reload());
                
                if (Uses <= 0) pickup.removeItem();
            }

            
        }

        private IEnumerator reload()
        {
            Debug.Log("Reload");
            if (able)
            {
                able = false;
                Uses--;
            }

            yield return new WaitForSecondsRealtime(1);
            shot = false;
            able = true;
            Debug.Log("Reload finished" + shot);
        }

        public override void startExplode(CircleCollider2D circle)
        {
            throw new System.NotImplementedException();
        }
    }

    public class Projectile : MeleeItemSub
    {
        public Projectile(int effect, int strength, Vector2 start, Vector2 direction, Vector2 velo, int Uses = 1) :
            base(velo, start, Uses)
        {
            this.effect = effect;
            this.strength = strength;
            Name = "Projectile";
            sprite = Resources.Load<Sprite>("Sprites/Projectile");
            init();
            if (flipped == -1)
            {
                Debug.Log("Help");
                main.GetComponent<SpriteRenderer>().flipY = true;
            }
            var eff = main.AddComponent(typeof(Effect)) as Effect;
            eff.item = this;
            eff.center = main.GetComponent<SpriteRenderer>().sprite.bounds.center;
            main.GetComponent<Transform>().position = start;
            main.GetComponent<Rigidbody2D>().velocity = direction;
            main.GetComponent<Rigidbody2D>().gravityScale = 0f;
            if (flipped == -1) main.GetComponent<SpriteRenderer>().flipY = true;
            main.tag = "Untagged";
        }
    }

    public class KnockBackRanged : RangedItemSub
    {
        protected KnockBackRanged(int Uses, Vector2 velocity, Vector2 position) : base(Uses, velocity, position)
        {
            effect = (int) Effects.Knockback;
        }
    }

    public class PistolItem : KnockBackRanged
    {
        public PistolItem(Vector2 velocity, Vector2 position, int Uses = 10) : base(Uses, velocity, position)
        {
            Name = "Pistol";
            sprite = Resources.Load<Sprite>("Sprites/Pistol");
            strength = 3;
            amount = 1;
            init();
            offset = new Vector2(0.3f,0);
            main.GetComponent<Rigidbody2D>().velocity = velocity;
        }
    }

    public class ShotgunItem : KnockBackRanged
    {
        public ShotgunItem(Vector2 velocity, Vector2 position, int Uses = 3) : base(Uses, velocity, position)
        {
            Name = "Shotgun";
            sprite = Resources.Load<Sprite>("Sprites/Shotgun");
            strength = 3;
            amount = 3;
            init();
            offset = new Vector2(0.7f,0.2f);
            main.GetComponent<Rigidbody2D>().velocity = velocity;
        }
    }

    public class SniperItem : KnockBackRanged
    {
        public SniperItem(Vector2 velocity, Vector2 position, int Uses = 1) : base(Uses, velocity, position)
        {
            Name = "Sniper";
            sprite = Resources.Load<Sprite>("Sprites/Sniper");
            strength = 10;
            amount = 1;
            init();
            offset = new Vector2(1,0.1f);
            main.GetComponent<Rigidbody2D>().velocity = velocity;
        }
    }

    public class MinigunItem : KnockBackRanged
    {
        public MinigunItem(Vector2 velocity, Vector2 position, int Uses = 15) : base(Uses, velocity, position)
        {
            Name = "Minigun";
            sprite = Resources.Load<Sprite>("Sprites/Minigun");
            strength = 1;
            amount = 1;
            init();
            offset = new Vector2(1.2f,0.2f);
            main.GetComponent<Rigidbody2D>().velocity = velocity;
        }
    }

    public class SlowRanged : RangedItemSub
    {
        protected SlowRanged(Vector2 velocity, Vector2 position, int Uses) : base(Uses, velocity, position)
        {
            effect = (int) Effects.Slow;
        }
    }

    public class FreezingPistolItem : SlowRanged
    {
        public FreezingPistolItem(Vector2 velocity, Vector2 position, int Uses = 6) : base(velocity, position, Uses)
        {
            Name = "Freezing Pistol";
            sprite = Resources.Load<Sprite>("Sprites/freezingPistol");
            strength = 1;
            amount = 1;
            init();
            offset = new Vector2(0.1f,0.2f);
            main.GetComponent<Rigidbody2D>().velocity = velocity;
        }
    }

    public class MeleeItemSub : ItemSub
    {
        private static MonoBehaviour monoBe;

        protected MeleeItemSub(Vector2 velocity, Vector2 position, int Uses) : base(Uses, velocity, position)
        {
            type = (int) Type.Melee;
        }

        private bool ineffect = false;
        private bool first = true;
        private bool able = true;

        private IEnumerator swing(float z)
        {
            Debug.Log("Start In Swing" + z);
            float calcValue;
            if (z <= 0)
            {
                calcValue = 1;

                float end = z;

                while (Mathf.Round(player.transform.eulerAngles.z) > end)
                {
                    player.transform.Rotate(Vector3.forward * calcValue);
                    yield return new WaitForEndOfFrame();
                }

                player.transform.Rotate(Vector3.forward * -calcValue);

                if (Uses == 0) pickup.removeItem();
                ineffect = false;
            }
            else
            {
                float end = 360 - z;
                calcValue = -1;
                if (flipped == -1)
                {
                    player.transform.Rotate(Vector3.forward * (2 * calcValue));


                    while (player.transform.eulerAngles.z < z)
                    {
                        player.transform.Rotate(Vector3.forward * calcValue);
                        yield return new WaitForEndOfFrame();
                    }
                }
                else
                {
                    while (player.transform.eulerAngles.z > end)
                    {
                        player.transform.Rotate(Vector3.forward * calcValue);
                        yield return new WaitForEndOfFrame();
                    }
                }

                monoBe.StartCoroutine(swing(0));
            }
        }

        public static void parseMono(MonoBehaviour mono)
        {
            monoBe = mono;
        }


        public override void action(GameObject game)
        {
            if (Uses > 0 && !ineffect)
            {
                if (!ineffect)
                {
                    Debug.Log(Uses);
                    ineffect = true;
                    monoBe.StartCoroutine(swing(120));
                    Uses--;
                }
            }
        }

        public override void startExplode(CircleCollider2D circle)
        {
            throw new System.NotImplementedException();
        }
    }

    public class KnockBackMelee : MeleeItemSub
    {
        protected KnockBackMelee(int Uses, Vector2 position, Vector2 velocity) : base(velocity, position, Uses)
        {
            effect = (int) Effects.Knockback;
        }
    }

    public class SwordItem : KnockBackMelee
    {
        public SwordItem(Vector2 velocity, Vector2 position, int Uses = 5) : base(Uses, position, velocity)
        {
            Name = "Sword";
            sprite = Resources.Load<Sprite>("Sprites/Sword");
            strength = 3;
            init();
            main.GetComponent<Rigidbody2D>().velocity = velocity;
            offset = new Vector2(0, 0.55f);
        }
    }

    public class BoostItem : KnockBackMelee
    {
        public BoostItem(Vector2 velocity, Vector2 position, int Uses = 2) : base(Uses, position, velocity)
        {
            Name = "Boost";
            sprite = Resources.Load<Sprite>("Sprites/Boost");
            strength = 3;
            init();
            main.GetComponent<Rigidbody2D>().velocity = velocity;
        }
    }

    public class GreatSwordItem : KnockBackMelee
    {
        public GreatSwordItem(Vector2 velocity, Vector2 position, int Uses = 3) : base(Uses, position, velocity)
        {
            Name = "Great Sword";
            sprite = Resources.Load<Sprite>("Sprites/GreatSword");
            strength = 5;
            init();
            offset = new Vector2(0f,1f);
            main.GetComponent<Rigidbody2D>().velocity = velocity;
        }
    }

    public class StunMelee : MeleeItemSub
    {
        protected StunMelee(Vector2 velocity, Vector2 position, int Uses) : base(velocity, position, Uses)
        {
            effect = (int) Effects.Stun;
        }
    }

    public class HammerItem : StunMelee
    {
        public HammerItem(Vector2 velocity, Vector2 position, int Uses = 3) : base(velocity, position, Uses)
        {
            Name = "Hammer";
            sprite = Resources.Load<Sprite>("Sprites/Hammer");
            strength = 2;
            init();
            offset = new Vector2(0.1f,1);
            main.GetComponent<Rigidbody2D>().velocity = velocity;
        }
    }

    public class SlowMelee : MeleeItemSub
    {
        protected SlowMelee(int Uses, Vector2 position, Vector2 velocity) : base(velocity, position, Uses)
        {
            effect = (int) Effects.Slow;
        }
    }

    public class SlowTemplate : SlowMelee
    {
        public SlowTemplate(int Uses, Vector2 position, Vector2 velocity) : base(Uses, position, velocity)
        {
            Name = "SlowTemplate";
            sprite = Resources.Load<Sprite>("Sprites/Slow");
            strength = 2;
            init();
            main.GetComponent<Rigidbody2D>().velocity = velocity;
        }
    }

    public class SpecialItemSub : ItemSub
    {
        public SpecialItemSub(int Uses, Vector2 position, Vector2 velocity) : base(Uses, position, velocity)
        {
            type = (int) Type.Special;
        }

        public override void action(GameObject game)
        {
        }

        public override void startExplode(CircleCollider2D circle)
        {
            throw new System.NotImplementedException();
        }

        protected void teleport(GameObject first, GameObject second)
        {
            Vector3 temp = first.transform.position;
            first.transform.position = second.transform.position;
            second.transform.position = temp;
        }
    }

    public class TeleportItem : SpecialItemSub
    {
        private GameObject[] all;

        public TeleportItem(Vector2 velocity, Vector2 position, int Uses = 1) : base(Uses, position, velocity)
        {
            Name = "Teleporter";
            sprite = Resources.Load<Sprite>("Sprites/Teleport");
            strength = 0;
            init();
            main.GetComponent<Rigidbody2D>().velocity = velocity;
        }

        public override void action(GameObject game)
        {
            if (Uses > 0)
            {
                bool found = false;
                GameObject first = game.transform.parent.parent.parent.gameObject;
                Debug.Log("First:" + first);
                GameObject second = null;
                all = GameObject.FindGameObjectsWithTag("Player");
                int num = Random.Range(0, 3);
                while (!found)
                {
                    if (all[num] != first)
                    {
                        second = all[num];
                        found = true;
                    }
                    else
                    {
                        num = Random.Range(0, 3);
                    }
                }

                Debug.Log("Second:" + second);
                teleport(first, second);
                Uses--;
                
                pickup.removeItem();
            }
        }
    }

    public class MassTeleportItem : SpecialItemSub
    {
        private GameObject tempGO;
        private GameObject[] all;

        public MassTeleportItem(Vector2 velocity, Vector2 position, int Uses = 1) : base(Uses, position, velocity)
        {
            Name = "MassTeleporter";
            sprite = Resources.Load<Sprite>("Sprites/MassTeleport");
            strength = 0;
            init();
            main.GetComponent<Rigidbody2D>().velocity = velocity;
            all = GameObject.FindGameObjectsWithTag("Player");
            Shuffle();
        }

        private void Shuffle()
        {
            for (int i = 0; i < all.Length; i++)
            {
                int rnd = Random.Range(0, all.Length);
                tempGO = all[rnd];
                all[rnd] = all[i];
                all[i] = tempGO;
            }
        }

        public override void action(GameObject game)
        {
            if (Uses > 0)
            {
                if (all.Length == 2) teleport(all[0], all[1]);
                else if (all.Length == 3)
                {
                    teleport(all[2], all[0]);
                    teleport(all[0], all[1]);
                }
                else if (all.Length == 4)
                {
                    teleport(all[0], all[1]);
                    teleport(all[2], all[3]);
                }

                Uses--;
                pickup.removeItem();
            }
        }
    }

    public class SlowMotionItem : SpecialItemSub
    {
        static MonoBehaviour monoBe;

        public SlowMotionItem(Vector2 velocity, Vector2 position, int Uses = 1) : base(Uses, position, velocity)
        {
            Name = "SlowMotion";
            sprite = Resources.Load<Sprite>("Sprites/Slow");
            strength = 2;
            init();
            main.GetComponent<Rigidbody2D>().velocity = velocity;
        }

        public override void action(GameObject game)
        {
            if (Uses > 0)
            {
                monoBe.StartCoroutine(slowTime());
                Uses--;
                pickup.removeItem();
            }
        }

        public static void parseMono(MonoBehaviour mono)
        {
            monoBe = mono;
        }

        public IEnumerator slowTime()
        {
            Time.timeScale = .25f;
            yield return new WaitForSecondsRealtime(strength);
            Time.timeScale = 1f;
        }
    }

    public class HotPotatoItem : SpecialItemSub
    {
        private static MonoBehaviour MonoBe;

        public HotPotatoItem(Vector2 velocity, Vector2 position, int Uses = 0) : base(Uses, position, velocity)
        {
            Name = "HotPotato";
            sprite = Resources.Load<Sprite>("Sprites/HotPotato");
            strength = 3;
            init();
            main.GetComponent<Rigidbody2D>().velocity = velocity;
        }


        public override void startExplode(CircleCollider2D circle)
        {
            Debug.Log("Start Co");
            MonoBe.StartCoroutine(explode(circle));
        }

        public IEnumerator explode(CircleCollider2D circle)
        {
            Debug.Log("Explode start wait for" + strength);

            yield return new WaitForSecondsRealtime(strength);

            Debug.Log("BAAMMMM");
            circle.enabled = true;
            Debug.Log("Enabled");
            yield return new WaitForSecondsRealtime(2);
            circle.enabled = false;
            Debug.Log("Disabled");
        }

        public void unloadPotato(CircleCollider2D circle)
        {
            MonoBe.StopCoroutine(explode(circle));
            Debug.Log("Stopped coroutine");
        }

        public static void parseMono(MonoBehaviour mono)
        {
            MonoBe = mono;
        }
    }

    public class ControlSwitchItem : SpecialItemSub
    {
        private static MonoBehaviour monoBe;
        private GameObject[] all;

        public ControlSwitchItem(Vector2 velocity, Vector2 position, int Uses = 1) : base(Uses, position, velocity)
        {
            Name = "ControlSwitch";
            sprite = Resources.Load<Sprite>("Sprites/ControlSwitch");
            strength = 3;
            init();
            main.GetComponent<Rigidbody2D>().velocity = velocity;
            all = GameObject.FindGameObjectsWithTag("Player");
        }

        public static void parseMono(MonoBehaviour mono)
        {
            monoBe = mono;
        }

        public override void action(GameObject game)
        {
            if (Uses > 0)
            {
                monoBe.StartCoroutine(switchControls());
                Uses--;
                pickup.removeItem();
            }
        }

        private IEnumerator switchControls()
        {
            foreach (GameObject player in all)
            {
                player.GetComponent<PlayerPlatformerController>().switched = -1;
            }

            yield return new WaitForSecondsRealtime(strength);

            foreach (GameObject player in all)
            {
                player.GetComponent<PlayerPlatformerController>().switched = 1;
            }
        }
    }
}