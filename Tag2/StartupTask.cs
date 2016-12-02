using System;
using Windows.ApplicationModel.Background;
using Windows.System.Threading;
using Windows.Devices.Gpio;

namespace Tag2
{
    public sealed class StartupTask : IBackgroundTask
    {
        private const int LedPinRed = 18;
        private const int LedPinGreen = 23;

        private GpioPin _pinRed;
        private GpioPin _pinGreen;
        private GpioPinValue _pinRedValue;
        private GpioPinValue _pinGreenValue;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            taskInstance.GetDeferral();
            InitGpio();
        }

        private void InitGpio()
        {
            var gpio = GpioController.GetDefault();

            if (gpio == null)
            {
                _pinRed = null;
                _pinGreen = null;
                return;
            }

            _pinRed = gpio.OpenPin(LedPinRed);
            _pinGreen = gpio.OpenPin(LedPinGreen);

            if (_pinRed == null || _pinGreen == null)
            {
                return;
            }

            _pinRed.SetDriveMode(GpioPinDriveMode.Output);
            _pinGreen.SetDriveMode(GpioPinDriveMode.Output);

            ThreadPoolTimer.CreatePeriodicTimer(TimerOnTick, TimeSpan.FromMilliseconds(500));
        }

        private void TimerOnTick(ThreadPoolTimer timer)
        {
            if (_pinRed == null || _pinGreen == null)
            {
                return;
            }

            if (_pinRedValue == GpioPinValue.High)
            {
                _pinRedValue = GpioPinValue.Low;
                _pinGreenValue = GpioPinValue.High;
            }
            else
            {
                _pinRedValue = GpioPinValue.High;
                _pinGreenValue = GpioPinValue.Low;
            }

            _pinRed.Write(_pinRedValue);
            _pinGreen.Write(_pinGreenValue);
        }
    }
}
