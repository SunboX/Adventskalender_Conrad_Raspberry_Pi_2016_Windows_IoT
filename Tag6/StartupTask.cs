using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.System.Threading;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Media;
using Windows.Media.Audio;
using Windows.Media.MediaProperties;
using Windows.Media.Render;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Tag6
{
    public sealed class StartupTask : IBackgroundTask
    {
        private GpioPin _gpio23; // Input
        private GpioPin _gpio24; // LED
        private AudioGraph _audioGraph;
        private AudioDeviceOutputNode _deviceOutputNode;
        private AudioFrameInputNode _frameInputNode;
        private double _theta = 0;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            taskInstance.GetDeferral();
            InitGpio();
            await CreateAudioGraph();
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

        private async Task CreateAudioGraph()
        {
            // Create an AudioGraph with default settings
            AudioGraphSettings settings = new AudioGraphSettings(AudioRenderCategory.Media);
            CreateAudioGraphResult result = await AudioGraph.CreateAsync(settings);

            if (result.Status != AudioGraphCreationStatus.Success)
            {
                // Cannot create graph
                Debug.WriteLine(String.Format("AudioGraph Creation Error because {0}", result.Status.ToString()));
                return;
            }

            _audioGraph = result.Graph;

            // Create a device output node
            CreateAudioDeviceOutputNodeResult deviceOutputResult = await _audioGraph.CreateDeviceOutputNodeAsync();

            if (deviceOutputResult.Status != AudioDeviceNodeCreationStatus.Success)
            {
                // Cannot create device output
                Debug.WriteLine(String.Format("Audio Device Output unavailable because {0}", deviceOutputResult.Status.ToString()));
                return;
            }

            _deviceOutputNode = deviceOutputResult.DeviceOutputNode;
            Debug.WriteLine("Device Output Node successfully created");

            CreateFrameInputNode();
            _frameInputNode.AddOutgoingConnection(_deviceOutputNode);

            _audioGraph.Start();
        }

        private void CreateFrameInputNode()
        {
            // Create the FrameInputNode at the same format as the graph, except explicitly set mono.
            AudioEncodingProperties nodeEncodingProperties = _audioGraph.EncodingProperties;
            nodeEncodingProperties.ChannelCount = 1;
            _frameInputNode = _audioGraph.CreateFrameInputNode(nodeEncodingProperties);

            // Initialize the Frame Input Node in the stopped state
            _frameInputNode.Stop();

            // Hook up an event handler so we can start generating samples when needed
            // This event is triggered when the node is required to provide data
            _frameInputNode.QuantumStarted += node_QuantumStarted;
        }

        private void node_QuantumStarted(AudioFrameInputNode sender, FrameInputNodeQuantumStartedEventArgs args)
        {
            // GenerateAudioData can provide PCM audio data by directly synthesizing it or reading from a file.
            // Need to know how many samples are required. In this case, the node is running at the same rate as the rest of the graph
            // For minimum latency, only provide the required amount of samples. Extra samples will introduce additional latency.
            uint numSamplesNeeded = (uint)args.RequiredSamples;

            if (numSamplesNeeded != 0)
            {
                AudioFrame audioData = GenerateAudioData(numSamplesNeeded);
                _frameInputNode.AddFrame(audioData);
            }
        }

        [ComImport]
        [Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private unsafe interface IMemoryBufferByteAccess
        {
            void GetBuffer(out byte* buffer, out uint capacity);
        }

        private unsafe AudioFrame GenerateAudioData(uint samples)
        {
            // Buffer size is (number of samples) * (size of each sample)
            // We choose to generate single channel (mono) audio. For multi-channel, multiply by number of channels
            uint bufferSize = samples * sizeof(float);
            AudioFrame frame = new Windows.Media.AudioFrame(bufferSize);

            using (AudioBuffer buffer = frame.LockBuffer(AudioBufferAccessMode.Write))
            using (IMemoryBufferReference reference = buffer.CreateReference())
            {
                byte* dataInBytes;
                uint capacityInBytes;

                // Get the buffer from the AudioFrame
                ((IMemoryBufferByteAccess)reference).GetBuffer(out dataInBytes, out capacityInBytes);

                // Cast to float since the data we are generating is float
                var dataInFloat = (float*)dataInBytes;

                float freq = 1000; // choosing to generate frequency of 1kHz
                float amplitude = 0.3f;
                int sampleRate = (int)_audioGraph.EncodingProperties.SampleRate;
                double sampleIncrement = (freq * (Math.PI * 2)) / sampleRate;

                // Generate a 1kHz sine wave and populate the values in the memory buffer
                for (int i = 0; i < samples; i++)
                {
                    double sinValue = amplitude * Math.Sin(_theta);
                    dataInFloat[i] = (float)sinValue;
                    _theta += sampleIncrement;
                }
            }

            return frame;
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
