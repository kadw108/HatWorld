using UnityEngine;
using RWCustom;

namespace HatWorld
{
    public class AntennaPhysical : HatPhysical
    {
        // Constants for sLeaser sprite index (higher index appears over lower)
        public const int botDisk = 1;
        public const int pole = 0;
        public const int topDisk = 2;
        public const int topCircle = 3;

        public override HatWearing getWornHat(GraphicsModule graphicsModule)
        {
            return new AntennaWearing(graphicsModule);
        }

        public AntennaPhysical(HatAbstract abstr, World world) : base(abstr, world) { }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[4];
            sLeaser.sprites[botDisk] = new FSprite("QuarterPips2", true) { scaleX = 0.6f };
            sLeaser.sprites[pole] = new FSprite("LizardScaleA1", true) { scaleY = 1.1f, scaleX = 0.2f };
            sLeaser.sprites[topDisk] = new FSprite("QuarterPips2", true) { scaleY = 0.5f, scaleX = 0.4f };
            sLeaser.sprites[topCircle] = new FSprite("Circle4", true) { scale = 0.7f };
            this.AddToContainer(sLeaser, rCam, null);
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            base.DrawSprites(sLeaser, rCam, timeStacker, camPos);

            for (int j = 0; j < sLeaser.sprites.Length; j++)
            {
                sLeaser.sprites[j].rotation = hatRotation;
            }

            sLeaser.sprites[botDisk].SetPosition(drawPos + upDir * 1);
            sLeaser.sprites[botDisk].rotation += 180f;

            sLeaser.sprites[pole].SetPosition(drawPos + upDir * 8);
            sLeaser.sprites[pole].rotation += 90f;

            sLeaser.sprites[topDisk].SetPosition(drawPos + upDir * 9);
            sLeaser.sprites[topDisk].rotation += 180f;

            sLeaser.sprites[topCircle].SetPosition(drawPos + upDir * 16);
        }

        public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            sLeaser.sprites[botDisk].color = new Color(0.23f, 0.25f, 0.34f); // gray
            sLeaser.sprites[pole].color = new Color(0.9f, 0.9f, 0.9f); // light gray
            sLeaser.sprites[topDisk].color = sLeaser.sprites[botDisk].color;
            sLeaser.sprites[topCircle].color = new Color(1f, 0.95f, 0.46f); // gold
        }
    }
}


