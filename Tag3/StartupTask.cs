using System;
using Windows.ApplicationModel.Background;
using Windows.System.Threading;
using Windows.Devices.Gpio;

namespace Tag3
{
    public sealed class StartupTask : IBackgroundTask
    {
        private const int Z = 250; // Millisekunden

        private GpioPin _gpio18; // Rot
        private GpioPin _gpio23; // Grün
        private GpioPin _gpio24; // Blau

        private int _step = 0;

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
                _gpio24 = null;
                return;
            }

            _gpio18 = gpio.OpenPin(18);
            _gpio23 = gpio.OpenPin(23);
            _gpio24 = gpio.OpenPin(24);

            if (_gpio18 == null || _gpio23 == null || _gpio24 == null)
            {
                return;
            }

            _gpio18.SetDriveMode(GpioPinDriveMode.Output);
            _gpio23.SetDriveMode(GpioPinDriveMode.Output);
            _gpio24.SetDriveMode(GpioPinDriveMode.Output);

            _gpio18.Write(GpioPinValue.Low);
            _gpio23.Write(GpioPinValue.Low);
            _gpio24.Write(GpioPinValue.High);

            ThreadPoolTimer.CreatePeriodicTimer(TimerOnTick, TimeSpan.FromMilliseconds(Z));
        }

        private void TimerOnTick(ThreadPoolTimer timer)
        {
            if (_gpio18 == null || _gpio23 == null || _gpio24 == null)
            {
                return;
            }

            _step++;
            if (_step == 7)
            {
                _step = 1; // beginne von vorn
            }

            switch (_step)
            {
                case 1:
                    _gpio18.Write(GpioPinValue.High);
                    _gpio23.Write(GpioPinValue.Low);
                    _gpio24.Write(GpioPinValue.High);
                    break;

                case 2:
                    _gpio18.Write(GpioPinValue.High);
                    _gpio23.Write(GpioPinValue.Low);
                    _gpio24.Write(GpioPinValue.Low);
                    break;

                case 3:
                    _gpio18.Write(GpioPinValue.High);
                    _gpio23.Write(GpioPinValue.High);
                    _gpio24.Write(GpioPinValue.Low);
                    break;

                case 4:
                    _gpio18.Write(GpioPinValue.Low);
                    _gpio23.Write(GpioPinValue.High);
                    _gpio24.Write(GpioPinValue.Low);
                    break;

                case 5:
                    _gpio18.Write(GpioPinValue.Low);
                    _gpio23.Write(GpioPinValue.High);
                    _gpio24.Write(GpioPinValue.High);
                    break;

                case 6:
                    _gpio18.Write(GpioPinValue.Low);
                    _gpio23.Write(GpioPinValue.Low);
                    _gpio24.Write(GpioPinValue.High);
                    break;
            }
        }
    }
}
