using SkiddingApp;
using SkiddingApp.Features;

namespace SkiddingApp
{
    public class Functions
    {
        private MemoryManager memory;
        public Teleport Teleport { get; private set; }
        public ESP ESP { get; private set; }
        public PlayerManager PlayerManager { get; private set; }
        public ServerMonitor ServerMonitor { get; private set; }
        public AimbotManager Aimbot { get; private set; }
        public TriggerbotManager Triggerbot { get; private set; }
        public SpeedManager Speed { get; private set; }
        public ChamsManager Chams { get; private set; }

        public Functions(MemoryManager mem)
        {
            this.memory = mem;
            this.Teleport = new Teleport(mem);
            this.ESP = new ESP(mem);
            this.PlayerManager = new PlayerManager(mem);
            this.ServerMonitor = new ServerMonitor(mem);
            this.Aimbot = new AimbotManager(mem, this.PlayerManager);
            this.Triggerbot = new TriggerbotManager(mem);
            this.Speed = new SpeedManager(mem);
            this.Chams = new ChamsManager(mem);
        }

        public void TeleportTo(long targetPlayer)
        {
            Teleport.ToPlayer(targetPlayer);
        }

        public void Spectate(long targetPlayer)
        {
            long dm = memory.GetDataModel();
            if (dm == 0) return;

            long workspace = memory.ReadInt64(dm + Offsets.DataModel.Workspace);
            long camera = memory.ReadInt64(workspace + Offsets.Workspace.CurrentCamera);
            long targetChar = memory.ReadInt64(targetPlayer + Offsets.Player.ModelInstance);
            
            if (camera != 0 && targetChar != 0)
            {
                long hum = memory.FindFirstChild(targetChar, "Humanoid");
                memory.WriteInt64(camera + Offsets.Camera.CameraSubject, hum != 0 ? hum : targetChar);
            }
        }
    }
}
