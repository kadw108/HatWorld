﻿using System;
// for mod integration (hooking mod methods)
using System.Reflection;
using MonoMod.RuntimeDetour; // if you get 'instance object was created as immutable' error, edit the .csproj manually
using UnityEngine;

namespace HatWorld.src.HatManager
{
    public class PlayerHatManager : CreatureHatManager
    {
        // tracks buttonpresses for custom wear-hat button
        private bool[] createHatInput = new bool[10];
        private bool[] wearHatInput = new bool[10];

        public const int maxPlayerNum = 4;
        public static KeyCode[] createHatKeys = new KeyCode[maxPlayerNum] { KeyCode.None, KeyCode.None, KeyCode.None, KeyCode.None };
        public static KeyCode[] wearHatKeys = new KeyCode[maxPlayerNum] { KeyCode.A, KeyCode.A, KeyCode.A, KeyCode.A };

        public PlayerHatManager(EntityID wearer) : base(wearer) { }

        public override void AddHooks()
        {
            base.AddHooks();

            if (HatWorldMain.fancyGraphicsRef != null)
            {
                // equivalent: On.FancySlugcats.FancyPlayerGraphics.InitiateSprites += FancyPlayerGraphics_InitiateSprites
                new Hook(HatWorldMain.fancyGraphicsRef.GetMethod("InitiateSprites",
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance),
                    typeof(PlayerHatManager).GetMethod("FancyPlayerGraphics_InitiateSprites",
                        BindingFlags.Public | BindingFlags.Instance),
                    this);

                // equivalent: On.FancySlugcats.FancyPlayerGraphics.DrawSprites += FancyPlayerGraphics_DrawSprites
                new Hook(HatWorldMain.fancyGraphicsRef.GetMethod("DrawSprites",
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance),
                    typeof(PlayerHatManager).GetMethod("FancyPlayerGraphics_DrawSprites",
                        BindingFlags.Public | BindingFlags.Instance),
                    this);
            }

            // add new button response to player controls
            On.Player.checkInput += Player_CheckInput;
            On.Player.Update += Player_Update;
        }

        /*
         * Hook method for FancySlugcats.FancyPlayerGraphics.InitiateSprites
         */
        public delegate void InitiateSprites_signature(
            PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam);
        public void FancyPlayerGraphics_InitiateSprites(InitiateSprites_signature orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            orig.Invoke(self, sLeaser, rCam);

            if ((self.owner as Creature != null) && (self.owner as Creature).abstractCreature.ID == wearer)
            {
                if (self != null && physicalWornHat != null)
                {
                    wornHat = (HatWearing)physicalWornHat.GetMethod("GetWornHat").Invoke(null, new object[] { self });
                    self.owner.room.AddObject(wornHat);
                    return;
                }
            }
        }

        /*
         * Hook method for FancySlugcats.FancyPlayerGraphics.DrawSprites
         */
        public delegate void DrawSprites_signature(
            PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos);
        public void FancyPlayerGraphics_DrawSprites(DrawSprites_signature orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig.Invoke(self, sLeaser, rCam, timeStacker, camPos);

            if ((self.owner as Creature != null) && (self.owner as Creature).abstractCreature.ID == wearer)
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
        private void Player_CheckInput(On.Player.orig_checkInput orig, Player self)
        {
            orig(self);

            if (self.abstractCreature.ID == wearer)
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

            if (self.abstractCreature.ID == wearer)
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
