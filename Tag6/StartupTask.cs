using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.System.Threading;
using Windows.Devices.Gpio;

namespace Tag6
{
    public sealed class StartupTask : IBackgroundTask
    {
        private const int T = 500;

        private GpioPin _gpio18; // Input
        private GpioPin _gpio23; // LED Rot
        private GpioPin _gpio24; // LED Grün
        private GpioPin _gpio25; // LED Blau

        private SoundGenerator _generator;
        private int _step = 0;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            taskInstance.GetDeferral();
            InitGpio();
            _generator = new SoundGenerator();
            await _generator.Initialize();
        }

        private void InitGpio()
        {
            var gpio = GpioController.GetDefault();

            if (gpio == null)
            {
                _gpio18 = null;
                _gpio23 = null;
                _gpio24 = null;
                _gpio25 = null;
                return;
            }

            _gpio18 = gpio.OpenPin(18);
            _gpio23 = gpio.OpenPin(23);
            _gpio24 = gpio.OpenPin(24);
            _gpio25 = gpio.OpenPin(25);

            if (_gpio18 == null || _gpio23 == null || _gpio24 == null || _gpio25 == null)
            {
                return;
            }

            _gpio18.SetDriveMode(GpioPinDriveMode.Input); // Pullup-Wiederstand an GPIO Pin ausschalten
            _gpio23.SetDriveMode(GpioPinDriveMode.Output);
            _gpio24.SetDriveMode(GpioPinDriveMode.Output);
            _gpio25.SetDriveMode(GpioPinDriveMode.Output);

            ThreadPoolTimer.CreatePeriodicTimer(TimerOnTick, TimeSpan.FromMilliseconds(T + 500));
        }
        
        private void TimerOnTick(ThreadPoolTimer timer)
        {
            if (_gpio18 == null || _gpio23 == null || _gpio24 == null || _gpio25 == null)
            {
                return;
            }
            
            _gpio23.Write(GpioPinValue.Low);
            _gpio24.Write(GpioPinValue.Low);
            _gpio25.Write(GpioPinValue.Low);

            if (_gpio18.Read() == GpioPinValue.Low)
            {
                _step++;
                if (_step == 4)
                {
                    _step = 1; // beginne von vorn
                }

                switch (_step)
                {
                    case 1:
                        _gpio23.Write(GpioPinValue.High);
                        _generator.PlayNoteByByKeyNumber(60, T);
                        break;

                    case 2:
                        _gpio24.Write(GpioPinValue.High);
                        _generator.PlayNoteByByKeyNumber(64, T);
                        break;

                    case 3:
                        _gpio25.Write(GpioPinValue.High);
                        _generator.PlayNoteByByKeyNumber(67, T);
                        break;
                }
            }
        }
    }
}
