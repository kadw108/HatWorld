// Finished for now.

using Fisobs;
using Fisobs.Core;

namespace HatWorld
{
    // based on abstractdatapearl
    public class HatAbstract : AbstractPhysicalObject
    {
        public HatType hatType;
        
        public HatAbstract(World world, WorldCoordinate pos, EntityID ID, HatType hatType) : base(world, HatFisob.Instance.Type, null, pos, ID) {
            this.hatType = hatType;
        }

        /*
        AbstractDataPearl inherits from AbstractConsumable so I'll probably have to make that change eventually
        For now stick with AbstractPhysicalObject

        public HatAbstract(World world, WorldCoordinate pos, EntityID ID, int originRoom,
            int placedObjectIndex, PlacedObject.ConsumableObjectData consumableData, HatType hatType) :
            base(world, HatFisob.Instance.Type, null, pos, ID, originRoom, placedObjectIndex, consumableData) {

            this.hatType = hatType;
        } */

        public override string ToString()
        {
            string hatTypeString = ((int) this.hatType).ToString();
            return this.SaveToString(hatTypeString);

            // see CentiShields and DataPearl.AbstractDataPearl for reference
            /*
            object[] array = new object[17];
            array[0] = this.ID.ToString();
            array[1] = "<oA>";
            array[2] = this.type.ToString();
            array[3] = "<oA>";
            array[4] = this.pos.room;
            array[5] = ".";
            array[6] = this.pos.x;
            array[7] = ".";
            array[8] = this.pos.y;
            array[9] = ".";
            array[10] = this.pos.abstractNode;
            array[11] = "<oA>";
            array[12] = this.originRoom;
            array[13] = "<oA>";
            array[14] = this.placedObjectIndex;
            array[15] = "<oA>";
            int num = 16;
            int num2 = (int)this.dataPearlType;
            array[num] = num2.ToString();
            return string.Concat(array);
            */


            /*
           object[] array = new object[13];
            array[0] = this.ID.ToString();
            array[1] = ";";
            array[2] = this.type.ToString();
            array[3] = ";";
            array[4] = this.pos.room;
            array[5] = ";";
            array[6] = this.pos.x;
            array[7] = ";";
            array[8] = this.pos.y;
            array[9] = ";";
            array[10] = ((int) this.hatType).ToString();
            array[11] = ";";
            array[12] = this.pos.abstractNode;
            return string.Concat(array);
            */

            /* 
             * Version with AbstractConsumable
            object[] array = new object[17];
            array[0] = this.ID.ToString();
            array[1] = ";";
            array[2] = this.type.ToString();
            array[3] = ";";
            array[4] = this.pos.room;
            array[5] = ";";
            array[6] = this.pos.x;
            array[7] = ";";
            array[8] = this.pos.y;
            array[9] = ";";
            array[10] = ((int) this.hatType).ToString();
            array[11] = ";";
            array[12] = this.originRoom;
            array[13] = ";";
            array[14] = this.placedObjectIndex;
            array[15] = ";";
            array[16] = this.pos.abstractNode;
            return string.Concat(array);
            */
        }

        public override void Realize()
        {
            if (realizedObject == null)
            {
                switch (this.hatType)
                {
                    case HatType.Santa:
                        realizedObject = new SantaHatPhysical(this, this.world);
                        break;

                    case HatType.Wizard:
                        realizedObject = new WizardHatPhysical(this, this.world);
                        break;

                    case HatType.Torch:
                        realizedObject = new TorchHatPhysical(this, this.world);
                        break;
                        
                    default:
                        realizedObject = new SantaHatPhysical(this, this.world);
                        break;
                }
            }

            base.Realize();
        }
    }
}
