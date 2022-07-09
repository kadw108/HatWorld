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
            return new HatAbstract(world, saveData.Pos, saveData.ID, saveData.CustomData);

            /*
             * Add sandbox later
            // If this is coming from a sandbox unlock, the hue and size should depend on the data value (see CentiShieldIcon).
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
