using UnityEngine;
using RWCustom;

namespace HatWorld
{
    sealed class TorchHatPhysical : HatPhysical
    {
        public float lastDarkness = -1f;
        public float darkness;

        // public HatAbstract Abstr { get; }

        // taken from FestiveWorld SantaHat
        // -- set in constructor, hardcoded to slugcat values
        public float headRadius = 5f;

        // For glow
        public LightSource lightSource;

        // etc...
        // To spawn a CustomPO in the world, use `new CustomAPO(world, pos, world.game.GetNewID()).Spawn()`.

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
			sLeaser.sprites = new FSprite[3];
			sLeaser.sprites[crownIndex] = new FSprite("SpiderLeg3B", true);
            sLeaser.sprites[crownIndex].scaleX = 1.5f;
			sLeaser.sprites[gemIndex] = new FSprite("deerEyeA", true);
            sLeaser.sprites[gemIndex].scale = 0.6f;
            sLeaser.sprites[gleamIndex] = new FSprite("tinyStar", true);

            this.AddToContainer(sLeaser, rCam, null);
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            // Taken from CentiShields
            /* Default DrawSprites code, gets basic values */
            Vector2 pos = Vector2.Lerp(firstChunk.lastPos, firstChunk.pos, timeStacker);
            float temp = Mathf.InverseLerp(305f, 380f, timeStacker);
            pos.y -= 20f * Mathf.Pow(temp, 3f);
            for (int j = 0; j < sLeaser.sprites.Length; j++)
            {
                sLeaser.sprites[j].x = pos.x - camPos.x;
                sLeaser.sprites[j].y = pos.y - camPos.y;
            }

            // Rotate hat
            Vector2 v = Vector3.Slerp(this.lastRotation, this.rotation, timeStacker);
            float hatRotation = Custom.VecToDeg(v);
            foreach (var sprite in sLeaser.sprites) sprite.rotation = hatRotation;

            // Setup
            Vector2 drawPos = sLeaser.sprites[crownIndex].GetPosition();
            Vector2 upDir = new Vector2(Mathf.Cos((hatRotation) * -0.017453292f), Mathf.Sin((hatRotation) * -0.017453292f));
            Vector2 rightDir = -Custom.PerpendicularVector(upDir);

            /* Set positions */
            sLeaser.sprites[gemIndex].SetPosition(drawPos + upDir * 3f);
            sLeaser.sprites[gleamIndex].SetPosition((sLeaser.sprites[gemIndex].GetPosition())+ new Vector2(-0.5f, 1.5f));

            /* Add glow */
            // From Lantern in game code
            if (this.lightSource == null)
            {
                this.lightSource = new LightSource(pos, false, new Color(1f, 0.8f, 0.4f), this);
                this.lightSource.affectedByPaletteDarkness = 0.9f;
                float flicker = 1 + Mathf.Pow(Random.value, 3f) * 0.1f * ((Random.value >= 0.5f) ? 1f : -1f);
                this.lightSource.setRad = new float?(35f * flicker);
                this.lightSource.setAlpha = new float?(0.9f);
                this.room.AddObject(this.lightSource);
            }
            else
            {
                this.lightSource.setPos = new Vector2?(pos);
            }

            if (slatedForDeletetion || room != rCam.room)
            {
                sLeaser.CleanSpritesAndRemove();
            }
        }

        public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            sLeaser.sprites[crownIndex].color = new Color(0.82f, 0.62f, 0f); // gold 2
            // sLeaser.sprites[gemIndex].color = new Color(1f, 0.86f, 0.40f); // orange
            // sLeaser.sprites[gemIndex].color = new Color(1f, 0.91f, 0.73f); // very light orange
            // sLeaser.sprites[gemIndex].color = new Color(1f, 0.65f, 0.49f); // light red orange
            // sLeaser.sprites[gemIndex].color = new Color(1f, 0.95f, 0.36f); // bright yellow
            sLeaser.sprites[gemIndex].color = new Color(0.95f, 0.34f, 0.25f); // red
        }
    }
}

