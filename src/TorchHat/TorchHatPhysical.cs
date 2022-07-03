﻿using UnityEngine;
using RWCustom;

namespace HatWorld
{
    sealed class TorchHatPhysical : HatPhysical
    {
        // For glow
        public LightSource lightSource;

        // Constants for sLeaser sprite index (higher index appears over lower)
        public int crownIndex = 0;
        public int gemIndex = 1;
        public int gleamIndex = 2;

		public override HatType hatType => HatType.Torch;

        public TorchHatPhysical(HatAbstract abstr, World world) : base(abstr, world) {}

        public override void Update(bool eu)
        {
            base.Update(eu);

            // taken from Mushroom Update
            this.lastDarkness = this.darkness;
            this.darkness = this.room.Darkness(base.firstChunk.pos);
            this.lastRotation = this.rotation;
            if (this.grabbedBy.Count > 0)
            {
                this.rotation = Custom.PerpendicularVector(Custom.DirVec(base.firstChunk.pos, this.grabbedBy[0].grabber.mainBodyChunk.pos));
                this.rotation.y = Mathf.Abs(this.rotation.y);
            }
        }

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

            /* Add glow */
            Vector2 firePos = drawPos + camPos;
            // From Lantern in game code
            if (this.lightSource == null)
            {
                this.lightSource = new LightSource(firePos, false, new Color(1f, 0.8f, 0.4f), this);
                this.lightSource.affectedByPaletteDarkness = 0.9f;
                float flicker = 1 + Mathf.Pow(Random.value, 3f) * 0.1f * ((Random.value >= 0.5f) ? 1f : -1f);
                this.lightSource.setRad = new float?(35f * flicker);
                this.lightSource.setAlpha = new float?(0.9f);
                this.room.AddObject(this.lightSource);
            }
            else
            {
                this.lightSource.setPos = new Vector2?(firePos);
            }

            if (!(slatedForDeletetion || room != rCam.room))
            {
                /* Fire particles */
                this.room.AddObject(new FlameParticle(drawPos, 10f));
            }
        }

        public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            sLeaser.sprites[crownIndex].color = new Color(0.82f, 0.62f, 0f); // gold 2
        }
    }
}

