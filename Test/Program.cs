using HumJ.Iot.Ft5436;
using System.Device.Gpio;
using System.Device.I2c;

var reset = 4;
var interrupt = 17;

var gpio = new GpioController();
var i2c = I2cDevice.Create(new I2cConnectionSettings(1, Ft5436.I2cAddress));

var ft5436 = new Ft5436(i2c, gpio, reset, interrupt);
ft5436.OnTouch += (sender, touchEvent) =>
{
    Console.Clear();
    Console.WriteLine($"Time: {DateTime.Now}");
    Console.WriteLine($"Status: 0x{touchEvent.Status:X6}");
    Console.WriteLine($"PointCount: {touchEvent.PointCount,-8}");
    foreach (var point in touchEvent.Points)
    {
        Console.WriteLine($"{point.Down,8}{point.Up,8}{point.X,8}{point.Y,8}{point.Pressure,8}{point.Size,8}");
    }
};