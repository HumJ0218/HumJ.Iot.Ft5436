using System.Device.Gpio;
using System.Device.I2c;

namespace HumJ.Iot.Ft5436
{
    public class Ft5436 : IDisposable
    {
        public const byte I2cAddress = 0x38;

        public event EventHandler<TouchEvent>? OnTouch;

        private GpioController gpio;
        private I2cDevice i2c;

        private readonly int reset;
        private readonly int interrupt;

        private static readonly byte[] command = new byte[1];
        private byte[] buffer = new byte[64];

        public Ft5436(I2cDevice i2c, GpioController gpio, int reset, int interrupt)
        {
            this.i2c = i2c;
            this.gpio = gpio;

            this.reset = reset;
            this.interrupt = interrupt;

            gpio.OpenPin(reset, PinMode.Output);
            gpio.OpenPin(interrupt, PinMode.Input);

            gpio.Write(reset, PinValue.High);
            gpio.Write(reset, PinValue.Low);

            gpio.RegisterCallbackForPinValueChangedEvent(interrupt, PinEventTypes.Falling, OnInterrupt);
        }

        public void Dispose()
        {
            gpio.UnregisterCallbackForPinValueChangedEvent(interrupt, OnInterrupt);
            gpio.ClosePin(interrupt);
            gpio.ClosePin(reset);

            gpio.Dispose();
            i2c.Dispose();

            gpio = null!;
            i2c = null!;
            buffer = null!;

            GC.SuppressFinalize(this);
        }

        private void OnInterrupt(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
        {
            i2c.WriteRead(command, buffer);

            var touchEvent = new TouchEvent(buffer);
            OnTouch?.Invoke(this, touchEvent);
        }

        public class TouchEvent
        {
            public int Status { get; private set; }
            public int PointCount { get; private set; }
            public TouchPoint[] Points { get; private set; }

            public TouchEvent(Span<byte> buffer)
            {
                Status = (buffer[0] << 16) | (buffer[1] << 8) | buffer[2];
                PointCount = buffer[2];

                var points = new List<TouchPoint>();
                for (var i = 3; buffer[i] != 0xFF; i += 6)
                {
                    points.Add(new TouchPoint(buffer.Slice(i, 6)));
                }

                Points = [.. points];
            }
        }

        public class TouchPoint(Span<byte> buffer)
        {
            public bool Down { get; } = (buffer[0] & 0x80) != 0;
            public bool Up { get; } = (buffer[0] & 0x40) != 0;
            public int X { get; } = ((buffer[0] & 0x0F) << 8) | buffer[1];
            public int Y { get; } = (buffer[2] << 8) | buffer[3];
            public int Pressure { get; } = buffer[4];
            public int Size { get; } = buffer[5] >> 4;
        }
    }
}