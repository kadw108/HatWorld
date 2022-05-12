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

        // Constants for sLeaser sprite index
        public int triIndex = 0;
        public int tuftIndex = 1;
        public int botIndex = 2;

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
            // Taken from CentiShields
            /* Default DrawSprites code, gets basic values */
            Vector2 pos = Vector2.Lerp(firstChunk.lastPos, firstChunk.pos, timeStacker);
            float temp = Mathf.InverseLerp(305f, 380f, timeStacker);
            pos.y -= 20f * Mathf.Pow(temp, 3f);
            for (int j = 0; j < sLeaser.sprites.Length; j++)
            {
                if (j != triIndex) // the triangle vertices are set later on in this method, moving them twice puts them in the wrong place
                {
                    sLeaser.sprites[j].x = pos.x - camPos.x;
                    sLeaser.sprites[j].y = pos.y - camPos.y;
                }
            }

            // rotate bottom + tuft
            Vector2 v = Vector3.Slerp(this.lastRotation, this.rotation, timeStacker);
            float hatRotation = Custom.VecToDeg(v);
            sLeaser.sprites[tuftIndex].rotation = hatRotation;
            sLeaser.sprites[botIndex].rotation = hatRotation;

            /* white ball (tuft) code */
            Vector2 basePosVector = new Vector2(sLeaser.sprites[botIndex].x, sLeaser.sprites[botIndex].y);
            Vector2 tuftBobble = new Vector2(Mathf.Cos((hatRotation) * -0.017453292f), Mathf.Sin((hatRotation) * -0.017453292f));
            Vector2 vector3 = -Custom.PerpendicularVector(tuftBobble);
            // basePosVector += tuftBobble * this.headRadius; doesn't work with held item, if included puts gap between tri and bottom
            Vector2 vector4 = basePosVector + tuftBobble * 20f;
            if (!Custom.DistLess(this.tuftPos, vector4, 20f))
            {
                this.tuftPos = vector4 + (this.tuftPos - vector4).normalized * 20f;
                if (!Custom.DistLess(this.lastTuftPos, this.tuftPos, 20f))
                {
                    this.lastTuftPos = this.tuftPos + (this.lastTuftPos - this.tuftPos).normalized * 20f;
                }
            }
            Vector2 tuftLocation = Vector2.Lerp(this.lastTuftPos, this.tuftPos, timeStacker);
            sLeaser.sprites[tuftIndex].SetPosition(tuftLocation);

            /* Triangle code */
            TriangleMesh triangleMesh = (TriangleMesh)sLeaser.sprites[triIndex];
            Vector2 triWidth1 = basePosVector - vector3 * 7f;
            Vector2 triWidth2 = basePosVector + vector3 * 7f;
            Vector2 triHeight1 = Vector2.Lerp(triWidth1, vector4, 0.5f);
            Vector2 triHeight2 = Vector2.Lerp(triWidth2, vector4, 0.5f);
            int i = 0;
            int num = triangleMesh.vertices.Length;
            while (i < num)
            {
                bool flag = i % 2 == 1;
                float num2 = (float)(i / 2) / (float)(num - 1) * 2f;
                Vector2 vector10 = Vector2.Lerp(Vector2.Lerp(flag ? triWidth2 : triWidth1, flag ? triHeight2 : triHeight1, num2), Vector2.Lerp(flag ? triHeight2 : triHeight1, tuftLocation, num2), num2);
                triangleMesh.MoveVertice(i, vector10);
                i++;
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
            sLeaser.sprites[triIndex].color = Color.blue;
            sLeaser.sprites[tuftIndex].color = Color.yellow;
            sLeaser.sprites[botIndex].color = Color.yellow;
        }
    }
}

