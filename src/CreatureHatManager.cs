using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HatWorld
{
    public class CreatureHatManager /* INCOMPLETE */
    {
        public static bool effectsOn = true;

        public Creature wearer;

        public HatWearing? wornHat = null; // actually worn hat - destroyed and recreated when the sprite disappears/appears (eg. between rooms)
        public HatPhysical? physicalWornHat = null; // physical object version of currently worn hat, persists between rooms

        public CreatureHatManager(Creature wearer)
        {
            this.wearer = wearer;
        }
        public virtual void AddHooks()
        {
            /*
            // Ensure hat appears when worn
            On.PlayerGraphics.InitiateSprites += PlayerGraphics_InitiateSprites;
            On.PlayerGraphics.DrawSprites += PlayerGraphics_DrawSprites;

            // add new button response to player controls
            On.Player.checkInput += Player_checkInput;
            On.Player.Update += Player_Update;
            */

            // Remove worn hats when player dies/quits game
            On.RainWorldGame.ExitGame += RainWorldGame_ExitGame;
            On.RainWorldGame.GoToDeathScreen += RainWorldGame_GoToDeathScreen;
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
    }
}
