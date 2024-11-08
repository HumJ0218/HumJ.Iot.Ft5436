using HumJ.Iot.Ft5436;
using System.Device.Gpio;
using System.Device.I2c;

var reset = 4;
var interrupt = 17;

var gpio = new GpioController();
var i2c = I2cDevice.Create(new I2cConnectionSettings(1, Ft5436.I2cAddress));

var ft5436 = new Ft5436(i2c, gpio, reset, interrupt);

Console.Clear();
while (true)
{
    ft5436.OnTouch += Ft5436_OnTouch;
    Console.ReadLine();
    ft5436.OnTouch -= Ft5436_OnTouch;
}

static void Ft5436_OnTouch(object? sender, Ft5436.TouchEvent touchEvent)
{
    Console.SetCursorPosition(0, 0);
    Console.Write($"Time: {DateTime.Now}".PadRight(Console.WindowWidth));
    Console.Write($"Status: 0x{touchEvent.Status:X6}".PadRight(Console.WindowWidth));
    Console.Write($"PointCount: {touchEvent.PointCount,-8}".PadRight(Console.WindowWidth));
    foreach (var point in touchEvent.Points)
    {
        Console.Write($"{point.Id,8}{point.Down,8}{point.Up,8}{point.Position.X,8}{point.Position.Y,8}{point.Pressure,8}{point.Size,8}".PadRight(Console.WindowWidth));
    }
    Console.WriteLine("".PadRight(Console.WindowWidth * (10 - touchEvent.Points.Length)));
}