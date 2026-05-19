using System;
using System.Collections.Generic;

namespace SkiddingApp.Features
{
    public class ChamsManager
    {
        private readonly MemoryManager memory;

        public bool Enabled { get; set; }
        public float Transparency { get; set; } = 0.35f;
        public (float r, float g, float b) Color = (1f, 0f, 1f);
        public bool TeamCheck { get; set; } = true;

        public ChamsManager(MemoryManager memory)
        {
            this.memory = memory;
        }

        public void Update(List<PlayerModel> players)
        {
            if (!Enabled || !memory.IsConnected) return;

            foreach (var player in players)
            {
                if (player.IsLocal || player.Health <= 0) continue;
                if (TeamCheck && player.IsSameTeam) continue;

                ApplyChams(player.Address);
            }
        }

        private void ApplyChams(long playerInstance)
        {
            long character = memory.ReadInt64(playerInstance + Offsets.Player.ModelInstance);
            if (character == 0) return;

            var queue = new Queue<long>();
            queue.Enqueue(character);

            while (queue.Count > 0)
            {
                long instance = queue.Dequeue();
                foreach (var child in memory.GetChildren(instance))
                {
                    queue.Enqueue(child);
                }

                string className = memory.GetClassName(instance);
                if (className != "Part" && className != "MeshPart" && className != "UnionOperation" &&
                    className != "WedgePart" && className != "CornerWedgePart" && className != "TrussPart")
                {
                    continue;
                }

                memory.WriteVector3(instance + Offsets.BasePart.Color3, Color.r, Color.g, Color.b);
                memory.WriteFloat(instance + Offsets.BasePart.Transparency, Transparency);
            }
        }
    }
}
