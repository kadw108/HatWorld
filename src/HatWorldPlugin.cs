// default imports
using System;
using BepInEx;
using UnityEngine;

// physical objects library
using Fisobs;

namespace HatWorld
{
    public enum HatType
    {
        Santa,
        Wizard,
        Torch,
        Flower
    }

    [BepInPlugin("kadw.hatworld", "HatWorld", "1.0.0")]
    public class HatWorldPlugin : BaseUnityPlugin
    {
        // for spawning random hats
        public static System.Random rand = new System.Random();

        // tracks if player is wearing hat. null if no hat
        public HatType? playerWearingHat = null;
        private WearingHat? wornHat = null;

        // tracks buttonpresses for custom wear-hat button
        bool[] createHatInput = new bool[10];
        bool[] wearHatInput = new bool[10];
        KeyCode createHatKey = KeyCode.T;
        KeyCode wearHatKey = KeyCode.Y;

        public void OnEnable()
        {
            // Ensure hat appears when worn
            On.PlayerGraphics.InitiateSprites += PlayerGraphics_InitiateSprites;
            On.PlayerGraphics.DrawSprites += PlayerGraphics_DrawSprites;

            // Make hat exist
            Fisobs.Core.Content.Register(new HatFisob());

            // add new button response to player controls
            On.Player.checkInput += Player_checkInput;
            On.Player.Update += Player_Update;

            // Put hats in their respective rooms where they can be found
            HatPlacer.OnEnable();
        }

        /* ---------- Player worn hat methods ------------ */

        /*
         * Adds worn hat to player when player enters new room, if hat is being worn
         */
        private void PlayerGraphics_InitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            orig.Invoke(self, sLeaser, rCam);

            if (self != null && playerWearingHat != null)
            {
                wornHat = addWornHat((HatType) playerWearingHat, self);
                self.owner.room.AddObject(wornHat);
                return;
            }
        }

        /*
         * Draws worn hat if it exists, every update
         */
        private void PlayerGraphics_DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig.Invoke(self, sLeaser, rCam, timeStacker, camPos);
            if (playerWearingHat != null && wornHat != null)
            {
                wornHat.ParentDrawSprites(sLeaser, rCam, timeStacker, camPos);
            }
        }

        /* ----------- Player general hat methods ----------- */

        /*
         * Adds custom button check for wearHat to controls (required for good buttonpress response)
         */
        private void Player_checkInput(On.Player.orig_checkInput orig, Player self)
        {
            orig(self);

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
                    createHatInput[0] = Input.GetKeyDown(createHatKey);
                    wearHatInput[0] = Input.GetKeyDown(wearHatKey);
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

        /*
         * Controls for creating + wearing hat
         */
        private void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
        {
            // addHatEffects(self);

            orig.Invoke(self, eu);

            // create hat flag
            bool hatFlag = self != null && createHatInput[0] && !createHatInput[1];
            if (hatFlag)
            {
                // generate random hat type out of all existing hat types
                HatType newHatType = (HatType) (rand.Next() % Enum.GetValues(typeof(HatType)).Length);
                Debug.Log("hatworld new hat generated " + newHatType);
                // HatType newHatType = HatType.Flower;

                HatAbstract newHat = new HatAbstract(self.room.world, self.abstractCreature.pos, self.room.game.GetNewID(), newHatType);
                self.room.abstractRoom.AddEntity(newHat);
                newHat.RealizeInRoom();
                self.SlugcatGrab(newHat.realizedObject, 0);
            }

            // wear hat flag
            bool wearFlag = self != null && wearHatInput[0] && !wearHatInput[1];
            if (wearFlag)
            {
                if (playerWearingHat == null)
                {
                    // if holding hat, remove held hat and add wear hat
                    for (int i = 0; i < 2; i++)
                    {
                        if (self.grasps[i] != null && self.grasps[i].grabbed is HatPhysical)
                        {
                            PhysicalObject grabbed = self.grasps[i].grabbed;
                            HatType grabbedHatType = ((HatPhysical) grabbed).hatType;
                            grabbed.Destroy();
                            self.room.RemoveObject(grabbed);
                            self.room.abstractRoom.RemoveEntity(grabbed.abstractPhysicalObject);
                            self.ReleaseGrasp(i);

                            // add worn hat
                            wornHat = addWornHat(grabbedHatType, self.graphicsModule);
                            playerWearingHat = grabbedHatType;
                            break;
                        }
                    }
                } else {
                    // remove worn hat
                    playerWearingHat = null;
                    HatType hatType = wornHat.hatType;
                    wornHat.Destroy();
                    self.room.RemoveObject(wornHat);

                    // add held hat
                    HatAbstract heldHat = new HatAbstract(self.room.world, self.abstractCreature.pos, self.room.game.GetNewID(), hatType);
                    self.room.abstractRoom.AddEntity(heldHat);
                    heldHat.RealizeInRoom();
                    self.SlugcatGrab(heldHat.realizedObject, 0);
                }
            }

        }
 
        /*
         * Add/remove special effects of hat being worn.
         */
        private void addHatEffects(Player self)
        {
            if (playerWearingHat == HatType.Wizard)
            {
                self.gravity = 0.8f;
            } else if (playerWearingHat != HatType.Wizard)
            {
                self.gravity = 0.9f;
            }
        }        

        /*
         * Given what the hat type should be, creates a WearingHat for when the player needs one
         * Always called with playerWearingHat as hatType
         */
        private WearingHat addWornHat(HatType hatType, GraphicsModule graphicsModule)
        {
            switch (hatType)
            {
                case HatType.Santa:
                    return new WearingSantaHat(graphicsModule, 3, -90f, 5f);

                case HatType.Wizard:
                    return new WearingWizardHat(graphicsModule, 3, -90f, 5f);

                case HatType.Torch:
                    return new WearingTorchHat(graphicsModule, 3, -90f, 5f);

                case HatType.Flower:
                    return new WearingFlowerHat(graphicsModule, 3, -90f, 5f);

                default:
                    return new WearingSantaHat(graphicsModule, 3, -90f, 5f);
            }
        } 
    
    }
}
