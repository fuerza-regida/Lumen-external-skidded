using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;

namespace SkiddingApp.Features
{
    public class TriggerbotManager
    {
        private readonly MemoryManager memory;

        public bool Enabled { get; set; }
        public string ActivationKey { get; set; } = "Mouse2";
        public float ActivationRadius { get; set; } = 32f;

        private bool lastTriggerFired;

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;

        public TriggerbotManager(MemoryManager memory)
        {
            this.memory = memory;
        }

        public void Update(List<PlayerModel> scannedPlayers)
        {
            if (!Enabled || !memory.IsConnected)
            {
                lastTriggerFired = false;
                return;
            }

            int vk = AimbotManager.MapKeyName(ActivationKey);
            if (vk == 0 || !IsKeyDown(vk))
            {
                lastTriggerFired = false;
                return;
            }

            var target = FindTargetUnderCrosshair(scannedPlayers);
            if (target == null)
            {
                lastTriggerFired = false;
                return;
            }

            if (!lastTriggerFired)
            {
                FireClick();
                lastTriggerFired = true;
            }
        }

        private PlayerModel? FindTargetUnderCrosshair(List<PlayerModel> players)
        {
            float[] matrix = memory.GetViewMatrix();
            double width = SystemParameters.PrimaryScreenWidth;
            double height = SystemParameters.PrimaryScreenHeight;
            double centerX = width / 2.0;
            double centerY = height / 2.0;

            foreach (var player in players)
            {
                if (player.IsLocal || player.Health <= 0) continue;

                var screen = memory.WorldToScreen(player.WorldPosition, matrix, width, height);
                if (!screen.visible) continue;

                double dx = screen.x - centerX;
                double dy = screen.y - centerY;
                if (Math.Sqrt(dx * dx + dy * dy) <= ActivationRadius)
                    return player;
            }

            return null;
        }

        private bool IsKeyDown(int vk) => (GetAsyncKeyState(vk) & 0x8000) != 0;

        private void FireClick()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
            System.Threading.Thread.Sleep(20);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);
        }
    }
}
