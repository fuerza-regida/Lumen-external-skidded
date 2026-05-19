namespace SkiddingApp
{
    /// <summary>
    /// Direct mirror of Offsets.hpp - Names and values strictly match the C++ dump.
    /// version-9377ee10133e4be3 updated to this version 19/05/26
    /// </summary>
    internal static class Offsets
    {
        public static class VisualEngine
        {
            public const long ViewMatrix = 0x140;
            public const long RenderView = 0xb80;
            public const long Pointer = 0x7bd51f8;
            public const long FakeDataModel = 0x1d0;
        }

        public static class DataModel
        {
            public const long PlaceId = 0x1a0;
            public const long GameId = 0x198;
            public const long GameLoaded = 0x638;
            public const long CreatorId = 0x190;
            public const long Workspace = 0x178;
            public const long ServerIP = 0x620;
            public const long ScriptContext = 0x430;
            public const long JobId = 0x138;
        }

        public static class Instance
        {
            public const long Parent = 0x70;
            public const long ClassDescriptor = 0x18;
            public const long ChildrenStart = 0x78;
            public const long ChildrenEnd = 0x8;
            public const long Name = 0xb0;
            public const long ClassName = 0x8;
        }

        public static class Player
        {
            public const long LocalPlayer = 0x138;
            public const long ModelInstance = 0x3a8;
            public const long UserId = 0x2d8;
            public const long DisplayName = 0x130;
            public const long TeamColor = 0x374;
            public const long Team = 0x2b0;
            public const long CameraMaxZoomDist = 0x330;
            public const long CameraMinZoomDist = 0x334;
        }

        public static class BasePart
        {
            public const long Primitive = 0x148;
            public const long Reflectance = 0xec;
            public const long Color3 = 0x194;
            public const long Transparency = 0xf0;
            public const long CastShadow = 0xf5;
            public const long Locked = 0xf6;
            public const long Massless = 0xf7;
        }

        public static class Primitive
        {
            public const long Position = 0xec;
            public const long CFrame = 0xc8;
            public const long Rotation = 0xc8;
            public const long Size = 0x1b8;
            public const long AssemblyLinearVelocity = 0xf8;
            public const long AssemblyAngularVelocity = 0x104;
            public const long Material = 0x236;
            public const long Shape = 0x1b1;
            public const long PrimitiveFlags = 0x1b6;
            public const long Owner = 0x200;
        }

        public static class Humanoid
        {
            public const long CameraOffset = 0x140;
            public const long UseJumpPower = 0x1ec;
            public const long AutoJumpEnabled = 0x1e0;
            public const long Health = 0x194;
            public const long MaxHealth = 0x1b4;
            public const long WalkSpeed = 0x1dc;
            public const long WalkSpeedCheck = 0x3c4;
            public const long JumpPower = 0x1b0;
            public const long JumpHeight = 0x1ac;
            public const long HipHeight = 0x1a0;
            public const long HealthDisplayDistance = 0x198;
            public const long MaxSlopeAngle = 0x1b8;
            public const long NameDisplayDistance = 0x1bc;
            public const long WalkToPoint = 0x17c;
            public const long FloorMaterial = 0x190;
            public const long WalkTimer = 0x480;
            public const long WalkToPart = 0x130;
            public const long AutoRotate = 0x1e1;
            public const long Sit = 0x1ea;
            public const long BreakJointsOnDeath = 0x1e3;
            public const long RequiresNeck = 0x1e9;
            public const long EvaluateStateMachine = 0x1e4;
            public const long RigType = 0x1cc;
            public const long TargetPoint = 0x164;
            public const long HumanoidState = 0x8a8;
            public const long HumanoidStateID = 0x20;
        }

        public static class Lighting
        {
            public const long Sky = 0x1e0;
            public const long ClockTime = 0x1c0;
            public const long Ambient = 0xe0;
            public const long EnvironmentDiffuseScale = 0x12c;
            public const long Atmosphere = 0x1f0;
            public const long EnvironmentSpecularScale = 0x130;
            public const long OutdoorAmbient = 0x110;
            public const long ColorShift_Bottom = 0xf8;
            public const long ColorShift_Top = 0xec;
        }

        public static class LightingParameters
        {
            public const long GeographicLatitude = 0x198;
        }

        public static class World
        {
            public const long FallenPartsDestroyHeight = 0x208;
            public const long Gravity = 0x210;
            public const long WorldStepPerSec = 0x678;
            public const long Primitives = 0x280;
        }

        public static class Workspace
        {
            public const long World = 0x408;
            public const long ReadOnlyGravity = 0x9e0;
            public const long CurrentCamera = 0x4b0;
        }

        public static class VectorForce
        {
            public const long ApplyAtCenterOfMass = 0x1a0;
            public const long Force = 0x190;
            public const long RelativeTo = 0x19c;
        }

        public static class DragDetector
        {
            public const long Enabled = 0x2e9;
            public const long RunLocally = 0x2eb;
            public const long MaxDragTranslation = 0x284;
            public const long MinDragTranslation = 0x290;
            public const long MaxForce = 0x2c4;
            public const long Responsiveness = 0x2d8;
            public const long DragStyle = 0x2b4;
        }

        public static class Terrain
        {
            public const long WaterColor = 0x1e8;
            public const long WaterReflectance = 0x200;
            public const long WaterTransparency = 0x204;
            public const long WaterWaveSize = 0x208;
            public const long WaterWaveSpeed = 0x20c;
            public const long GrassLength = 0x1f8;
        }

        public static class FFlag
        {
            public const long TaskSchedulerTargetFps = 0x7bfe5d0;
            public const long DebugDisableTimeoutDisconnect = 0x7760388;
            public const long PhysicsSenderMaxBandwidthBps = 0x70b740c;
            public const long EnableLoadModule = 0x7618968;
            public const long PhysicsSenderMaxBandwidthBpsScaling = 0x70b7410;
            public const long DebugSkyGray = 0x75535f8;
            public const long PartyPlayerInactivityTimeoutInSeconds = 0x70b4614;
            public const long NextGenReplicatorEnabledWrite4 = 0x7a68cd8;
        }

        public static class Camera
        {
            public const long DiagonalFieldOfView = 0x15c;
            public const long MaxAxisFieldOfView = 0xf;
            public const long FieldOfView = 0x160;
            public const long Position = 0x11c;
            public const long CFrame = 0xf8;
            public const long ViewportInt16 = 0x2ac;
            public const long ViewportSize = 0x2e8;
            public const long CameraSubject = 0xe8;
            public const long CameraType = 0x158;
        }

        public static class Script
        {
            public const long RequireBypass = 0x0;
        }

        public static class Players
        {
            public const long RespawnTime = 0x150;
        }

        public static class Model
        {
            public const long PrimaryPart = 0x278;
            public const long Scale = 0x164;
        }

        public static class Tool
        {
            public const long CanBeDropped = 0x4c8;
            public const long Enabled = 0x4c9;
            public const long ManualActivationOnly = 0x4ca;
            public const long RequiresHandle = 0x4cb;
            public const long Tooltip = 0x478;
            public const long Grip = 0x498;
            public const long GripRight = 0x498;
            public const long GripUp = 0x4a4;
            public const long GripForward = 0x4b0;
            public const long GripPos = 0x4bc;
        }

        public static class ProximityPrompt
        {
            public const long ActionText = 0xc8;
            public const long ObjectText = 0xe8;
            public const long HoldDuration = 0x138;
            public const long MaxActivationDistance = 0x140;
            public const long KeyboardKeyCode = 0x13c;
            public const long Enabled = 0x14e;
            public const long RequiresLineOfSight = 0x14f;
        }

        public static class ClickDetector
        {
            public const long MaxActivationDistance = 0x100;
        }

        public static class Misc
        {
            public const long Value = 0xd0;
            public const long StatsItemValue = 0xd0;
        }

        public static class PrimitiveFlags
        {
            public const long Anchored = 0x2;
            public const long CanCollide = 0x8;
            public const long CanTouch = 0x10;
            public const long CanQuery = 0x20;
        }

        public static class FakeDataModel
        {
            public const long Pointer = 0x74f6758;
            public const long RealDataModel = 0x1d0;
        }

        public static class GuiObject
        {
            public const long Visible = 0x5b5;
            public const long ZIndex = 0x5ac;
            public const long BackgroundColor3 = 0x548;
            public const long BackgroundTransparency = 0x56c;
            public const long BorderSizePixel = 0x574;
        }

        public static class TextLabel
        {
            public const long Text = 0xb60;
            public const long TextColor3 = 0xe58;
            public const long TextSize = 0xe84;
            public const long TextTransparency = 0xe8c;
        }

        public static class Attachment
        {
            public const long WorldCFrame = 0xb8;
            public const long WorldAxis = 0xd0;
            public const long WorldSecondaryAxis = 0xe8;
            public const long WorldPosition = 0xdc;
        }

        public static class Sound
        {
            public const long SoundId = 0xe0;
            public const long Volume = 0x148;
            public const long PlaybackSpeed = 0x134;
            public const long Looped = 0x155;
            public const long LoopRegion = 0x110;
            public const long PlaybackRegion = 0x118;
        }

        public static class BodyVelocity
        {
            public const long MaxForce = 0x2a0;
            public const long Velocity = 0x2ac;
            public const long P = 0x2b8;
        }

        public static class LinearVelocity
        {
            public const long MaxForce = 0x1b0;
            public const long VectorVelocity = 0x264;
        }

        public static class ModuleScript
        {
            public const long Bytecode = 0x140;
            public const long Hash = 0x160;
        }

        public static class LocalScript
        {
            public const long Bytecode = 0x1a8;
        }

        public static class Bytecode
        {
            public const long Size = 0x28;
            public const long Pointer = 0x10;
        }

        public static class MeshData
        {
            public const long FaceEnd = 0x38;
            public const long FaceStart = 0x30;
            public const long VertexEnd = 0x8;
            public const long VertexStart = 0x0;
        }

        public static class MeshPart
        {
            public const long MeshId = 0x2f8;
            public const long Texture = 0x328;
        }
    }
}
