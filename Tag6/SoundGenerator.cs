using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media;
using Windows.Media.Audio;
using Windows.Media.Render;

namespace Tag6
{
    public sealed class SoundGenerator
    {
        private AudioGraph _audioGraph;
        private AudioFrameInputNode _frameInputNode;
        private double _theta;
        private float _freq = 1000F;

        private readonly List<SoundNote> _notes = new List<SoundNote>
        {
            new SoundNote
            {
                KeyNumber = 88,
                Notation = "C8",
                Frequency = 4186.01F
            },
            new SoundNote
            {
                KeyNumber = 87,
                Notation = "B7",
                Frequency = 3951.07F
            },
            new SoundNote
            {
                KeyNumber = 86,
                Notation = "A#7/Bb7",
                Frequency = 3729.31F
            },
            new SoundNote
            {
                KeyNumber = 85,
                Notation = "A7",
                Frequency = 3520.00F
            },
            new SoundNote
            {
                KeyNumber = 84,
                Notation = "G#7/Ab7",
                Frequency = 3322.44F
            },
            new SoundNote
            {
                KeyNumber = 83,
                Notation = "G7",
                Frequency = 3135.96F
            },
            new SoundNote
            {
                KeyNumber = 82,
                Notation = "F#7/Gb7",
                Frequency = 2959.96F
            },
            new SoundNote
            {
                KeyNumber = 81,
                Notation = "F7",
                Frequency = 2793.83F
            },
            new SoundNote
            {
                KeyNumber = 80,
                Notation = "E7",
                Frequency = 2637.02F
            },
            new SoundNote
            {
                KeyNumber = 79,
                Notation = "D#7/Eb7",
                Frequency = 2489.02F
            },
            new SoundNote
            {
                KeyNumber = 78,
                Notation = "D7",
                Frequency = 2349.32F
            },
            new SoundNote
            {
                KeyNumber = 77,
                Notation = "C#7/Db7",
                Frequency = 2217.46F
            },
            new SoundNote
            {
                KeyNumber = 76,
                Notation = "C7",
                Frequency = 2093.00F
            },
            new SoundNote
            {
                KeyNumber = 75,
                Notation = "B6",
                Frequency = 1975.53F
            },
            new SoundNote
            {
                KeyNumber = 74,
                Notation = "A#6/Bb6",
                Frequency = 1864.66F
            },
            new SoundNote
            {
                KeyNumber = 73,
                Notation = "A6",
                Frequency = 1760.00F
            },
            new SoundNote
            {
                KeyNumber = 72,
                Notation = "G#6/Ab6",
                Frequency = 1661.22F
            },
            new SoundNote
            {
                KeyNumber = 71,
                Notation = "G6",
                Frequency = 1567.98F
            },
            new SoundNote
            {
                KeyNumber = 70,
                Notation = "F#6/Gb6",
                Frequency = 1479.98F
            },
            new SoundNote
            {
                KeyNumber = 69,
                Notation = "F6",
                Frequency = 1396.91F
            },
            new SoundNote
            {
                KeyNumber = 68,
                Notation = "E6",
                Frequency = 1318.51F
            },
            new SoundNote
            {
                KeyNumber = 67,
                Notation = "D#6/Eb6",
                Frequency = 1244.51F
            },
            new SoundNote
            {
                KeyNumber = 66,
                Notation = "D6",
                Frequency = 1174.66F
            },
            new SoundNote
            {
                KeyNumber = 65,
                Notation = "C#6/Db6",
                Frequency = 1108.73F
            },
            new SoundNote
            {
                KeyNumber = 64,
                Notation = "C6",
                Frequency = 1046.50F
            },
            new SoundNote
            {
                KeyNumber = 63,
                Notation = "B5",
                Frequency = 987.767F
            },
            new SoundNote
            {
                KeyNumber = 62,
                Notation = "A#5/Bb5",
                Frequency = 932.328F
            },
            new SoundNote
            {
                KeyNumber = 61,
                Notation = "A5",
                Frequency = 880.000F
            },
            new SoundNote
            {
                KeyNumber = 60,
                Notation = "G#5/Ab5",
                Frequency = 830.609F
            },
            new SoundNote
            {
                KeyNumber = 59,
                Notation = "G5",
                Frequency = 783.991F
            },
            new SoundNote
            {
                KeyNumber = 58,
                Notation = "F#5/Gb5",
                Frequency = 739.989F
            },
            new SoundNote
            {
                KeyNumber = 57,
                Notation = "F5",
                Frequency = 698.456F
            },
            new SoundNote
            {
                KeyNumber = 56,
                Notation = "E5",
                Frequency = 659.255F
            },
            new SoundNote
            {
                KeyNumber = 55,
                Notation = "D#5/Eb5",
                Frequency = 622.254F
            },
            new SoundNote
            {
                KeyNumber = 54,
                Notation = "D5",
                Frequency = 587.330F
            },
            new SoundNote
            {
                KeyNumber = 53,
                Notation = "C#5/Db5",
                Frequency = 554.365F
            },
            new SoundNote
            {
                KeyNumber = 52,
                Notation = "C5",
                Frequency = 523.251F
            },
            new SoundNote
            {
                KeyNumber = 51,
                Notation = "B4",
                Frequency = 493.883F
            },
            new SoundNote
            {
                KeyNumber = 50,
                Notation = "A#4/Bb4",
                Frequency = 466.164F
            },
            new SoundNote
            {
                KeyNumber = 49,
                Notation = "A4",
                Frequency = 440.0F
            },
            new SoundNote
            {
                KeyNumber = 48,
                Notation = "G#4/Ab4",
                Frequency = 415.305F
            },
            new SoundNote
            {
                KeyNumber = 47,
                Notation = "G4",
                Frequency = 391.995F
            },
            new SoundNote
            {
                KeyNumber = 46,
                Notation = "F#4/Gb4",
                Frequency = 369.994F
            },
            new SoundNote
            {
                KeyNumber = 45,
                Notation = "F4",
                Frequency = 349.228F
            },
            new SoundNote
            {
                KeyNumber = 44,
                Notation = "E4",
                Frequency = 329.628F
            },
            new SoundNote
            {
                KeyNumber = 43,
                Notation = "D#4/Eb4",
                Frequency = 311.127F
            },
            new SoundNote
            {
                KeyNumber = 42,
                Notation = "D4",
                Frequency = 293.665F
            },
            new SoundNote
            {
                KeyNumber = 41,
                Notation = "C#4/Db4",
                Frequency = 277.183F
            },
            new SoundNote
            {
                KeyNumber = 40,
                Notation = "C4",
                Frequency = 261.626F
            },
            new SoundNote
            {
                KeyNumber = 39,
                Notation = "B3",
                Frequency = 246.942F
            },
            new SoundNote
            {
                KeyNumber = 38,
                Notation = "A#3/Bb3",
                Frequency = 233.082F
            },
            new SoundNote
            {
                KeyNumber = 37,
                Notation = "A3",
                Frequency = 220.000F
            },
            new SoundNote
            {
                KeyNumber = 36,
                Notation = "G#3/Ab3",
                Frequency = 207.652F
            },
            new SoundNote
            {
                KeyNumber = 35,
                Notation = "G3",
                Frequency = 195.998F
            },
            new SoundNote
            {
                KeyNumber = 34,
                Notation = "F#3/Gb3",
                Frequency = 184.997F
            },
            new SoundNote
            {
                KeyNumber = 33,
                Notation = "F3",
                Frequency = 174.614F
            },
            new SoundNote
            {
                KeyNumber = 32,
                Notation = "E3",
                Frequency = 164.814F
            },
            new SoundNote
            {
                KeyNumber = 31,
                Notation = "D#3/Eb3",
                Frequency = 155.563F
            },
            new SoundNote
            {
                KeyNumber = 30,
                Notation = "D3",
                Frequency = 146.832F
            },
            new SoundNote
            {
                KeyNumber = 29,
                Notation = "C#3/Db3",
                Frequency = 138.591F
            },
            new SoundNote
            {
                KeyNumber = 28,
                Notation = "C3",
                Frequency = 130.813F
            },
            new SoundNote
            {
                KeyNumber = 27,
                Notation = "B2",
                Frequency = 123.471F
            },
            new SoundNote
            {
                KeyNumber = 26,
                Notation = "A#2/Bb2",
                Frequency = 116.541F
            },
            new SoundNote
            {
                KeyNumber = 25,
                Notation = "A2",
                Frequency = 110.000F
            },
            new SoundNote
            {
                KeyNumber = 24,
                Notation = "G#2/Ab2",
                Frequency = 103.826F
            },
            new SoundNote
            {
                KeyNumber = 23,
                Notation = "G2",
                Frequency = 97.9989F
            },
            new SoundNote
            {
                KeyNumber = 22,
                Notation = "F#2/Gb2",
                Frequency = 92.4986F
            },
            new SoundNote
            {
                KeyNumber = 21,
                Notation = "F2",
                Frequency = 87.3071F
            },
            new SoundNote
            {
                KeyNumber = 20,
                Notation = "E2",
                Frequency = 82.4069F
            },
            new SoundNote
            {
                KeyNumber = 19,
                Notation = "D#2/Eb2",
                Frequency = 77.7817F
            },
            new SoundNote
            {
                KeyNumber = 18,
                Notation = "D2",
                Frequency = 73.4162F
            },
            new SoundNote
            {
                KeyNumber = 17,
                Notation = "C#2/Db2",
                Frequency = 69.2957F
            },
            new SoundNote
            {
                KeyNumber = 16,
                Notation = "C2",
                Frequency = 65.4064F
            },
            new SoundNote
            {
                KeyNumber = 15,
                Notation = "B1",
                Frequency = 61.7354F
            },
            new SoundNote
            {
                KeyNumber = 14,
                Notation = "A#1/Bb1",
                Frequency = 58.2705F
            },
            new SoundNote
            {
                KeyNumber = 13,
                Notation = "A1",
                Frequency = 55.0000F
            },
            new SoundNote
            {
                KeyNumber = 12,
                Notation = "G#1/Ab1",
                Frequency = 51.9131F
            },
            new SoundNote
            {
                KeyNumber = 11,
                Notation = "G1",
                Frequency = 48.9994F
            },
            new SoundNote
            {
                KeyNumber = 10,
                Notation = "F#1/Gb1",
                Frequency = 46.2493F
            },
            new SoundNote
            {
                KeyNumber = 9,
                Notation = "F1",
                Frequency = 43.6535F
            },
            new SoundNote
            {
                KeyNumber = 8,
                Notation = "E1",
                Frequency = 41.2034F
            },
            new SoundNote
            {
                KeyNumber = 7,
                Notation = "D#1/Eb1",
                Frequency = 38.8909F
            },
            new SoundNote
            {
                KeyNumber = 6,
                Notation = "D1",
                Frequency = 36.7081F
            },
            new SoundNote
            {
                KeyNumber = 5,
                Notation = "C#1/Db1",
                Frequency = 34.6478F
            },
            new SoundNote
            {
                KeyNumber = 4,
                Notation = "C1",
                Frequency = 32.7032F
            },
            new SoundNote
            {
                KeyNumber = 3,
                Notation = "B0",
                Frequency = 30.8677F
            },
            new SoundNote
            {
                KeyNumber = 2,
                Notation = "A#0/Bb0",
                Frequency = 29.1352F
            },
            new SoundNote
            {
                KeyNumber = 1,
                Notation = "A0",
                Frequency = 27.5000F
            },
            new SoundNote
            {
                Notation = "G#0/Ab0",
                Frequency = 25.9565F
            },
            new SoundNote
            {
                Notation = "G0",
                Frequency = 24.4997F
            },
            new SoundNote
            {
                Notation = " 	F#0/Gb0",
                Frequency = 23.1247F
            },
            new SoundNote
            {
                Notation = "F0",
                Frequency = 21.8268F
            },
            new SoundNote
            {
                Notation = "E0",
                Frequency = 20.6017F
            },
            new SoundNote
            {
                Notation = "D#0/Eb0",
                Frequency = 19.4454F
            },
            new SoundNote
            {
                Notation = "D0",
                Frequency = 18.3540F
            },
            new SoundNote
            {
                Notation = "C#0/Db0",
                Frequency = 17.3239F
            },
            new SoundNote
            {
                Notation = "C0",
                Frequency = 16.3516F
            }
        };

