using System;
using UnityEngine;

namespace HatWorld.src.HatManager
{
    public class PlayerHatManager : CreatureHatManager
    {
        // tracks buttonpresses for custom wear-hat button
        bool[] createHatInput = new bool[10];
        bool[] wearHatInput = new bool[10];

        public const int maxPlayerNum = 4;
        public static KeyCode[] createHatKeys = new KeyCode[maxPlayerNum] { KeyCode.A, KeyCode.A, KeyCode.A, KeyCode.A };
        public static KeyCode[] wearHatKeys = new KeyCode[maxPlayerNum] { KeyCode.S, KeyCode.S, KeyCode.S, KeyCode.S };

        public PlayerHatManager(Player wearer) : base(wearer) { }

        public override void AddHooks()
        {
            base.AddHooks();

            // add new button response to player controls
            On.Player.checkInput += Player_CheckInput;
            On.Player.Update += Player_Update;
        }

        /*
         * Adds custom button check for wearHat to controls (required for good buttonpress response)
         */
        private void Player_CheckInput(On.Player.orig_checkInput orig, Player self)
        {
            orig(self);

            if (self == wearer)
            {
                // move inputs back
                for (int i = self.input.Length - 1; i > 0; i--)
                {
                    createHatInput[i] = createHatInput[i - 1];
                    wearHatInput[i] = wearHatInput[i - 1];
                }

                // get new inputs
                if (self.stun == 0 && !self.dead)
                {
                    /* controller support? configmachine
                    if (self.controller != null)
                    {
                        this.input[0] = this.controller.GetInput();
                    }
                    else
                    */
                    {
                        createHatInput[0] = Input.GetKeyDown(createHatKeys[self.playerState.playerNumber]);
                        wearHatInput[0] = Input.GetKeyDown(wearHatKeys[self.playerState.playerNumber]);
                    }
                }
                else
                {
                    createHatInput[0] = false;
                    wearHatInput[0] = false;
                }

                // adjust input if map is open / is sleeping
                if (self.standStillOnMapButton && self.input[0].mp || self.Sleeping)
                {
                    createHatInput[0] = false;
                    wearHatInput[0] = false;
                }
            }
        }

        /*
         * Controls for creating + wearing hat
         */
        private void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig.Invoke(self, eu);

            if (self == wearer)
            {
                // Remove hats when player sleeps in shelter, so hats can be saved as shelter items
                if (self.sleepCounter <= -6 && physicalWornHat != null) 
                {
                    TakeOffHat(self);
                }
                else
                {
                    // create hat flag
                    bool hatFlag = self != null && createHatInput[0] && !createHatInput[1];
                    if (hatFlag)
                    {
                        // generate random hat type out of all existing hat types
                        Type newHatType = HatWorldMain.hatTypes[(int)(UnityEngine.Random.value * HatWorldMain.hatTypes.Count)];
                        Debug.Log("hatworld new hat generated " + newHatType);
                        // string newHatType = "HatWorld.FountainPhysical";

                        HatAbstract newHat = new HatAbstract(self.room.world, self.abstractCreature.pos, self.room.game.GetNewID(), newHatType);
                        self.room.abstractRoom.AddEntity(newHat);
                        newHat.RealizeInRoom();

                        self.SlugcatGrab(newHat.realizedObject, self.FreeHand());
                    }

                    // wear hat flag
                    bool wearFlag = self != null && wearHatInput[0] && !wearHatInput[1];
                    if (wearFlag)
                    {
                        if (physicalWornHat == null)
                        {
                            PutOnHat(self);
                        }
                        else
                        {
                            HatPhysical heldHat = TakeOffHat(self);
                            self.SlugcatGrab(heldHat, self.FreeHand());
                        }
                    }
                }
            }
        }
    }
}
