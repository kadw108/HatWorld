using UnityEngine;
using RWCustom;

namespace HatWorld
{
    sealed class WizardHatPhysical : HatPhysical
    {
        public float lastDarkness = -1f;
        public float darkness;

        // public HatAbstract Abstr { get; }

        // taken from FestiveWorld SantaHat
        public Vector2 tuftPos = Vector2.zero;
        public Vector2 lastTuftPos = Vector2.zero;
        // -- set in constructor, hardcoded to slugcat values
        public float headRadius = 5f;

        // rotation (initial value from SantaHat)
        public Vector2 rotation;
        public Vector2 lastRotation;

        // etc...
        // To spawn a CustomPO in the world, use `new CustomAPO(world, pos, world.game.GetNewID()).Spawn()`.

        // Constants for sLeaser sprite index (higher index appears over lower)
        public int coneIndex = 0;
        public int tuftIndex = 1;
        public int beltIndex = 2;
        public int botIndex = 3;

		public override HatType hatType => HatType.Wizard;

        public WizardHatPhysical(HatAbstract abstr, World world) : base(abstr, world)
        {
            /*
            Abstr = abstr;

            bodyChunks = new BodyChunk[1];
            bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0f, 0f), 5f, 0.07f);
            bodyChunkConnections = new PhysicalObject.BodyChunkConnection[0];
            airFriction = 0.999f;
            gravity = 0.8f;
            bounce = 0.3f;
            surfaceFriction = 0.45f;
            collisionLayer = 1;
            waterFriction = 0.92f;
            buoyancy = 0.75f;
            firstChunk.loudness = 3f;
            */
        }

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

            // taken from FestiveWorld SantaHat
            this.lastTuftPos = this.tuftPos;
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            // Taken from FestiveWorld SantaHat
            sLeaser.sprites = new FSprite[4];
            TriangleMesh.Triangle[] array = new TriangleMesh.Triangle[]
            {
                new TriangleMesh.Triangle(0, 1, 2),
                new TriangleMesh.Triangle(1, 2, 3),
                new TriangleMesh.Triangle(2, 3, 4),
                new TriangleMesh.Triangle(3, 4, 5),
                new TriangleMesh.Triangle(4, 5, 6),
                new TriangleMesh.Triangle(5, 6, 7),
                new TriangleMesh.Triangle(6, 7, 8)
            };
            TriangleMesh cone = new TriangleMesh("Futile_White", array, false, false);
            sLeaser.sprites[coneIndex] = cone;
            sLeaser.sprites[tuftIndex] = new FSprite("mouseEyeA5", true);
            sLeaser.sprites[beltIndex] = new FSprite("LizardScaleA6", true);
            sLeaser.sprites[beltIndex].scaleY = 0.8f;
			sLeaser.sprites[botIndex] = new FSprite("SpearFragment2", true);
			sLeaser.sprites[botIndex].scaleY = 1.4f;
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
                if (j != coneIndex) // the triangle vertices are set later on in this method, moving them twice puts them in the wrong place
                {
                    sLeaser.sprites[j].x = pos.x - camPos.x;
                    sLeaser.sprites[j].y = pos.y - camPos.y;
                }
            }

            // rotate bottom + tuft
            Vector2 v = Vector3.Slerp(this.lastRotation, this.rotation, timeStacker);
            float hatRotation = Custom.VecToDeg(v);
            sLeaser.sprites[tuftIndex].rotation = hatRotation;
            sLeaser.sprites[beltIndex].rotation = hatRotation;
            sLeaser.sprites[botIndex].rotation = hatRotation;

            // setup
            Vector2 drawPos = new Vector2(sLeaser.sprites[botIndex].x, sLeaser.sprites[botIndex].y);
            Vector2 upDir = new Vector2(Mathf.Cos((hatRotation) * -0.017453292f), Mathf.Sin((hatRotation) * -0.017453292f));
            Vector2 rightDir = -Custom.PerpendicularVector(upDir);
            // drawPos += upDir * this.headRadius; doesn't work with held item, if included puts gap between tri and bottom

			/* Tuft */
			const float TUFTNUM = 25f; // some combination of height and stretch, changing it too much ruins the tuft bobble
			Vector2 targetTuftPos = drawPos + upDir * (TUFTNUM + 5);
			if (!Custom.DistLess(this.tuftPos, targetTuftPos, TUFTNUM))
			{
				this.tuftPos = targetTuftPos + (this.tuftPos - targetTuftPos).normalized * TUFTNUM;
				if (!Custom.DistLess(this.lastTuftPos, this.tuftPos, TUFTNUM))
				{
					this.lastTuftPos = this.tuftPos + (this.lastTuftPos - this.tuftPos).normalized * TUFTNUM;
				}
			}
			Vector2 tuftLocation = Vector2.Lerp(this.lastTuftPos, this.tuftPos, timeStacker);
			sLeaser.sprites[tuftIndex].SetPosition(tuftLocation);

            /* Belt */
            Vector2 beltLocation = new Vector2(sLeaser.sprites[botIndex].x, sLeaser.sprites[botIndex].y) + upDir * 3;
            sLeaser.sprites[beltIndex].SetPosition(beltLocation);

            /* Cone */
            TriangleMesh cone = (TriangleMesh)sLeaser.sprites[0];
            Vector2 coneTip = Vector2.Lerp(lastTuftPos, tuftPos, timeStacker);
            for (int i = 0, len = cone.vertices.Length; i < len; i++)
            {
                bool r = i % 2 == 1;
                float h = i / 2 / (float)(len - 1) * 2f;

                Vector2 coneBase;
                if (r)
                    coneBase = drawPos - rightDir * 7f;
                else
                    coneBase = drawPos + rightDir * 7f;
                Vector2 coneMid = Vector2.Lerp(coneBase, targetTuftPos, 0.5f);

                Vector2 verticePos = Vector2.Lerp(Vector2.Lerp(coneBase, coneMid, h), Vector2.Lerp(coneMid, coneTip, h), h);
                cone.MoveVertice(i, verticePos);
            }


            if (slatedForDeletetion || room != rCam.room)
            {
                sLeaser.CleanSpritesAndRemove();
            }
        }

        public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            //blackColor = palette.blackColor;
            //earthColor = Color.Lerp(palette.fogColor, palette.blackColor, 0.5f);

			sLeaser.sprites[coneIndex].color = new Color(0.35f, 0.4f, 0.8f); // blue

			sLeaser.sprites[tuftIndex].color = new Color(1f, 0.80f, 0.49f); // orange yellow
            // sLeaser.sprites[tuftIndex].color = new Color(0.8f, 0.8f, 0.9f); // silver blue
			// sLeaser.sprites[tuftIndex].color = new Color(1f, 0.96f, 0.55f); // light yellow

            sLeaser.sprites[beltIndex].color = sLeaser.sprites[tuftIndex].color;
            sLeaser.sprites[botIndex].color = sLeaser.sprites[coneIndex].color;
        }
    }
}

