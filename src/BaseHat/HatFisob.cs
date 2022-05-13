// Finished for now.
namespace HatWorld
{
    sealed class HatFisob : Fisobs.Items.Fisob
    {
        public static readonly HatFisob Instance = new HatFisob();
        private static readonly HatProperties properties = new();

        public HatFisob() : base(EnumExt_HatWorld.HatAbstract)
        {
            /*
            // Fisobs auto-loads the `icon_CentiShield` embedded resource as a texture.
            // See `CentiShields.csproj` for how you can add embedded resources to your project.

            // If you want a simple grayscale icon, you can omit the following line.
            Icon = new CentiShieldIcon();

            RegisterUnlock(EnumExt_CentiShields.OrangeCentiShield, parent: MultiplayerUnlocks.SandboxUnlockID.BigCentipede, data: 70);
            RegisterUnlock(EnumExt_CentiShields.RedCentiShield, parent: MultiplayerUnlocks.SandboxUnlockID.RedCentipede, data: 0);
            */
        }

        public override Fisobs.Properties.ItemProperties Properties(PhysicalObject forObject)
        {
            // You could create a new instance of your ItemProperties class each time here like in the Mosquitoes example,
            // but we don't need to, so we just return a static instance.
            return properties;
        }

        public override AbstractPhysicalObject Parse(World world, Fisobs.Core.EntitySaveData saveData, Fisobs.Sandbox.SandboxUnlock? unlock)
        {
            // taken from Centishields
            string[] p = saveData.CustomData.Split(';');

            if (p.Length < 9)
            {
                p = new string[9];
            }

            // MAY CHANGE LATER - see PlacedObject.DataPearlData
            // create a ConsumableObjectData from the information read out of the saveData
            // define the properties manually, because they're hardcoded in by object type and we're using a custom HatAbstract type
            PlacedObject.ConsumableObjectData consumableData = new PlacedObject.ConsumableObjectData(null);
            consumableData.panelPos.x = int.TryParse(p[3], out var xx) ? xx : 0;
            consumableData.panelPos.y = int.TryParse(p[3], out var yy) ? yy : 0;
            consumableData.minRegen = 0;
            consumableData.maxRegen = 0;

            HatAbstract returned = new HatAbstract(
                world,
                saveData.Pos,
                saveData.ID,
                int.TryParse(p[6], out var originRoom) ? originRoom : 0, // originRoom
                int.TryParse(p[7], out var placedObjectIndex) ? placedObjectIndex : 0, // placedObjectIndex
                consumableData, // consumableData
                (HatType)(int.TryParse(p[8], out var h) ? h : 0) // HatType
            );
            // world, saveData.Pos, saveData.ID, 
            /* I don't even know how to use {} with object constructors
            {
                // ID = p[0],
                // type = p[1],
                // room = p[3],
                // x = int.TryParse(p[0], out var a) ? a : 0,
                // y = int.TryParse(p[0], out var a) ? a : 0,
                hatType = (HatAbstract.HatType) (int.TryParse(p[6], out var h) ? h : 0),
                // abstractNode = p[7]??
            };
            */

            return returned;

            /*
             * Add sandbox later
            // If this is coming from a sandbox unlock, the hue and size should depend on the data value (see CentiShieldIcon below).
            if (unlock is SandboxUnlock u)
            {
                result.hue = u.Data / 1000f;

                if (u.Data == 0)
                {
                    result.scaleX += 0.2f;
                    result.scaleY += 0.2f;
                }
            }

            return result;
            */
        }
    }
}
