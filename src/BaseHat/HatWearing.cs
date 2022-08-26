using RWCustom;
using UnityEngine;

namespace HatWorld
{
    // Hat that is being worn by slugcat
    // Uses code from FestiveWorld mod
    public abstract class HatWearing : UpdatableAndDeletable, IDrawable
    {
        public static bool effectsOn = true;

        public GraphicsModule parent { get; }

        private int anchorSprite;
        protected float rotation;
        protected float headRadius;

        // from ParentDrawSprites
        protected Vector2 basePos;
        protected float baseRot;
        protected bool flipX;
        protected bool flipY;

        // for DrawSprites
        protected Vector2 drawPos;
        protected Vector2 upDir;
        protected Vector2 rightDir;

        protected bool initialized;

        public HatWearing(GraphicsModule parent)
        {
            this.parent = parent;

            switch (parent)
            {
                case PlayerGraphics:
                    this.rotation = -90f;
                    this.headRadius = 5f;

                    // if FancySlugcats is active, the head sprite has a different index
                    if (HatWorldMain.fancyGraphicsRef == null)
                    {
                        this.anchorSprite = 3;
                    }
                    else
                    {
                        this.anchorSprite = 8;
                    }
                    break;

                case ScavengerGraphics:
                    this.anchorSprite = (parent as ScavengerGraphics).HeadSprite;
                    this.rotation = 90f;
                    this.headRadius = 7f;
                    break;
            }

            this.initialized = false; // ParentDrawSprites must run to set basePos and drawPos
            parent.owner.room.AddObject(this);

            if (effectsOn)
            {
                // Debug.Log("HatWorld: add hat effects " + this.GetType() + " " + parent.owner);
                this.AddHatEffects(parent.owner as Creature);
            }
        }

        public abstract void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam);

        public abstract void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette);

        public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            if (this.initialized)
            {
                drawPos = this.basePos;

                upDir = new Vector2(Mathf.Cos((rotation + this.baseRot) * -0.017453292f), Mathf.Sin((rotation + this.baseRot) * -0.017453292f));
                rightDir = -Custom.PerpendicularVector(upDir);
                if (flipY) upDir *= -1;
                if (flipX) rightDir *= -1;
                drawPos += upDir * headRadius;

                ChildDrawSprites(sLeaser, rCam, timeStacker, camPos);

                if (parent.culled && !parent.lastCulled)
                {
                    foreach (var sprite in sLeaser.sprites) sprite.isVisible = !parent.culled || (parent is VultureGraphics vult && vult.shadowMode);
                }
                if (base.slatedForDeletetion || rCam.room != this.room || this.room != this.parent.owner.room)
                {
                    sLeaser.CleanSpritesAndRemove();
                }
            }
        }
        protected abstract void ChildDrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos);

        public override sealed void Update(bool eu)
        {
            base.Update(eu);

            if ((this.parent.owner) == null || this.parent.owner.slatedForDeletetion || base.slatedForDeletetion)
            {
                this.Destroy();
            }
            else if (this.parent.owner.room != null && this.initialized)
            {
                ChildUpdate(eu);
            }
        }
        protected virtual void ChildUpdate(bool eu) { }

        public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            if (newContatiner == null)
            {
                newContatiner = rCam.ReturnFContainer("Items");
            }
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                sLeaser.sprites[i].RemoveFromContainer();
            }
            for (int j = 0; j < sLeaser.sprites.Length; j++)
            {
                newContatiner.AddChild(sLeaser.sprites[j]);
            }
        }
        internal void ParentDrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            if (sLeaser.sprites.Length > anchorSprite)
            {
                this.basePos = sLeaser.sprites[anchorSprite].GetPosition();
                this.baseRot = sLeaser.sprites[anchorSprite].rotation;

                this.flipX = (sLeaser.sprites[anchorSprite].scaleX > 0f);
                this.flipY = (sLeaser.sprites[anchorSprite].scaleY < 0f);

                this.initialized = true; // basePos and baseRot have been set, program can now update/draw as normal
            }
        }

        public override void Destroy()
        {
            if (effectsOn && parent.owner != null)
            {
                // Debug.Log("HatWorld: remove hat effects " + this.GetType() + " " + parent.owner);
                this.RemoveHatEffects(parent.owner as Creature);
            }
            base.Destroy();
        }

        protected virtual void AddHatEffects(Creature wearer) { }
        protected virtual void RemoveHatEffects(Creature wearer) { }
    }
}
