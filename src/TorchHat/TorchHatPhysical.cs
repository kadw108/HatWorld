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

        // etc...
        // To spawn a CustomPO in the world, use `new CustomAPO(world, pos, world.game.GetNewID()).Spawn()`.

        // Constants for sLeaser sprite index (higher index appears over lower)
        public int botIndex = 0;
        public int fireIndex = 1;

		public override HatType hatType => HatType.Torch;

        public TorchHatPhysical(HatAbstract abstr, World world) : base(abstr, world) { }

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

            // taken from FestiveWorld SantaHat (with changes)
            // this.lastTuftPos = this.tuftPos;
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
			sLeaser.sprites = new FSprite[2];
			sLeaser.sprites[botIndex] = new FSprite("SpiderLeg3B", true);
			sLeaser.sprites[fireIndex] = new FSprite("deerEyeB", true);

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

            // rotate bottom + tuft
            Vector2 v = Vector3.Slerp(this.lastRotation, this.rotation, timeStacker);
            float hatRotation = Custom.VecToDeg(v);
            foreach (var sprite in sLeaser.sprites) sprite.rotation = hatRotation;

            // setup
            Vector2 drawPos = sLeaser.sprites[botIndex].GetPosition();
            Vector2 upDir = new Vector2(Mathf.Cos((hatRotation) * -0.017453292f), Mathf.Sin((hatRotation) * -0.017453292f));
            Vector2 rightDir = -Custom.PerpendicularVector(upDir);
            // drawPos += upDir * this.headRadius; doesn't work with held item, if included puts gap between tri and bottom

            if (slatedForDeletetion || room != rCam.room)
            {
                sLeaser.CleanSpritesAndRemove();
            }
        }

        public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
			sLeaser.sprites[botIndex].color = new Color(0.15f, 0.07f, 0.2f);
        }
    }
}

