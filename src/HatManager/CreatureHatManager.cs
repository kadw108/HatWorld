using System;
using UnityEngine;

namespace HatWorld.src.HatManager
{
    public abstract class CreatureHatManager
    {
        public static bool effectsOn = true;

        public Creature wearer;

        public HatWearing? wornHat = null; // actually worn hat - destroyed and recreated when the sprite disappears/appears (eg. between rooms)
        public Type? physicalWornHat = null; // physical hat type of currently worn hat, persists between rooms

        public CreatureHatManager(Creature wearer)
        {
            this.wearer = wearer;
        }
        public virtual void AddHooks()
        {
			On.RoomCamera.SpriteLeaser.CleanSpritesAndRemove += SpriteLeaser_CleanSpritesAndRemove;

            // Remove worn hats when wearer dies
            On.Creature.Die += Creature_Die;

            // Remove worn hats when player dies/quits game
            On.RainWorldGame.ExitGame += RainWorldGame_ExitGame;
            On.RainWorldGame.GoToDeathScreen += RainWorldGame_GoToDeathScreen;

            // Ensure hat appears when worn - leaves FancySlugcats integration to PlayerHatManager
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

            if (self.owner == wearer)
            {
                if (self != null && physicalWornHat != null)
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

            if (self.owner == wearer)
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

            if (self == wearer)
            {
                TakeOffHat(self);
            }
        }

        private void RainWorldGame_ExitGame(On.RainWorldGame.orig_ExitGame orig, RainWorldGame self, bool asDeath, bool asQuit)
        {
            wornHat = null;
            physicalWornHat = null;

            orig.Invoke(self, asDeath, asQuit);
        }

        private void RainWorldGame_GoToDeathScreen(On.RainWorldGame.orig_GoToDeathScreen orig, RainWorldGame self)
        {
            wornHat = null;
            physicalWornHat = null;

            orig.Invoke(self);
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
                    self.ReleaseGrasp(i);
                    break;
                }
            }
        }
    
        public virtual HatPhysical TakeOffHat(Creature self)
        {
            if (physicalWornHat != null && wornHat != null)
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
