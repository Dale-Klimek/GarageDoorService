using System;
using System.Threading.Tasks;

namespace GarageDoorService.Services
{
    using System.Threading;
    using Windows.Devices.Gpio;
    using Shared;

    class GarageDoorService : IGarageDoorService
    {
        public bool IsInitialized => Service.Initialized;

        private static readonly AsyncLock Lock = new AsyncLock();

        private const int Delay = 1000;

        private Timer _leftDoorTimer;
        private Timer _rightDoorTimer;

        public async Task<bool> Initialize()
        {
            if (IsInitialized)
                return IsInitialized;
            using (await Lock.LockAsync())
            {
                return await Service.Initialize();
            }
        }

        public async Task OpenLeftGarageDoor()
        {
            using (await Lock.LockAsync())
            {
                if (!IsInitialized)
                    await Service.Initialize();
                if (!IsInitialized)
                    return; //failing to initialize

                Service.StartLeftDoor();
                _leftDoorTimer?.Dispose();
                _leftDoorTimer = new Timer(ReleaseLeftDoor, null, Delay, Timeout.Infinite);
            }
        }

        public async Task OpenRightGarageDoor()
        {
            using (await Lock.LockAsync())
            {
                if (!IsInitialized)
                    await Service.Initialize();
                if (!IsInitialized)
                    return; //failing to initialize

                Service.StartRightDoor();
                _rightDoorTimer?.Dispose();
                _rightDoorTimer = new Timer(ReleaseRightDoor, null, Delay, Timeout.Infinite);
            }
        }

        private async void ReleaseLeftDoor(object state)
        {
            using (await Lock.LockAsync())
            {
                Service.StopLeftDoor();
            }
        }

        private async void ReleaseRightDoor(object state)
        {
            using (await Lock.LockAsync())
            {
                Service.StopRightDoor();
            }
        }

        private static class Service
        {
            private static GpioPin _leftDoor;
            private static GpioPin _rightDoor;

            private static bool _rightInitialized = false;
            private static bool _leftInitialized = false;

            public static bool Initialized { get; private set; } = false;

            public static async Task<bool> Initialize()
            {
                if (Initialized)
                    return Initialized;
                var controller = await GpioController.GetDefaultAsync();
                if (controller == null)
                    return Initialized; //No GPIO controller on device
                
                if (!_leftInitialized)
                    _leftInitialized = InitializeLeft(controller);
                if (!_rightInitialized)
                    _rightInitialized = InitializeRight(controller);
                if (_leftInitialized && _rightInitialized)
                    Initialized = true;
                return Initialized;
            }

            public static void LeftDoor(bool startOpening)
            {
                _leftDoor.Write(startOpening ? GpioPinValue.Low : GpioPinValue.High);
            }

            public static void RightDoor(bool startOpening)
            {
                _rightDoor.Write(startOpening ? GpioPinValue.Low : GpioPinValue.High);
            }

            public static void StartLeftDoor()
            {
                _leftDoor.Write(GpioPinValue.Low);
            }

            public static void StopLeftDoor()
            {
                _leftDoor.Write(GpioPinValue.High);
            }

            public static void StartRightDoor()
            {
                _rightDoor.Write(GpioPinValue.Low);
            }

            public static void StopRightDoor()
            {
                _rightDoor.Write(GpioPinValue.High);
            }

            private static bool InitializeLeft(GpioController controller)
            {
                if (!controller.TryOpenPin(2, GpioSharingMode.Exclusive, out _leftDoor, out GpioOpenStatus leftStatus))
                    return false;
                if (leftStatus != GpioOpenStatus.PinOpened)
                    return false;
                _leftDoor.SetDriveMode(GpioPinDriveMode.Output);
                _leftDoor.Write(GpioPinValue.High);
                return true;
            }

            private static bool InitializeRight(GpioController controller)
            {
                if (!controller.TryOpenPin(3, GpioSharingMode.Exclusive, out _rightDoor, out GpioOpenStatus rightStatus))
                    return false;
                if (rightStatus != GpioOpenStatus.PinOpened)
                    return false;
                _rightDoor.SetDriveMode(GpioPinDriveMode.Output);
                _rightDoor.Write(GpioPinValue.High);
                return true;
            }
        }
    }
}