        private Timer _timer;

        internal async Task Initialize()
        {
            // Create an AudioGraph with default settings
            var settings = new AudioGraphSettings(AudioRenderCategory.Media);
            var result = await AudioGraph.CreateAsync(settings);

            if (result.Status != AudioGraphCreationStatus.Success)
            {
                // Cannot create graph
                Debug.WriteLine(string.Format("AudioGraph Creation Error because {0}", result.Status.ToString()));
                return;
            }

            _audioGraph = result.Graph;

            // Create a device output node
            var deviceOutputResult = await _audioGraph.CreateDeviceOutputNodeAsync();

            if (deviceOutputResult.Status != AudioDeviceNodeCreationStatus.Success)
            {
                // Cannot create device output
                Debug.WriteLine(string.Format("Audio Device Output unavailable because {0}",
                    deviceOutputResult.Status.ToString()));
                return;
            }

            var deviceOutputNode = deviceOutputResult.DeviceOutputNode;
            Debug.WriteLine("Device Output Node successfully created");

            CreateFrameInputNode();
            _frameInputNode.AddOutgoingConnection(deviceOutputNode);
        }

        public IAsyncAction InitializeAction()
        {
            return Initialize().AsAsyncAction();
        }

        /// <summary>
        /// Plays the note by notation.
        /// </summary>
        /// <param name="notation">The notation.</param>
        /// <param name="duration">The duration in milliseconds.</param>
        public void PlayNoteByNotation(string notation, int duration)
        {
            var note = _notes.Find(n => n.Notation == notation);
            PlayNote(note, duration);
        }

