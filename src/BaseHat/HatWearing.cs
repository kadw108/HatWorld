using RWCustom;
using UnityEngine;

namespace HatWorld
{
	// Hat that is being worn by slugcat
	// Uses code from FestiveWorld mod
	public abstract class HatWearing : UpdatableAndDeletable, IDrawable
	{
		public GraphicsModule parent { get; }

		public int anchorSprite = 3;
		public float rotation;
		public float headRadius = 5f;

		// for ParentDrawSprites
		public Vector2 basePos;
		public float baseRot;
		public bool flipX;
		public bool flipY;

		// for DrawSprites
		public Vector2 drawPos;
		public Vector2 upDir;
		public Vector2 rightDir;

		public bool initialized;

		public HatWearing(GraphicsModule parent)
		{
			this.parent = parent;
			this.rotation = -90f;
			this.initialized = false; // ParentDrawSprites must run to set basePos and drawPos
			parent.owner.room.AddObject(this);
		}

		public abstract void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam);

		public abstract void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette);

		public virtual void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
			if (this.initialized)
            {
                drawPos = this.basePos;

                upDir = new Vector2(Mathf.Cos((rotation + this.baseRot) * -0.017453292f), Mathf.Sin((rotation + this.baseRot) * -0.017453292f));
                rightDir = -Custom.PerpendicularVector(upDir);
                if (flipY) upDir *= -1;
                if (flipX) rightDir *= -1;
                drawPos += upDir * headRadius;

                if (base.slatedForDeletetion || rCam.room != this.room || this.room != this.parent.owner.room)
                {
                    sLeaser.CleanSpritesAndRemove();
                }

				ChildDrawSprites(sLeaser, rCam, timeStacker, camPos);
            }
        }
		public abstract void ChildDrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos);

		public override void Update(bool eu)
        {
			base.Update(eu);

			GraphicsModule graphicsModule = this.parent;
			if (((graphicsModule != null) ? graphicsModule.owner : null) == null || this.parent.owner.slatedForDeletetion || base.slatedForDeletetion)
			{
				this.Destroy();
			}
			else if (this.parent.owner.room != null && this.initialized)
            {
				ChildUpdate(eu);
            }
		}
		public virtual void ChildUpdate(bool eu) { }

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
		public void ParentDrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
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

	}
}
