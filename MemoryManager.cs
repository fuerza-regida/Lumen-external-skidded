using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;

namespace SkiddingApp
{
    public class MemoryManager
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        private const uint PROCESS_ALL_ACCESS = 0x1F0FFF;
        private Process? _process;
        private IntPtr _processHandle = IntPtr.Zero;
        private long _baseAddress;

        public bool IsConnected => _process != null && !_process.HasExited && _processHandle != IntPtr.Zero;

        public bool Attach(string processName)
        {
            Detach();
            var processes = Process.GetProcessesByName(processName);
            if (processes.Length == 0) return false;

            _process = processes[0];
            _processHandle = OpenProcess(PROCESS_ALL_ACCESS, false, _process.Id);
            if (_processHandle == IntPtr.Zero)
            {
                _process = null;
                return false;
            }

            try { _baseAddress = _process.MainModule?.BaseAddress.ToInt64() ?? 0; }
            catch { _baseAddress = 0; }

            return true;
        }

        public void Detach()
        {
            if (_processHandle != IntPtr.Zero)
            {
                CloseHandle(_processHandle);
                _processHandle = IntPtr.Zero;
            }
            _process = null;
        }

        public byte[] ReadBytes(long address, int size)
        {
            var buffer = new byte[size];
            if (_processHandle == IntPtr.Zero || address == 0) return new byte[0];
            if (ReadProcessMemory(_processHandle, new IntPtr(address), buffer, size, out int read) && read == size)
                return buffer;
            return new byte[0];
        }

        public long ReadInt64(long address)
        {
            var b = ReadBytes(address, 8);
            return b.Length == 8 ? BitConverter.ToInt64(b, 0) : 0;
        }

        public int ReadInt32(long address)
        {
            var b = ReadBytes(address, 4);
            return b.Length == 4 ? BitConverter.ToInt32(b, 0) : 0;
        }

        public float ReadFloat(long address)
        {
            var b = ReadBytes(address, 4);
            return b.Length == 4 ? BitConverter.ToSingle(b, 0) : 0f;
        }

        public bool WriteInt64(long address, long value) => WriteProcessMemory(_processHandle, new IntPtr(address), BitConverter.GetBytes(value), 8, out _);
        public bool WriteFloat(long address, float value) => WriteProcessMemory(_processHandle, new IntPtr(address), BitConverter.GetBytes(value), 4, out _);

        public string ReadString(long address)
        {
            if (address == 0) return string.Empty;
            long length = ReadInt64(address + 0x10);
            if (length <= 0 || length > 1024) return string.Empty;

            long dataAddr = length >= 16 ? ReadInt64(address) : address;
            byte[] data = ReadBytes(dataAddr, (int)length);
            if (data.Length == 0) return string.Empty;
            return Encoding.UTF8.GetString(data).Split('\0')[0];
        }

        public (float x, float y, float z) ReadVector3(long address)
        {
            var b = ReadBytes(address, 12);
            if (b.Length < 12) return (0, 0, 0);
            return (BitConverter.ToSingle(b, 0), BitConverter.ToSingle(b, 4), BitConverter.ToSingle(b, 8));
        }

        public bool WriteVector3(long address, float x, float y, float z)
        {
            var b = new byte[12];
            Buffer.BlockCopy(BitConverter.GetBytes(x), 0, b, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(y), 0, b, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(z), 0, b, 8, 4);
            return WriteProcessMemory(_processHandle, new IntPtr(address), b, 12, out _);
        }

        public bool WriteBytes(long address, byte[] bytes)
        {
            if (_processHandle == IntPtr.Zero || address == 0 || bytes == null || bytes.Length == 0) return false;
            return WriteProcessMemory(_processHandle, new IntPtr(address), bytes, bytes.Length, out _);
        }

        public float[] ReadCFrame(long address)
        {
            var buffer = ReadBytes(address, 64);
            if (buffer.Length < 64) return new float[0];
            float[] matrix = new float[16];
            for (int i = 0; i < 16; i++) matrix[i] = BitConverter.ToSingle(buffer, i * 4);
            return matrix;
        }

        public bool WriteCFrame(long address, float[] matrix)
        {
            if (matrix == null || matrix.Length != 16) return false;
            var buffer = new byte[64];
            for (int i = 0; i < 16; i++) Buffer.BlockCopy(BitConverter.GetBytes(matrix[i]), 0, buffer, i * 4, 4);
            return WriteBytes(address, buffer);
        }

        // --- Advanced Logic ---

        public long GetDataModel()
        {
            if (!IsConnected || _baseAddress == 0) return 0;
            long fake = ReadInt64(_baseAddress + Offsets.FakeDataModel.Pointer);
            if (fake == 0) fake = ReadInt64(_baseAddress + Offsets.VisualEngine.Pointer);
            if (fake == 0) return 0;
            return ReadInt64(fake + Offsets.FakeDataModel.RealDataModel);
        }

