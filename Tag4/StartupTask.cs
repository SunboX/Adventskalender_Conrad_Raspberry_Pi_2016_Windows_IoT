using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Background;
using Windows.System.Threading;
using Windows.Devices.Gpio;

namespace Tag4
{
    public sealed class StartupTask : IBackgroundTask
    {
        private const int Z = 100; // Millisekunden

        private int _step = 0;
        private Dictionary<string, GpioPin> _gpioPins = new Dictionary<string, GpioPin>();
        private readonly Random _random = new Random();

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
                _gpioPins = new Dictionary<string, GpioPin>
                {
                    {"gpio23", null}, // Grün
                    {"gpio24", null}, // Blau
                    {"gpio25", null} // Rot
                };
                return;
            }

            _gpioPins["gpio23"] = gpio.OpenPin(23);
            _gpioPins["gpio24"] = gpio.OpenPin(24);
            _gpioPins["gpio25"] = gpio.OpenPin(25);

            if (_gpioPins["gpio23"] == null || _gpioPins["gpio24"] == null || _gpioPins["gpio25"] == null)
            {
                return;
            }

            _gpioPins["gpio23"].SetDriveMode(GpioPinDriveMode.Output);
            _gpioPins["gpio24"].SetDriveMode(GpioPinDriveMode.Output);
            _gpioPins["gpio25"].SetDriveMode(GpioPinDriveMode.Output);

            ThreadPoolTimer.CreatePeriodicTimer(TimerOnTick, TimeSpan.FromMilliseconds(Z));
        }

        private void TimerOnTick(ThreadPoolTimer timer)
        {
            if (_gpioPins["gpio23"] == null || _gpioPins["gpio24"] == null || _gpioPins["gpio25"] == null)
            {
                return;
            }

            _step++;
            if (_step == 3)
            {
                _step = 1; // beginne von vorn
            }

            switch (_step)
            {
                case 1:
                    _gpioPins["gpio" + _random.Next(23, 25)].Write(GpioPinValue.High);
                    break;

                case 2:
                    _gpioPins["gpio" + _random.Next(23, 25)].Write(GpioPinValue.Low);
                    break;
            }
        }
    }
}
