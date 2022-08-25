using System.Linq;
using RWCustom;
using UnityEngine;

namespace HatWorld
{
    public abstract class HatPhysical : PlayerCarryableItem, IDrawable
    {
        public HatAbstract Abstr { get; }

        // rotation (initial value from SantaHat)
        protected Vector2 rotation;
        protected Vector2 lastRotation;

        // for DrawSprites
        protected Vector2 drawPos;
        protected float hatRotation;
        protected Vector2 upDir;
        protected Vector2 rightDir;

        public HatPhysical(HatAbstract abstr, World world) : base(abstr)
        {
            Abstr = abstr;

            this.bodyChunks = new BodyChunk[1];
            this.bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0f, 0f), 5f, 0.07f);
            this.bodyChunkConnections = new PhysicalObject.BodyChunkConnection[0];
            this.airFriction = 0.999f;
            this.gravity = 0.8f;
            this.bounce = 0.3f;
            this.surfaceFriction = 0.45f;
            this.collisionLayer = 1;
            this.waterFriction = 0.92f;
            this.buoyancy = 0.75f;
            this.firstChunk.loudness = 3f;
        }

        public override float ThrowPowerFactor
        {
            get
            {
                return 0.3f;
            }
        }

        public override void PlaceInRoom(Room placeRoom)
        {
            base.PlaceInRoom(placeRoom);
            base.firstChunk.HardSetPosition(placeRoom.MiddleOfTile(this.abstractPhysicalObject.pos));
            this.NewRoom(placeRoom);

            this.rotation = new Vector2(-1, 0); // points hat up (270 deg rotation)
        }

        public override void Update(bool eu)
        {
            base.Update(eu);

            bool flag = false;
            if (this.grabbedBy.Any((Creature.Grasp grasp) => grasp.grabber is Player))
            {
                flag = true;
            }
            PlacedObjectInfo placedObjectInfo;
            if (flag && HatPlacer.infos.TryGetValue(this.Abstr, out placedObjectInfo))
            {
                StoryGameSession getStorySession = this.room.game.GetStorySession;
                if (getStorySession != null)
                {
                    getStorySession.saveState.ReportConsumedItem(this.room.world, false, placedObjectInfo.origRoom, placedObjectInfo.placedObjectIndex, Random.Range(placedObjectInfo.minRegen, placedObjectInfo.maxRegen));
                }
                HatPlacer.infos.Remove(this.Abstr);
            }

            // From Mushroom.Update
            this.lastRotation = this.rotation;
            if (this.grabbedBy.Count > 0)
            {
                this.rotation = Custom.PerpendicularVector(Custom.DirVec(base.firstChunk.pos, this.grabbedBy[0].grabber.mainBodyChunk.pos));
                this.rotation.y = Mathf.Abs(this.rotation.y);
            }
        }

        public abstract void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam);

        public virtual void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            if (slatedForDeletetion || room != rCam.room)
            {
                sLeaser.CleanSpritesAndRemove();
            }

            // From CentiShields
            /* Default DrawSprites code, gets basic values */
            drawPos = Vector2.Lerp(firstChunk.lastPos, firstChunk.pos, timeStacker);
            float temp = Mathf.InverseLerp(305f, 380f, timeStacker);
            drawPos -= new Vector2(0, 20f * Mathf.Pow(temp, 3f));
            drawPos -= camPos;

            // rotate bottom + tuft
            Vector2 v = Vector3.Slerp(this.lastRotation, this.rotation, timeStacker);
            hatRotation = Custom.VecToDeg(v);

            // setup
            upDir = new Vector2(Mathf.Cos((hatRotation) * -0.017453292f), Mathf.Sin((hatRotation) * -0.017453292f));
            rightDir = -Custom.PerpendicularVector(upDir);
        }

        public abstract void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette);

        public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            if (newContatiner == null)
                newContatiner = rCam.ReturnFContainer("Items");

            foreach (FSprite fsprite in sLeaser.sprites)
            {
                fsprite.RemoveFromContainer();
                newContatiner.AddChild(fsprite);
            }
        }

        // equivalent of a static abstract method but those don't exist in C# and I am too lazy to find a better option
        public static HatWearing GetWornHat(GraphicsModule graphicsModule)
        {
            return null;
        }
    }
}
