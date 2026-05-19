using SkiddingApp.Features;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace SkiddingApp.Features
{
    public class OverlayWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int WS_EX_LAYERED = 0x00080000;
        private const int WS_EX_TOOLWINDOW = 0x00000080;

        private ESPSettings settings;
        private List<PlayerModel> playersToDraw = new List<PlayerModel>();
        private readonly object drawLock = new object();
        private SkiddingApp.MemoryManager memory;

        private Typeface typeface = new Typeface("Consolas");
        
        public OverlayWindow(ESPSettings settings, SkiddingApp.MemoryManager mem)
        {
            this.settings = settings;
            this.memory = mem;
            
            // WPF properties for a click-through transparent overlay
            this.WindowStyle = WindowStyle.None;
            this.AllowsTransparency = true;
            this.Background = Brushes.Transparent;
            this.Topmost = true;
            this.ShowInTaskbar = false;
            
            // Full screen dimensions
            this.Left = 0;
            this.Top = 0;
            this.Width = SystemParameters.PrimaryScreenWidth;
            this.Height = SystemParameters.PrimaryScreenHeight;
            
            this.Loaded += OverlayWindow_Loaded;
            CompositionTarget.Rendering += RenderTick;
        }

        private void RenderTick(object? sender, EventArgs e)
        {
            if (settings.Enabled && memory.IsConnected)
            {
                this.InvalidateVisual();
            }
            else if (!settings.Enabled)
            {
                // Clear when disabled
                this.InvalidateVisual();
            }
        }

        private void OverlayWindow_Loaded(object sender, RoutedEventArgs e)
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT | WS_EX_LAYERED | WS_EX_TOOLWINDOW);
        }

        public void UpdatePlayers(List<PlayerModel> newPlayers)
        {
            lock (drawLock)
            {
                playersToDraw = new List<PlayerModel>(newPlayers);
            }
            this.Dispatcher.InvokeAsync(() => this.InvalidateVisual());
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (!settings.Enabled || !memory.IsConnected) return;

            List<PlayerModel> playersSnapshot;
            lock (drawLock)
            {
                playersSnapshot = new List<PlayerModel>(playersToDraw);
            }

            float[] matrix = memory.GetViewMatrix();
            double screenWidth = this.Width;
            double screenHeight = this.Height;

            foreach (var player in playersSnapshot)
            {
                if (player.IsLocal) continue;
                if (player.Health <= 0 && settings.KnockCheck) continue; // If knockcheck is enabled, ignore dead. Wait, usually knock check implies ignoring knocked players.
                if (player.Health <= 0) continue; 
                if (settings.TeamCheck && player.IsSameTeam && player.Team != 0) continue;

                // Update screen pos live
                var screenPos = memory.WorldToScreen(player.WorldPosition, matrix, screenWidth, screenHeight);
                if (!screenPos.visible) continue;

                player.ScreenX = screenPos.x;
                player.ScreenY = screenPos.y;

                // Determine base color based on Team
                Color baseColor = (player.IsSameTeam && player.Team != 0) ? settings.FriendColor : settings.EnemyColor;
                SolidColorBrush baseBrush = new SolidColorBrush(baseColor);
                Pen basePen = new Pen(baseBrush, 1.5);
                
                // Scale values based on distance roughly
                double distanceScale = Math.Max(0.1, 1000.0 / (player.RawDistance > 0 ? player.RawDistance : 100.0));
                double boxWidth = 50 * distanceScale;
                double boxHeight = 100 * distanceScale;

                double x = player.ScreenX - (boxWidth / 2);
                double y = player.ScreenY - (boxHeight / 2);

                if (settings.Boxes)
                {
                    Modules.BoxESP.Draw(drawingContext, x, y, boxWidth, boxHeight, basePen, settings.FillBackground ? baseBrush : null);
                }

                if (settings.HealthBar)
                {
                    Modules.HealthESP.Draw(drawingContext, x, y, boxHeight, player.Health, player.MaxHealth, settings.HighHealthColor, settings.LowHealthColor);
                }

                if (settings.Names)
                {
                    string nameText = settings.NameType == "Username" ? player.Name : player.DisplayName;
                    Modules.NameESP.Draw(drawingContext, x, y, boxWidth, nameText, settings.NameColor, typeface);
                }

                if (settings.Distance)
                {
                    Modules.DistanceESP.Draw(drawingContext, x, y, boxWidth, boxHeight, player.Distance, settings.DistanceColor, typeface);
                }
            }
        }
    }
}


