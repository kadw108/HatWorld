﻿using UnityEngine;
using HatWorld;

namespace HatWorldCustomExample
{
    sealed class RedBubblePhysical : HatPhysical
    {
        // Constants for sLeaser sprite index (higher index appears over lower)
        public const int glassIndex = 0;
        public const int neckIndex = 1;
        public const int edgeIndex = 2;

        // Every HatPhysical derived class should implement this method and return the equivalent HatWearing class.
        public static new HatWearing GetWornHat(GraphicsModule graphicsModule)
        {
            return new RedBubbleWearing(graphicsModule);
        }
        public RedBubblePhysical(HatAbstract abstr, World world) : base(abstr, world) { }

        // Determine which/how many sprites the item has
        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[3];
            sLeaser.sprites[neckIndex] = new FSprite("SpearFragment2", true) { scale = 1.1f }; // neck collar
            sLeaser.sprites[edgeIndex] = new FSprite("LizardBubble7", true) { scale = 1.3f }; // edge of bubble
            sLeaser.sprites[glassIndex] = new FSprite("Circle20", true) { scale = 1f }; // inside of bubble

            this.AddToContainer(sLeaser, rCam, null);
        }

        // Setup - determine where sprites get drawn
        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
            for (int j = 0; j < sLeaser.sprites.Length; j++)
            {
                sLeaser.sprites[j].rotation = hatRotation;
            }

            sLeaser.sprites[neckIndex].SetPosition(drawPos + upDir * -10);
            sLeaser.sprites[edgeIndex].SetPosition(drawPos);
            sLeaser.sprites[glassIndex].SetPosition(drawPos);
        }

        // Add colors to sprites
        public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            sLeaser.sprites[neckIndex].color = new Color(0.74f, 0.83f, 0.90f);
            sLeaser.sprites[edgeIndex].color = new Color(0.57f, 0.79f, 0.94f);
            sLeaser.sprites[glassIndex].color = new Color(1f, 0f, 0f, 0.5f);
        }
    }
}
