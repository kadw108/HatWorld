using UnityEngine;
using RWCustom;

namespace HatWorld
{
    sealed class SantaPhysical : HatPhysical
    {
        // taken from FestiveWorld SantaHat
        public Vector2 tuftPos;
        public Vector2 lastTuftPos;
		public Vector2 tuftVel;
        // -- set in constructor, hardcoded to slugcat values
        public float headRadius = 5f;

        // etc...
        // To spawn a CustomPO in the world, use `new CustomAPO(world, drawPos, world.game.GetNewID()).Spawn()`.

        // Constants for sLeaser sprite index
        public int triIndex = 0;
        public int tuftIndex = 1;
        public int botIndex = 2;

        public override HatWearing GetWornHat(GraphicsModule graphicsModule)
        {
            return new SantaWearing(graphicsModule);
        }

        public SantaPhysical(HatAbstract abstr, World world) : base(abstr, world) {}

        public override void Update(bool eu)
        {
            base.Update(eu);

            // taken from FestiveWorld SantaHat (with changes)
            this.lastTuftPos = this.tuftPos;

            if (this.room != null)
			{
                float rotationFloat = Custom.VecToDeg(this.rotation);
                Vector2 upDir = new Vector2(Mathf.Cos((rotationFloat) * -0.017453292f), Mathf.Sin((rotationFloat) * -0.017453292f));
                Vector2 rightDir = -Custom.PerpendicularVector(upDir);

                Vector2 tipPos = this.tuftPos;
				this.tuftVel.y -= this.gravity;
				this.tuftVel += rightDir * ((Vector2.Dot(rightDir, this.tuftPos - tipPos) > 0f) ? 1.5f : -1.5f);
				this.tuftVel += (tipPos - this.tuftPos) * 0.2f;
				this.tuftVel *= 0.6f;
				this.tuftPos += this.tuftVel;
				if (!Custom.DistLess(this.tuftPos, tipPos, 13f))
				{
					this.tuftPos = tipPos + (this.tuftPos - tipPos).normalized * 13f;
				}
			}
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            // Taken from FestiveWorld SantaHat
            sLeaser.sprites = new FSprite[3];
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
            TriangleMesh triangleMesh = new TriangleMesh("Futile_White", array, false, false);
            sLeaser.sprites[triIndex] = triangleMesh;
            sLeaser.sprites[tuftIndex] = new FSprite("JetFishEyeA", true);
            sLeaser.sprites[botIndex] = new FSprite("LizardScaleA6", true);
            this.AddToContainer(sLeaser, rCam, null);
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            // Setup
            base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
            for (int j = 0; j < sLeaser.sprites.Length; j++)
            {
                if (j != triIndex) // the triangle vertices are set later on in this method, moving them twice puts them in the wrong place
                {
                    sLeaser.sprites[j].SetPosition(drawPos);
                    sLeaser.sprites[j].rotation = hatRotation;
                }
            }

            /* white ball (tuft) code */
            // drawPos += upDir * this.headRadius; doesn't work with held item, if included puts gap between cone and bottom
            Vector2 targetTuftPos = drawPos + upDir * 20f;
            if (!Custom.DistLess(this.tuftPos, targetTuftPos, 20f))
            {
                this.tuftPos = targetTuftPos + (this.tuftPos - targetTuftPos).normalized * 20f;
                if (!Custom.DistLess(this.lastTuftPos, this.tuftPos, 20f))
                {
                    this.lastTuftPos = this.tuftPos + (this.lastTuftPos - this.tuftPos).normalized * 20f;
                }
            }
            Vector2 tuftLocation = Vector2.Lerp(this.lastTuftPos, this.tuftPos, timeStacker);
            sLeaser.sprites[tuftIndex].SetPosition(tuftLocation);

            // Cone
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
        }

        public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            //blackColor = palette.blackColor;
            //earthColor = Color.Lerp(palette.fogColor, palette.blackColor, 0.5f);
            sLeaser.sprites[triIndex].color = Color.red;
            sLeaser.sprites[tuftIndex].color = Color.white;
            sLeaser.sprites[botIndex].color = Color.white;
        }
    }
}
