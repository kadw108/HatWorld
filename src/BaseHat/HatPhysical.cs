using UnityEngine;
using RWCustom;
using System.Linq;

namespace HatWorld
{
    abstract class HatPhysical : PlayerCarryableItem, IDrawable
    {
        // rotation (initial value from SantaHat)
        public Vector2 rotation;
        public Vector2 lastRotation;
        
        public HatAbstract Abstr { get; }
        public abstract HatType hatType { get; }

        // To spawn a CustomPO in the world, use `new CustomAPO(world, pos, world.game.GetNewID()).Spawn()`.

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

        // from datapearl not sure if needed
        public override void PlaceInRoom(Room placeRoom)
        {
            base.PlaceInRoom(placeRoom);
            base.firstChunk.HardSetPosition(placeRoom.MiddleOfTile(this.abstractPhysicalObject.pos));
            this.NewRoom(placeRoom);

            this.rotation = new Vector2(-1, 0); // points hats up (270 deg rotation)
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
        }

        public abstract void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam);

        public abstract void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos);

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
    }
}
