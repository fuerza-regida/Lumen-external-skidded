using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;

namespace SkiddingApp.Features
{
    public class AimbotManager
    {
        private readonly MemoryManager memory;
        private readonly PlayerManager playerManager;

        public bool Enabled { get; set; }
        public string ActivationKey { get; set; } = "Mouse1";
        public float Fov { get; set; } = 120f;
        public float Smoothness { get; set; } = 8f;
        public bool TeamCheck { get; set; } = true;
        public bool Use3DTargeting { get; set; } = false;

        private bool hasStoredOriginalSubject;
        private long originalCameraSubject;

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        public AimbotManager(MemoryManager memory, PlayerManager playerManager)
        {
            this.memory = memory;
            this.playerManager = playerManager;
        }

        public void Update(List<PlayerModel> scannedPlayers)
        {
            if (!Enabled || !memory.IsConnected)
            {
                RestoreCameraSubject();
                return;
            }

            int vk = MapKeyName(ActivationKey);
            if (vk == 0 || !IsKeyDown(vk))
            {
                RestoreCameraSubject();
                return;
            }

            PlayerModel? target = FindBestTarget(scannedPlayers);
            if (target == null)
            {
                RestoreCameraSubject();
                return;
            }

            AimAtTarget(target);
        }

        private void RestoreCameraSubject()
        {
            if (!hasStoredOriginalSubject) return;

            long camera = memory.GetCurrentCamera();
            if (camera != 0 && originalCameraSubject != 0)
            {
                memory.WriteInt64(camera + Offsets.Camera.CameraSubject, originalCameraSubject);
            }

            hasStoredOriginalSubject = false;
            originalCameraSubject = 0;
        }

        private void AimAtTarget(PlayerModel target)
        {
            long camera = memory.GetCurrentCamera();
            if (camera == 0) return;

            if (!hasStoredOriginalSubject)
            {
                originalCameraSubject = memory.ReadInt64(camera + Offsets.Camera.CameraSubject);
                hasStoredOriginalSubject = originalCameraSubject != 0;
            }

            var currentCFrame = memory.ReadCFrame(camera + Offsets.Camera.CFrame);
            if (currentCFrame.Length != 16) return;

            if (!memory.TryGetRootPrimitive(memory.ReadInt64(target.Address + Offsets.Player.ModelInstance), out long targetPrim)) return;
            var targetPos = memory.ReadVector3(targetPrim + Offsets.Primitive.Position);

            var cameraPos = (x: currentCFrame[12], y: currentCFrame[13], z: currentCFrame[14]);
            var desiredMatrix = BuildLookAtMatrix(cameraPos, targetPos, (0f, 1f, 0f));
            if (desiredMatrix == null) return;

            float alpha = Math.Clamp(1f / Smoothness, 0.05f, 1f);
            float[] nextCFrame = InterpolateMatrix(currentCFrame, desiredMatrix, alpha);
            memory.WriteCFrame(camera + Offsets.Camera.CFrame, nextCFrame);
        }

        private float[]? BuildLookAtMatrix((float x, float y, float z) origin, (float x, float y, float z) target, (float x, float y, float z) upDir)
        {
            var forward = Normalize((target.x - origin.x, target.y - origin.y, target.z - origin.z));
            if (forward == (0f, 0f, 0f)) return null;

            var right = Normalize(Cross(upDir, forward));
            if (right == (0f, 0f, 0f))
            {
                right = (1f, 0f, 0f);
            }

            var up = Cross(forward, right);
            // Roblox uses the negative Z axis as forward for CFrame
            var basis = new float[16];
            basis[0] = right.x;
            basis[1] = right.y;
            basis[2] = right.z;
            basis[3] = 0f;
            basis[4] = up.x;
            basis[5] = up.y;
            basis[6] = up.z;
            basis[7] = 0f;
            basis[8] = -forward.x;
            basis[9] = -forward.y;
            basis[10] = -forward.z;
            basis[11] = 0f;
            basis[12] = origin.x;
            basis[13] = origin.y;
            basis[14] = origin.z;
            basis[15] = 1f;
            return basis;
        }

        private float[] InterpolateMatrix(float[] current, float[] target, float alpha)
        {
            var result = new float[16];
            for (int i = 0; i < 16; i++)
            {
                result[i] = current[i] + (target[i] - current[i]) * alpha;
            }
            return result;
        }

        private (float x, float y, float z) Normalize((float x, float y, float z) v)
        {
            float length = (float)Math.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
            if (length <= 1e-6f) return (0f, 0f, 0f);
            return (v.x / length, v.y / length, v.z / length);
        }

        private (float x, float y, float z) Cross((float x, float y, float z) a, (float x, float y, float z) b)
        {
            return (
                a.y * b.z - a.z * b.y,
                a.z * b.x - a.x * b.z,
                a.x * b.y - a.y * b.x
            );
        }

        private PlayerModel? FindBestTarget(List<PlayerModel> players)
        {
            float[] matrix = memory.GetViewMatrix();
            double width = SystemParameters.PrimaryScreenWidth;
            double height = SystemParameters.PrimaryScreenHeight;
            double centerX = width / 2.0;
            double centerY = height / 2.0;
            double bestScore = double.MaxValue;
            PlayerModel? bestTarget = null;

            foreach (var player in players)
            {
                if (player.IsLocal || player.Health <= 0) continue;
                if (TeamCheck && player.IsSameTeam) continue;
                if (Use3DTargeting)
                {
                    if (player.RawDistance < 0) continue;
                    double max3d = Math.Max(width, height); // fallback cap
                    // consider only reasonably close players (raw distance in world units)
                    if (player.RawDistance <= 0 || player.RawDistance > 10000) continue;
                    double score = player.RawDistance;
                    if (score < bestScore)
                    {
                        bestScore = score;
                        bestTarget = player;
                    }
                }
                else
                {
                    var screen = memory.WorldToScreen(player.WorldPosition, matrix, width, height);
                    if (!screen.visible) continue;

                    double dx = screen.x - centerX;
                    double dy = screen.y - centerY;
                    double distance = Math.Sqrt(dx * dx + dy * dy);
                    double maxDistance = (Fov / 180.0) * Math.Max(width, height) * 0.5;

                    if (distance <= maxDistance && distance < bestScore)
                    {
                        bestScore = distance;
                        bestTarget = player;
                    }
                }
            }

            return bestTarget;
        }

        private bool IsKeyDown(int vk) => (GetAsyncKeyState(vk) & 0x8000) != 0;

        public static int MapKeyName(string keyName)
        {
            if (string.IsNullOrWhiteSpace(keyName))
                return 0;

            string key = keyName.Trim().ToLowerInvariant();
            return key switch
            {
                "mouse1" or "lbutton" => 0x01,
                "mouse2" or "rbutton" => 0x02,
                "mouse3" or "mbutton" => 0x04,
                "xbutton1" => 0x05,
                "xbutton2" => 0x06,
                "space" => 0x20,
                "ctrl" => 0x11,
                "shift" => 0x10,
                "alt" => 0x12,
                "tab" => 0x09,
                _ when key.Length == 1 => char.ToUpperInvariant(key[0]),
                _ => 0,
            };
        }
    }
}
