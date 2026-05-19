using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace SkiddingApp
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Storyboard? slideInLeft;
        private DateTime startTime;
        private bool isVisible = true;
        private MemoryManager memory = new MemoryManager();
        private Functions skiddingFunctions;
        private Features.ESPSettings espSettings = new Features.ESPSettings();
        private Features.OverlayWindow overlayWindow;
        private AppConfig config;
            private DispatcherTimer saveTimer;
            private string lastSaved = "Never";
            public string LastSaved { get => lastSaved; set { if (lastSaved == value) return; lastSaved = value; OnPropertyChanged(); } }

            public List<string> AvailableKeys { get; set; } = new List<string>();

        // ObservableCollection for WPF DataBinding
        public ObservableCollection<Features.PlayerModel> PlayerList { get; set; } = new ObservableCollection<Features.PlayerModel>();

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);
        private const int VK_LCONTROL = 0xA2;
        private bool lastKeyState = false;

        public string ConfigPath => ConfigManager.ConfigFilePath;

        public bool ESPMasterEnabled
        {
            get => config.ESPMasterEnabled;
            set
            {
                if (config.ESPMasterEnabled == value) return;
                config.ESPMasterEnabled = value;
                espSettings.Enabled = value;
                OnPropertyChanged();
            }
        }

        public bool TeamCheckEnabled
        {
            get => config.TeamCheckEnabled;
            set
            {
                if (config.TeamCheckEnabled == value) return;
                config.TeamCheckEnabled = value;
                espSettings.TeamCheck = value;
                skiddingFunctions.Aimbot.TeamCheck = value;
                skiddingFunctions.Chams.TeamCheck = value;
                OnPropertyChanged();
            }
        }

        public bool KnockCheckEnabled
        {
            get => config.KnockCheckEnabled;
            set
            {
                if (config.KnockCheckEnabled == value) return;
                config.KnockCheckEnabled = value;
                espSettings.KnockCheck = value;
                OnPropertyChanged();
            }
        }

        public bool BoxesEnabled
        {
            get => config.BoxesEnabled;
            set
            {
                if (config.BoxesEnabled == value) return;
                config.BoxesEnabled = value;
                espSettings.Boxes = value;
                OnPropertyChanged();
            }
        }

        public bool FillBackground
        {
            get => config.FillBackground;
            set
            {
                if (config.FillBackground == value) return;
                config.FillBackground = value;
                espSettings.FillBackground = value;
                OnPropertyChanged();
            }
        }

        public bool HealthBarEnabled
        {
            get => config.HealthBarEnabled;
            set
            {
                if (config.HealthBarEnabled == value) return;
                config.HealthBarEnabled = value;
                espSettings.HealthBar = value;
                OnPropertyChanged();
            }
        }

        public bool NamesEnabled
        {
            get => config.NamesEnabled;
            set
            {
                if (config.NamesEnabled == value) return;
                config.NamesEnabled = value;
                espSettings.Names = value;
                OnPropertyChanged();
            }
        }

        public string NameType
        {
            get => config.NameType;
            set
            {
                if (config.NameType == value) return;
                config.NameType = value;
                espSettings.NameType = value;
                OnPropertyChanged();
            }
        }

        public bool DistanceEnabled
        {
            get => config.DistanceEnabled;
            set
            {
                if (config.DistanceEnabled == value) return;
                config.DistanceEnabled = value;
                espSettings.Distance = value;
                OnPropertyChanged();
            }
        }

        public bool AimbotEnabled
        {
            get => config.AimbotEnabled;
            set
            {
                if (config.AimbotEnabled == value) return;
                config.AimbotEnabled = value;
                skiddingFunctions.Aimbot.Enabled = value;
                OnPropertyChanged();
            }
        }

        public string AimbotKey
        {
            get => config.AimbotKey;
            set
            {
                if (config.AimbotKey == value) return;
                config.AimbotKey = value;
                skiddingFunctions.Aimbot.ActivationKey = value;
                OnPropertyChanged();
            }
        }

        public double AimbotFov
        {
            get => config.AimbotFov;
            set
            {
                if (config.AimbotFov == (float)value) return;
                config.AimbotFov = (float)value;
                skiddingFunctions.Aimbot.Fov = (float)value;
                OnPropertyChanged();
            }
        }

        public double AimbotSmoothness
        {
            get => config.AimbotSmoothness;
            set
            {
                if (config.AimbotSmoothness == (float)value) return;
                config.AimbotSmoothness = (float)value;
                skiddingFunctions.Aimbot.Smoothness = (float)value;
                OnPropertyChanged();
            }
        }

        public bool Use3DTargeting
        {
            get => config.Use3DTargeting;
            set
            {
                if (config.Use3DTargeting == value) return;
                config.Use3DTargeting = value;
                skiddingFunctions.Aimbot.Use3DTargeting = value;
                OnPropertyChanged();
            }
        }

        public bool TriggerbotEnabled
        {
            get => config.TriggerbotEnabled;
            set
            {
                if (config.TriggerbotEnabled == value) return;
                config.TriggerbotEnabled = value;
                skiddingFunctions.Triggerbot.Enabled = value;
                OnPropertyChanged();
            }
        }

        public string TriggerbotKey
        {
            get => config.TriggerbotKey;
            set
            {
                if (config.TriggerbotKey == value) return;
                config.TriggerbotKey = value;
                skiddingFunctions.Triggerbot.ActivationKey = value;
                OnPropertyChanged();
            }
        }

        public double TriggerbotRadius
        {
            get => config.TriggerbotRadius;
            set
            {
                if (config.TriggerbotRadius == (float)value) return;
                config.TriggerbotRadius = (float)value;
                skiddingFunctions.Triggerbot.ActivationRadius = (float)value;
                OnPropertyChanged();
            }
        }

        public bool SpeedEnabled
        {
            get => config.SpeedEnabled;
            set
            {
                if (config.SpeedEnabled == value) return;
                config.SpeedEnabled = value;
                skiddingFunctions.Speed.Enabled = value;
                OnPropertyChanged();
            }
        }

        public double SpeedValue
        {
            get => config.SpeedValue;
            set
            {
                if (config.SpeedValue == (float)value) return;
                config.SpeedValue = (float)value;
                skiddingFunctions.Speed.Speed = (float)value;
                OnPropertyChanged();
            }
        }

        public bool ChamsEnabled
        {
            get => config.ChamsEnabled;
            set
            {
                if (config.ChamsEnabled == value) return;
                config.ChamsEnabled = value;
                skiddingFunctions.Chams.Enabled = value;
                OnPropertyChanged();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            config = ConfigManager.Load();
            startTime = DateTime.Now;
            skiddingFunctions = new Functions(memory);
            ApplyConfig();
            
            if (Application.Current.TryFindResource("SlideInLeft") is Storyboard sb) slideInLeft = sb;

            // Initialize and show the Overlay
            overlayWindow = new Features.OverlayWindow(espSettings, memory);
            overlayWindow.Show();

            // Input Handling Timer
            var keyTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
            keyTimer.Tick += (s, e) => {
                bool isKeyDown = (GetAsyncKeyState(VK_LCONTROL) & 0x8000) != 0;
                if (isKeyDown && !lastKeyState) ToggleVisibility();
                lastKeyState = isKeyDown;
            };
            keyTimer.Start();

            // Uptime Timer
            var uptimeTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            uptimeTimer.Tick += (s, e) => {
                TimeSpan uptime = DateTime.Now - startTime;
                UptimeText.Text = string.Format("{0:D2}:{1:D2}:{2:D2}", uptime.Hours, uptime.Minutes, uptime.Seconds);
            };
            uptimeTimer.Start();

            // Save settings when the window closes
            this.Closing += MainWindow_Closing;

            // Setup autosave debounce timer
            saveTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            saveTimer.Tick += (s, e) => {
                saveTimer.Stop();
                ConfigManager.Save(config);
                LastSaved = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            };

            // Populate available keys for ComboBoxes
            AvailableKeys = new List<string> { "Mouse1", "Mouse2", "Space", "Ctrl", "Shift", "Alt" };
            for (char c = 'A'; c <= 'Z'; c++) AvailableKeys.Add(c.ToString());
            for (char n = '0'; n <= '9'; n++) AvailableKeys.Add(n.ToString());

            // Start Advanced Background Loop (Tutorial Style)
            StartAdvancedLoop();
        }

        private void ApplyConfig()
        {
            espSettings.Enabled = config.ESPMasterEnabled;
            espSettings.TeamCheck = config.TeamCheckEnabled;
            espSettings.KnockCheck = config.KnockCheckEnabled;
            espSettings.Boxes = config.BoxesEnabled;
            espSettings.FillBackground = config.FillBackground;
            espSettings.HealthBar = config.HealthBarEnabled;
            espSettings.Names = config.NamesEnabled;
            espSettings.NameType = config.NameType;
            espSettings.Distance = config.DistanceEnabled;

            skiddingFunctions.Aimbot.Enabled = config.AimbotEnabled;
            skiddingFunctions.Aimbot.ActivationKey = config.AimbotKey;
            skiddingFunctions.Aimbot.Fov = config.AimbotFov;
            skiddingFunctions.Aimbot.Smoothness = config.AimbotSmoothness;
            skiddingFunctions.Aimbot.TeamCheck = config.TeamCheckEnabled;
            skiddingFunctions.Aimbot.Use3DTargeting = config.Use3DTargeting;

            skiddingFunctions.Triggerbot.Enabled = config.TriggerbotEnabled;
            skiddingFunctions.Triggerbot.ActivationKey = config.TriggerbotKey;
            skiddingFunctions.Triggerbot.ActivationRadius = config.TriggerbotRadius;

            skiddingFunctions.Speed.Enabled = config.SpeedEnabled;
            skiddingFunctions.Speed.Speed = config.SpeedValue;

            skiddingFunctions.Chams.Enabled = config.ChamsEnabled;
            skiddingFunctions.Chams.TeamCheck = config.TeamCheckEnabled;

            OnPropertyChanged(nameof(ESPMasterEnabled));
            OnPropertyChanged(nameof(TeamCheckEnabled));
            OnPropertyChanged(nameof(KnockCheckEnabled));
            OnPropertyChanged(nameof(BoxesEnabled));
            OnPropertyChanged(nameof(FillBackground));
            OnPropertyChanged(nameof(HealthBarEnabled));
            OnPropertyChanged(nameof(NamesEnabled));
            OnPropertyChanged(nameof(NameType));
            OnPropertyChanged(nameof(DistanceEnabled));
            OnPropertyChanged(nameof(AimbotEnabled));
            OnPropertyChanged(nameof(AimbotKey));
            OnPropertyChanged(nameof(AimbotFov));
            OnPropertyChanged(nameof(AimbotSmoothness));
            OnPropertyChanged(nameof(TriggerbotEnabled));
            OnPropertyChanged(nameof(TriggerbotKey));
            OnPropertyChanged(nameof(TriggerbotRadius));
            OnPropertyChanged(nameof(SpeedEnabled));
            OnPropertyChanged(nameof(SpeedValue));
            OnPropertyChanged(nameof(ChamsEnabled));
            OnPropertyChanged(nameof(AvailableKeys));
            OnPropertyChanged(nameof(LastSaved));
        }

        private void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            ConfigManager.Save(config);
        }

        private async void StartAdvancedLoop()
        {
            while (true)
            {
                try
                {
                    if (!memory.IsConnected)
                    {
                        if (memory.Attach("RobloxPlayerBeta"))
                        {
                            Application.Current.Dispatcher.Invoke(() => {
                                ConnectionStatus.Text = "Connected";
                                ConnectionStatus.Foreground = System.Windows.Media.Brushes.LightGreen;
                            });
                        }
                    }

                    if (memory.IsConnected)
                    {
                        long dataModel = memory.GetDataModel();
                        if (dataModel != 0)
                        {
                            // Update PlaceID on UI thread
                            var serverInfo = skiddingFunctions.ServerMonitor.GetInfo();
                            Application.Current.Dispatcher.Invoke(() => PlaceIDText.Text = serverInfo.PlaceId.ToString());

                            // Perform Heavy Memory Scanning on Background Thread
                            var scannedPlayers = await Task.Run(() => {
                                var players = skiddingFunctions.PlayerManager.UpdatePlayers();
                                skiddingFunctions.Speed.Update();
                                skiddingFunctions.Chams.Update(players);
                                skiddingFunctions.Aimbot.Update(players);
                                skiddingFunctions.Triggerbot.Update(players);
                                return players;
                            });

                            // Sync back to UI thread (Communication with WPF)
                            Application.Current.Dispatcher.Invoke(() => {
                                // Send updated player list to the OverlayWindow
                                overlayWindow.UpdatePlayers(scannedPlayers);

                                // Sync Collections
                                PlayerListItems.ItemsSource = scannedPlayers.OrderBy(x => x.Name).ToList();
                                if (NearbyPlayersList != null)
                                    NearbyPlayersList.ItemsSource = scannedPlayers.Where(x => x.RawDistance >= 0 && x.RawDistance < 2000).OrderBy(x => x.RawDistance).Take(8).ToList();
                                
                                PlayerCountText.Text = scannedPlayers.Count.ToString();
                            });
                        }
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(() => {
                            ConnectionStatus.Text = "Disconnected";
                            ConnectionStatus.Foreground = System.Windows.Media.Brushes.Red;
                        });
                    }
                }
                catch { }

                await Task.Delay(1000); // 1 second refresh rate as per tutorial suggestion
            }
        }



        private void ToggleVisibility()
        {
            if (isVisible)
            {
                DoubleAnimation anim = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
                anim.Completed += (s, e) => this.Hide();
                this.BeginAnimation(Window.OpacityProperty, anim);
            }
            else
            {
                this.Show();
                this.Activate();
                this.BeginAnimation(Window.OpacityProperty, new DoubleAnimation(1, TimeSpan.FromSeconds(0.2)));
            }
            isVisible = !isVisible;
        }

        private void Spectate_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b && b.Tag is long target)
            {
                skiddingFunctions.Spectate(target);
            }
        }

        private void Teleport_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b && b.Tag is long target)
            {
                skiddingFunctions.TeleportTo(target);
            }
        }

        private void ProcessVisuals(List<SkiddingApp.Features.PlayerModel> scannedPlayers)
        {
            // Logic moved to OverlayWindow for 60FPS rendering
        }

        private bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private bool SetConfig<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            // schedule debounced save for config changes
            try { if (saveTimer != null) { saveTimer.Stop(); saveTimer.Start(); } } catch { }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void Tab_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton btn)
            {
                slideInLeft?.Begin(ContentHost);
                string tab = btn.Content.ToString() ?? string.Empty;
                if (HomeView != null) HomeView.Visibility = tab == "Home" ? Visibility.Visible : Visibility.Collapsed;
                if (PlayersView != null) PlayersView.Visibility = tab == "Players" ? Visibility.Visible : Visibility.Collapsed;
                if (VisualsView != null) VisualsView.Visibility = tab == "Visuals" ? Visibility.Visible : Visibility.Collapsed;
                if (AimbotView != null) AimbotView.Visibility = tab == "Aimbot" ? Visibility.Visible : Visibility.Collapsed;
                if (MiscView != null) MiscView.Visibility = tab == "Misc" ? Visibility.Visible : Visibility.Collapsed;
                if (SettingsView != null) SettingsView.Visibility = tab == "Settings" ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) { if (e.LeftButton == MouseButtonState.Pressed) DragMove(); }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            ConfigManager.Save(config);
            LastSaved = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void ReloadSettings_Click(object sender, RoutedEventArgs e)
        {
            config = ConfigManager.Load();
            ApplyConfig();
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();
    }
}