using System;
using UnityEngine;

namespace HatWorld
{
    public class PlayerHatManager : CreatureHatManager
    {
        // tracks buttonpresses for custom wear-hat button
        bool[] createHatInput = new bool[10];
        bool[] wearHatInput = new bool[10];

        public const int maxPlayerNum = 4;
        public static KeyCode[] createHatKeys = new KeyCode[maxPlayerNum] { KeyCode.T, KeyCode.A, KeyCode.T, KeyCode.T };
        public static KeyCode[] wearHatKeys = new KeyCode[maxPlayerNum] { KeyCode.Y, KeyCode.S, KeyCode.Y, KeyCode.Y };

        public PlayerHatManager(Player wearer) : base(wearer) { }

        public override void AddHooks()
        {
            base.AddHooks();

            // Ensure hat appears when worn
            On.PlayerGraphics.InitiateSprites += PlayerGraphics_InitiateSprites;
            On.PlayerGraphics.DrawSprites += PlayerGraphics_DrawSprites;

            // add new button response to player controls
            On.Player.checkInput += Player_checkInput;
            On.Player.Update += Player_Update;
        }

        /*
         * Adds worn hat to player when player enters new room, if hat is being worn
         */
        private void PlayerGraphics_InitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            orig.Invoke(self, sLeaser, rCam);

            if (self.player == wearer)
            {
                if (self != null && physicalWornHat != null)
                {
                    wornHat = physicalWornHat.GetWornHat(self);
                    self.owner.room.AddObject(wornHat);
                    return;
                }
            }
        }

        /*
         * Draws worn hat if it exists, every update
         */
        private void PlayerGraphics_DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig.Invoke(self, sLeaser, rCam, timeStacker, camPos);

            if (self.player == wearer)
            {
                if (physicalWornHat != null && wornHat != null)
                {
                    wornHat.ParentDrawSprites(sLeaser, rCam, timeStacker, camPos);
                }
            }
        }

        /*
         * Adds custom button check for wearHat to controls (required for good buttonpress response)
         */
        private void Player_checkInput(On.Player.orig_checkInput orig, Player self)
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
                    /* no controller support yet
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
                if ((self.standStillOnMapButton && self.input[0].mp) || self.Sleeping)
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
                if (self.sleepCounter <= -6 && physicalWornHat != null) // (self.sleepCounter <= -6) // self.readyForWin
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
                        Type newHatType = HatWorldMain.hatTypes[(int) (UnityEngine.Random.value * HatWorldMain.hatTypes.Count)];
                        Debug.Log("hatworld new hat generated " + newHatType);
                        // string newHatType = "HatWorld.TorchPhysical";

                        HatAbstract newHat = new HatAbstract(self.room.world, self.abstractCreature.pos, self.room.game.GetNewID(), newHatType);
                        self.room.abstractRoom.AddEntity(newHat);
                        newHat.RealizeInRoom();
                        self.SlugcatGrab(newHat.realizedObject, 0);
                    }

                    // wear hat flag
                    bool wearFlag = self != null && wearHatInput[0] && !wearHatInput[1];
                    if (wearFlag)
                    {
                        if (physicalWornHat == null)
                        {
                            // if holding hat, remove held hat and add wear hat
                            for (int i = 0; i < 2; i++)
                            {
                                if (self.grasps[i] != null && self.grasps[i].grabbed is HatPhysical)
                                {
                                    physicalWornHat = (HatPhysical) self.grasps[i].grabbed;

                                    // add worn hat
                                    wornHat = physicalWornHat.GetWornHat(self.graphicsModule);

                                    // hat effects
                                    wornHat.AddHatEffects(self);

                                    // remove held hat
                                    physicalWornHat.Destroy();
                                    self.room.RemoveObject(physicalWornHat);
                                    self.room.abstractRoom.RemoveEntity(physicalWornHat.abstractPhysicalObject);
                                    self.ReleaseGrasp(i);
                                    break;
                                }
                            }
                        } else {
                            TakeOffHat(self);
                        }
                    }
                }
            }
        }

        public void TakeOffHat(Player self)
        {
            // remove hat effects
            wornHat.RemoveHatEffects(self);

            // remove worn hat
            HatAbstract heldHat = new HatAbstract(self.room.world, self.abstractCreature.pos, self.room.game.GetNewID(), physicalWornHat.GetType());
            physicalWornHat = null;
            wornHat.Destroy();
            self.room.RemoveObject(wornHat);

            // add held hat
            self.room.abstractRoom.AddEntity(heldHat);
            heldHat.RealizeInRoom();
            self.SlugcatGrab(heldHat.realizedObject, 0);
        }
    }
}
