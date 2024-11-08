namespace HumJ.Iot.Ft5436
{
    public partial class Ft5436
    {
        public class TouchEvent
        {
            public int Status { get; }
            public int PointCount { get; }
            public TouchPoint[] Points { get; }

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
    }
}