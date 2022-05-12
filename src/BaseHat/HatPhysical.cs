using UnityEngine;
using RWCustom;

namespace HatWorld
{
    abstract class HatPhysical : PlayerCarryableItem, IDrawable
    {
        public HatAbstract Abstr { get; }
        public abstract HatType hatType { get; }

        // To spawn a CustomPO in the world, use `new CustomAPO(world, pos, world.game.GetNewID()).Spawn()`.

        public HatPhysical(HatAbstract abstr, World world) : base(abstr)
        {
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
        }

        public override void Update(bool eu)
        {
            base.Update(eu);
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
