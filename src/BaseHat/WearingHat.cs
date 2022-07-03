using RWCustom;
using UnityEngine;

namespace HatWorld
{
	// Hat that is being worn by slugcat
	// Uses code from FestiveWorld mod
	public abstract class WearingHat : UpdatableAndDeletable, IDrawable
	{
		public GraphicsModule parent { get; set; }
		public int anchorSprite { get; set; }
		public float rotation { get; set; }
		public float headRadius { get; set; }

		public bool flipX { get; set; }
		public bool flipY { get; set; }

		public Vector2 basePos { get; set; }
		public float baseRot { get; set; }

		public WearingHat(GraphicsModule parent, int anchorSprite, float rotation, float headRadius)
		{
			this.parent = parent;
			this.anchorSprite = anchorSprite;
			this.rotation = rotation;
			this.headRadius = headRadius;
			parent.owner.room.AddObject(this);
		}

		// method to get type of hat, required so HatWorldPlugin knows which type of hat to give back when worn hat is removed
		public abstract HatType hatType { get; }

		public abstract void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam);

		public void ParentDrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
		{
			if (sLeaser.sprites.Length > this.anchorSprite)
			{
				this.basePos = sLeaser.sprites[this.anchorSprite].GetPosition();
				this.baseRot = sLeaser.sprites[this.anchorSprite].rotation;

				this.flipX = (sLeaser.sprites[this.anchorSprite].scaleX > 0f);
				this.flipY = (sLeaser.sprites[this.anchorSprite].scaleY < 0f);
			}
		}

		public virtual void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
			if (base.slatedForDeletetion || rCam.room != this.room || this.room != this.parent.owner.room)
			{
				sLeaser.CleanSpritesAndRemove();
			}
        }

		public override void Update(bool eu)
        {
			base.Update(eu);

			GraphicsModule graphicsModule = this.parent;
			if (((graphicsModule != null) ? graphicsModule.owner : null) == null || this.parent.owner.slatedForDeletetion || base.slatedForDeletetion)
			{
				this.Destroy();
			}
			if (this.parent.owner.room != null)
            {
				ChildUpdate(eu);
            }
			/*
			if (base.slatedForDeletetion)
			{
				base.RemoveFromRoom();
			}
			*/
		}
		public abstract void ChildUpdate(bool eu);

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

		public abstract void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette);
	}
}
