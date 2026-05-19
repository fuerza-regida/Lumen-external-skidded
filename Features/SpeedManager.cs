namespace SkiddingApp.Features
{
    public class SpeedManager
    {
        private readonly MemoryManager memory;

        public bool Enabled { get; set; }
        public float Speed { get; set; } = 32f;

        public SpeedManager(MemoryManager memory)
        {
            this.memory = memory;
        }

        public void Update()
        {
            if (!memory.IsConnected) return;

            long dm = memory.GetDataModel();
            if (dm == 0) return;

            long playersService = memory.FindService(dm, "Players");
            if (playersService == 0) return;

            long local = memory.ReadInt64(playersService + Offsets.Player.LocalPlayer);
            if (local == 0) return;

            long localCharacter = memory.ReadInt64(local + Offsets.Player.ModelInstance);
            if (localCharacter == 0) return;

            long humanoid = memory.FindFirstChild(localCharacter, "Humanoid");
            if (humanoid == 0) return;

            float value = Enabled ? Speed : 16f;
            memory.WriteFloat(humanoid + Offsets.Humanoid.WalkSpeed, value);
        }
    }
}