        public List<long> GetChildren(long instance, long offset = 0x78)
        {
            var list = new List<long>();
            if (instance == 0) return list;

            long childrenPtr = ReadInt64(instance + offset);
            if (childrenPtr == 0) return list;

            long start = ReadInt64(childrenPtr);
            long end = ReadInt64(childrenPtr + Offsets.Instance.ChildrenEnd);

            if (start == 0 || end == 0 || start >= end || (end - start) > 0x100000) return list;

            int count = (int)((end - start) / 8);
            byte[] buffer = ReadBytes(start, count * 8);
            if (buffer.Length == 0) return list;

            for (int i = 0; i < buffer.Length; i += 8)
            {
                long child = BitConverter.ToInt64(buffer, i);
                if (child != 0) list.Add(child);
            }
            return list;
        }

        public string GetClassName(long instance)
        {
            if (instance == 0) return string.Empty;
            long desc = ReadInt64(instance + Offsets.Instance.ClassDescriptor);
            if (desc == 0) return string.Empty;
            return ReadString(ReadInt64(desc + 0x8));
        }

        public long FindService(long dataModel, string name)
        {
            if (dataModel == 0) return 0;

            // Direct mapping fallback for Workspace and Players
            if (name == "Workspace") return ReadInt64(dataModel + 0x178);
            if (name == "Players")
            {
                // Robust method using LocalPlayer offset
                var children = GetChildren(dataModel);
                foreach (var child in children)
                {
                    long localPlayer = ReadInt64(child + Offsets.Player.LocalPlayer);
                    if (localPlayer != 0)
                    {
                        long parent = ReadInt64(localPlayer + Offsets.Instance.Parent);
                        if (parent == child)
                        {
                            return child;
                        }
                    }
                }
            }

            // Brute force search through common children offsets
            long[] offsets = { 0x78, 0x80, 0x50, 0xA0 };
            foreach (var offset in offsets)
            {
                var children = GetChildren(dataModel, offset);
                foreach (var child in children)
                {
                    string cName = ReadString(child + Offsets.Instance.Name);
                    string className = GetClassName(child);
                    if (cName == name || className == name)
                        return child;
                }
            }

            // Ultra deep scan of DataModel memory
            for (int i = 0x100; i < 0x600; i += 8)
            {
                long potential = ReadInt64(dataModel + i);
                if (potential != 0 && (potential % 4 == 0))
                {
                    if (ReadString(potential + Offsets.Instance.Name) == name || GetClassName(potential) == name)
                        return potential;
                }
            }

            return 0;
        }

        public long FindFirstChild(long instance, string name)
        {
            return GetChildren(instance).FirstOrDefault(c => ReadString(c + Offsets.Instance.Name) == name);
        }

        public bool TryGetRootPrimitive(long character, out long primitive)
        {
            primitive = 0;
            if (character == 0) return false;
            long primary = ReadInt64(character + Offsets.Model.PrimaryPart);
            if (primary == 0) primary = FindFirstChild(character, "HumanoidRootPart");
            if (primary != 0) primitive = ReadInt64(primary + Offsets.BasePart.Primitive);
            return primitive != 0;
        }

        public (float health, float maxHealth) GetHealth(long character)
        {
            long hum = FindFirstChild(character, "Humanoid");
            if (hum == 0) return (0, 100);
            return (ReadFloat(hum + Offsets.Humanoid.Health), ReadFloat(hum + Offsets.Humanoid.MaxHealth));
        }

        public float[] GetViewMatrix()
        {
            long visualEngine = ReadInt64(_baseAddress + Offsets.VisualEngine.Pointer);
            if (visualEngine == 0) return new float[16];
            
            long renderView = ReadInt64(visualEngine + Offsets.VisualEngine.RenderView);
            if (renderView == 0) return new float[16];

            byte[] buffer = ReadBytes(renderView + Offsets.VisualEngine.ViewMatrix, 64);
            if (buffer.Length < 64) return new float[16];

            float[] matrix = new float[16];
            for (int i = 0; i < 16; i++) matrix[i] = BitConverter.ToSingle(buffer, i * 4);
            return matrix;
        }

        public long GetCurrentCamera()
        {
            long dm = GetDataModel();
            if (dm == 0) return 0;

            long workspace = ReadInt64(dm + Offsets.DataModel.Workspace);
            if (workspace == 0) return 0;

            return ReadInt64(workspace + Offsets.Workspace.CurrentCamera);
        }

        public (float x, float y, bool visible) WorldToScreen((float x, float y, float z) pos, float[] matrix, double width, double height)
        {
            float w = matrix[3] * pos.x + matrix[7] * pos.y + matrix[11] * pos.z + matrix[15];
            if (w < 0.01f) return (0, 0, false);

            float x = matrix[0] * pos.x + matrix[4] * pos.y + matrix[8] * pos.z + matrix[12];
            float y = matrix[1] * pos.x + matrix[5] * pos.y + matrix[9] * pos.z + matrix[13];

            float nx = (x / w);
            float ny = (y / w);

            return (
                (float)((width / 2) + (nx * (width / 2))),
                (float)((height / 2) - (ny * (height / 2))),
                true
            );
        }
    }
}
