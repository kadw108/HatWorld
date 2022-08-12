using System;
using System.Collections.Generic;
using UnityEngine;

namespace HatWorld.src.HatManager
{
    public abstract class CreatureHatManager
    {
        public static Dictionary<EntityID, CreatureHatManager> HatManagers = new();
        public static bool effectsOn = true;

        public EntityID wearer;

        public HatWearing? wornHat; // actually worn hat - destroyed and recreated when the sprite disappears/appears (eg. between rooms)
        public Type? physicalWornHat; // physical hat type of currently worn hat, persists between rooms

        public static void CreateHatManager(Creature crea, EntityID wearer)
        {
            if (!HatManagers.ContainsKey(wearer))
            {
                if (crea is Scavenger)
                {
                    HatManagers[wearer] = new ScavHatManager(wearer);
                    HatManagers[wearer].AddHooks();
                }

                if (crea is Player)
                {
                    HatManagers[wearer] = new PlayerHatManager(wearer);
                    HatManagers[wearer].AddHooks();
                }

                Debug.Log("hatworld new hat manager" + wearer);
            }
        }

        public CreatureHatManager(EntityID wearer)
        {
            this.wearer = wearer;
            this.wornHat = null;
            this.physicalWornHat = null;
        }
        public virtual void AddHooks()
        {
			On.RoomCamera.SpriteLeaser.CleanSpritesAndRemove += SpriteLeaser_CleanSpritesAndRemove;

            // Remove worn hats when wearer dies
            On.Creature.Die += Creature_Die;

            // Remove all worn hats when game session ends (player dies/quits game/sleeps successfully)
            On.RainWorldGame.ShutDownProcess += RainWorldGame_ShutDownProcess;

            // Ensure hat appears when worn - leave FancySlugcats integration to PlayerHatManager
            if (!(this is PlayerHatManager) || HatWorldMain.fancyGraphicsRef == null)
            {
                On.GraphicsModule.InitiateSprites += GraphicsModule_InitiateSprites;
                On.GraphicsModule.DrawSprites += GraphicsModule_DrawSprites;
            }
        }

        /*
         * Adds worn hat to player when player enters new room, if hat is being worn
         */
        private void GraphicsModule_InitiateSprites(On.GraphicsModule.orig_InitiateSprites orig, GraphicsModule self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            orig.Invoke(self, sLeaser, rCam);

            if ((self.owner as Creature).abstractCreature.ID == wearer)
            {
                Debug.Log("hatworld " + wearer + "check initiate hat " + (physicalWornHat != null));
                if (physicalWornHat != null)
                {
                    wornHat = (HatWearing) physicalWornHat.GetMethod("GetWornHat").Invoke(null, new object[] { self });
                    self.owner.room.AddObject(wornHat);
                    return;
                }
            }
        }

        /*
         * Draws worn hat if it exists, every update
         */
        private void GraphicsModule_DrawSprites(On.GraphicsModule.orig_DrawSprites orig, GraphicsModule self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig.Invoke(self, sLeaser, rCam, timeStacker, camPos);

            if ((self.owner as Creature).abstractCreature.ID == wearer)
            {
                if (physicalWornHat != null && wornHat != null)
                {
                    wornHat.ParentDrawSprites(sLeaser, rCam, timeStacker, camPos);
                }
            }
        }

		// Remove hats when a creature's sprites are removed
		private void SpriteLeaser_CleanSpritesAndRemove(On.RoomCamera.SpriteLeaser.orig_CleanSpritesAndRemove orig, RoomCamera.SpriteLeaser self)
		{
			orig(self);

			if (wornHat != null && wornHat.parent == self.drawableObject)
			{
				wornHat.Destroy();
			}
		}

        public void Creature_Die(On.Creature.orig_Die orig, Creature self)
        {
            orig(self);

            if (self.abstractCreature.ID == wearer && (self is Player)) // dying takes off hat unless you are player
            {
                TakeOffHat(self);
            }
        }

        private void RainWorldGame_ShutDownProcess(On.RainWorldGame.orig_ShutDownProcess orig, RainWorldGame self)
        {
            wornHat = null;
            physicalWornHat = null;

            orig(self);
        }

        public virtual void PutOnHat(Creature self)
        {
            // if holding hat, remove held hat and add wear hat
            for (int i = 0; i < self.grasps.Length; i++)
            {
                if (self.grasps[i] != null && self.grasps[i].grabbed is HatPhysical)
                {
                    HatPhysical physicalHat = (HatPhysical)self.grasps[i].grabbed;
                    physicalWornHat = physicalHat.GetType();

                    // add worn hat
                    wornHat = (HatWearing) physicalWornHat.GetMethod("GetWornHat").Invoke(null, new object[] { self.graphicsModule });

                    // hat effects
                    if (effectsOn)
                    {
                        wornHat.AddHatEffects(self);
                    }

                    // remove held hat
                    physicalHat.Destroy();
                    self.room.RemoveObject(physicalHat);
                    self.room.abstractRoom.RemoveEntity(physicalHat.abstractPhysicalObject);
                    self.room.abstractRoom.RemoveEntity(physicalHat.abstractPhysicalObject.ID);
                    self.ReleaseGrasp(i);
                    break;
                }
            }
        }
    
        public virtual HatPhysical TakeOffHat(Creature self)
        {
            if (physicalWornHat != null && wornHat != null && self.room != null)
            {
                // remove hat effects
                if (effectsOn)
                {
                    wornHat.RemoveHatEffects(self);
                }

                // remove worn hat
                HatAbstract heldHat = new HatAbstract(self.room.world, self.abstractCreature.pos, self.room.game.GetNewID(), physicalWornHat);
                physicalWornHat = null;
                wornHat.Destroy();
                self.room.RemoveObject(wornHat);

                // add held hat
                self.room.abstractRoom.AddEntity(heldHat);
                heldHat.RealizeInRoom();
                return (HatPhysical) heldHat.realizedObject;
            }
            return null;
        }
    }
}
