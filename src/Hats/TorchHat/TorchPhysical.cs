using UnityEngine;
using RWCustom;

namespace HatWorld
{
    sealed class TorchPhysical : HatPhysical
    {
        // For glow
        public LightSource? lightSource;

        // Constants for sLeaser sprite index (higher index appears over lower)
        public const int crownIndex = 0;
        public const int gemIndex = 1;

        public static new HatWearing GetWornHat(GraphicsModule graphicsModule)
        {
            return new TorchWearing(graphicsModule);
        }

        public TorchPhysical(HatAbstract abstr, World world) : base(abstr, world) {}

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
			sLeaser.sprites = new FSprite[2];
			sLeaser.sprites[crownIndex] = new FSprite("SpiderLeg3B", true);
            sLeaser.sprites[crownIndex].scaleX = 1.5f;
			sLeaser.sprites[gemIndex] = new FSprite("deerEyeA", true);
            sLeaser.sprites[gemIndex].scale = 0.4f;

            this.AddToContainer(sLeaser, rCam, null);
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            // Setup
            base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
            for (int j = 0; j < sLeaser.sprites.Length; j++)
            {
                sLeaser.sprites[j].SetPosition(drawPos);
                sLeaser.sprites[j].rotation = hatRotation;
            }

            /* Set positions */
            sLeaser.sprites[gemIndex].SetPosition(drawPos + upDir * 2f);

            bool inWater = (this.firstChunk.submersion > 0.9 || (this.room != null && this.room.roomRain != null && this.room.roomRain.intensity > 0.4));
            if (slatedForDeletetion || room != rCam.room || inWater)
            {
                if (this.lightSource != null && this.room != null)
                {
                    // Remove LightSource objects when changing rooms
                    this.room.RemoveObject(this.lightSource);
                }
                this.lightSource = null;
            }
            else if (!inWater)
            {
                /* Fire particles */
                this.room.AddObject(new FlameParticle(drawPos, 10f, new Vector2(0, 0)));

                /* Add glow */
                Vector2 firePos = drawPos + camPos;
                // From Lantern in game code
                if (this.lightSource == null)
                {
                    this.lightSource = new LightSource(firePos, false, new Color(1f, 0.8f, 0.4f), this);
                    this.lightSource.affectedByPaletteDarkness = 0.5f;
                    float flicker = 1 + Mathf.Pow(Random.value, 3f) * 0.1f * ((Random.value >= 0.5f) ? 1f : -1f);
                    this.lightSource.setRad = new float?(45f * flicker);
                    this.lightSource.setAlpha = new float?(0.9f);
                    this.room.AddObject(this.lightSource);
                }
                else
                {
                    this.lightSource.setPos = new Vector2?(firePos);
                }
            }
        }

        public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            sLeaser.sprites[crownIndex].color = new Color(0.82f, 0.62f, 0f); // gold
        }
    }
}