        /// <summary>
        /// Plays the note by by key number.
        /// </summary>
        /// <param name="keyNumber">The key number.</param>
        /// <param name="duration">The duration in milliseconds.</param>
        public void PlayNoteByByKeyNumber(int keyNumber, int duration)
        {
            var note = _notes.Find(n => n.KeyNumber == keyNumber);
            PlayNote(note, duration);
        }

        /// <summary>
        /// Plays a note.
        /// </summary>
        /// <param name="note">The note.</param>
        /// <param name="duration">The duration in milliseconds.</param>
        /// <exception cref="Exception">Not initialized</exception>
        public void PlayNote(SoundNote note, int duration)
        {
            if (_audioGraph == null || _frameInputNode == null || note == null)
            {
                throw new Exception("Not initialized");
            }

            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;

                _audioGraph.Stop();
                _frameInputNode.Stop();
                _freq = 1000F;
                _theta = 0;
            }

            _freq = note.Frequency;
            _audioGraph.Start();
            _frameInputNode.Start();

            _timer = new Timer(OnTimeout, null, duration, Timeout.Infinite);
        }

        private void OnTimeout(object state)
        {
            if (_audioGraph == null || _frameInputNode == null)
            {
                throw new Exception("Not initialized");
            }

            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }

            _audioGraph.Stop();
            _frameInputNode.Stop();
            _freq = 1000F;
            _theta = 0;
        }

        private void CreateFrameInputNode()
        {
            // Create the FrameInputNode at the same format as the graph, except explicitly set mono.
            var nodeEncodingProperties = _audioGraph.EncodingProperties;
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
            var numSamplesNeeded = (uint)args.RequiredSamples;

            if (numSamplesNeeded == 0)
            {
                return;
            }
            var audioData = GenerateAudioData(numSamplesNeeded);
            _frameInputNode.AddFrame(audioData);
        }

        private unsafe AudioFrame GenerateAudioData(uint samples)
        {
            // Buffer size is (number of samples) * (size of each sample)
            // We choose to generate single channel (mono) audio. For multi-channel, multiply by number of channels
            var bufferSize = samples * sizeof(float);
            var frame = new AudioFrame(bufferSize);

            using (var buffer = frame.LockBuffer(AudioBufferAccessMode.Write))
            using (var reference = buffer.CreateReference())
            {
                byte* dataInBytes;
                uint capacityInBytes;

                // Get the buffer from the AudioFrame
                ((IMemoryBufferByteAccess)reference).GetBuffer(out dataInBytes, out capacityInBytes);

                // Cast to float since the data we are generating is float
                var dataInFloat = (float*)dataInBytes;

                const float amplitude = 0.3f;
                var sampleRate = (int)_audioGraph.EncodingProperties.SampleRate;
                var sampleIncrement = _freq * (Math.PI * 2) / sampleRate;

                // Generate the sine wave and populate the values in the memory buffer
                for (var i = 0; i < samples; i++)
                {
                    var sinValue = amplitude * Math.Sin(_theta);
                    dataInFloat[i] = (float)sinValue;
                    _theta += sampleIncrement;
                }
            }

            return frame;
        }

        [ComImport]
        [Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private unsafe interface IMemoryBufferByteAccess
        {
            void GetBuffer(out byte* buffer, out uint capacity);
        }
    }
}