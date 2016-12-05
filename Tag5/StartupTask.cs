using System;
using Windows.ApplicationModel.Background;
using Windows.System.Threading;
using Windows.Devices.Gpio;

namespace Tag5
{
    public sealed class StartupTask : IBackgroundTask
    {
        private GpioPin _gpio23; // Input
        private GpioPin _gpio24; // LED

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
                _gpio23 = null;
                _gpio24 = null;
                return;
            }
            
            _gpio23 = gpio.OpenPin(23);
            _gpio24 = gpio.OpenPin(24);

            if (_gpio23 == null || _gpio24 == null)
            {
                return;
            }
            
            _gpio23.SetDriveMode(GpioPinDriveMode.Input); // Pullup-Wiederstand an GPIO Pin ausschalten
            _gpio24.SetDriveMode(GpioPinDriveMode.Output);

            ThreadPoolTimer.CreatePeriodicTimer(TimerOnTick, TimeSpan.FromMilliseconds(200));
        }

        private void TimerOnTick(ThreadPoolTimer timer)
        {
            if (_gpio23 == null || _gpio24 == null)
            {
                return;
            }

            if (_gpio23.Read() == GpioPinValue.Low)
            {
                _gpio24.Write(GpioPinValue.High);
            }
            else
            {
                _gpio24.Write(GpioPinValue.Low);
            }
        }
    }
}
