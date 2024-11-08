using System.Numerics;

namespace HumJ.Iot.Ft5436
{
    public partial class Ft5436
    {
        public class TouchPoint(Span<byte> buffer)
        {
            public bool Down { get; } = (buffer[0] & 0x80) != 0;
            public bool Up { get; } = (buffer[0] & 0x40) != 0;
            public int Id { get; } = buffer[2] >> 4;

            public Vector2 Position { get; } = new Vector2(((buffer[0] & 0x0F) << 8) | buffer[1], ((buffer[2] & 0x0F) << 8) | buffer[3]);
            public int Pressure { get; } = buffer[4];
            public int Size { get; } = buffer[5] >> 4;
        }
    }
}