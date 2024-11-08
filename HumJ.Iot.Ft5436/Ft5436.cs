using System.Device.Gpio;
using System.Device.I2c;

namespace HumJ.Iot.Ft5436
{
    public partial class Ft5436 : IDisposable
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

            gpio.Write(reset, PinValue.Low);
            gpio.Write(reset, PinValue.High);

            gpio.RegisterCallbackForPinValueChangedEvent(interrupt, PinEventTypes.Falling, OnInterrupt);
        }

        public void Dispose()
        {
            OnTouch = null;

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
    }
}