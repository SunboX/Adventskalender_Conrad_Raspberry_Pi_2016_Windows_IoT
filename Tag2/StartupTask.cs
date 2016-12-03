using System;
using Windows.ApplicationModel.Background;
using Windows.System.Threading;
using Windows.Devices.Gpio;

namespace Tag2
{
    public sealed class StartupTask : IBackgroundTask
    {
        private GpioPin _gpio18;
        private GpioPin _gpio23;
        private GpioPinValue _gpio18Value;
        private GpioPinValue _gpio23Value;

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
                _gpio18 = null;
                _gpio23 = null;
                return;
            }

            _gpio18 = gpio.OpenPin(18);
            _gpio23 = gpio.OpenPin(23);

            if (_gpio18 == null || _gpio23 == null)
            {
                return;
            }

            _gpio18.SetDriveMode(GpioPinDriveMode.Output);
            _gpio23.SetDriveMode(GpioPinDriveMode.Output);

            ThreadPoolTimer.CreatePeriodicTimer(TimerOnTick, TimeSpan.FromMilliseconds(500));
        }

        private void TimerOnTick(ThreadPoolTimer timer)
        {
            if (_gpio18 == null || _gpio23 == null)
            {
                return;
            }

            if (_gpio18Value == GpioPinValue.High)
            {
                _gpio18Value = GpioPinValue.Low;
                _gpio23Value = GpioPinValue.High;
            }
            else
            {
                _gpio18Value = GpioPinValue.High;
                _gpio23Value = GpioPinValue.Low;
            }

            _gpio18.Write(_gpio18Value);
            _gpio23.Write(_gpio23Value);
        }
    }
}
