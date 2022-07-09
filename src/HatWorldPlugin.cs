// default imports
using System;
using BepInEx;
using UnityEngine;

// physical objects library
using Fisobs;
using System.Collections.Generic;
using System.Reflection;

namespace HatWorld
{
    [BepInPlugin("kadw.hatworld", "HatWorld", "1.0.0")]
    public class HatWorldPlugin : BaseUnityPlugin
    {
        public static List<Type> hatTypes = new List<Type>() {
            typeof(SantaPhysical), typeof(WizardPhysical), typeof(BubblePhysical), typeof(FlowerPhysical),
            typeof(TorchPhysical)
        };

        // for spawning random hats
        public static System.Random rand = new System.Random();

        // tracks if player is wearing hat. null if no hat
        private HatWearing? wornHat = null;
        private HatPhysical? physicalWornHat = null; // physical object version of currently worn hat

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
            HatPlacer.AddSpawns(Assembly.GetExecutingAssembly().GetManifestResourceStream("HatWorld.src.HatPlacer.spawns.txt"));
        }

        /* ---------- Player worn hat methods ------------ */

        /*
         * Adds worn hat to player when player enters new room, if hat is being worn
         */
        private void PlayerGraphics_InitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            orig.Invoke(self, sLeaser, rCam);

            if (self != null && physicalWornHat != null)
            {
                wornHat = physicalWornHat.getWornHat(self);
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
            if (physicalWornHat != null && wornHat != null)
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
            addHatEffects(self);

            orig.Invoke(self, eu);

            // create hat flag
            bool hatFlag = self != null && createHatInput[0] && !createHatInput[1];
            if (hatFlag)
            {
                // generate random hat type out of all existing hat types
                Type newHatType = hatTypes[rand.Next() % hatTypes.Count];
                Debug.Log("hatworld new hat generated " + newHatType);
                // string newHatType = "HatWorld.BubblePhysical";

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
                            wornHat = physicalWornHat.getWornHat(self.graphicsModule);

                            // remove held hat
                            physicalWornHat.Destroy();
                            self.room.RemoveObject(physicalWornHat);
                            self.room.abstractRoom.RemoveEntity(physicalWornHat.abstractPhysicalObject);
                            self.ReleaseGrasp(i);
                            break;
                        }
                    }
                } else {
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
 
        /*
         * Add/remove special effects of hat being worn.
         */
        private void addHatEffects(Player self)
        {
            if (wornHat != null)
            {
                Type hatType = wornHat.GetType();
                if (hatType.Equals(typeof(WizardPhysical)))
                {
                    self.gravity = 0.4f;
                } else
                {
                    self.gravity = 0.9f;
                }
            }        
        }

        public static Type GetType(string typeName)
        {
            foreach(Type t in hatTypes)
            {
                if (t.ToString().Equals(typeName))
                {
                    return t;
                }
            }
            return null;
        }

        public static void addType(Type type)
        {
            hatTypes.Add(type); 
        }
    }
}
