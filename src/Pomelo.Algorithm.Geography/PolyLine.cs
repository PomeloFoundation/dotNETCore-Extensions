using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System
{
    public class PolyLine : List<Point>
    {
        public PolyLine()
        {
        }

        public PolyLine(ICollection<Point> pts)
        {
            this.AddRange(pts);
            ConvertPoints();
        }

        public double Circumference
        {
            get
            {
                ConvertPoints();
                double c = 0;
                for (var i = 0; i < this.Count - 1; i++)
                    c += this[i].GetDistance(this[i + 1]);
                return c;
            }
        }

        private void ConvertPoints()
        {
            if (this.Count > 0)
            {
                var first = this.First();
                for (var i = 0; i < this.Count; i++)
                {
                    if (this[i].Type != first.Type)
                    {
                        if (first.Type == PointType.None)
                            throw new NotSupportedException($"不支持{this[i].Type}与{first.Type}混合构成折线");
                        else if (first.Type == PointType.WGS)
                            this[i] = this[i].ToWgsPoint();
                        else if (first.Type == PointType.GCJ)
                            this[i] = this[i].ToGcjPoint();
                        else
                            this[i] = this[i].ToBaiduPoint();
                    }
                }
            }
        }

        public Point Begin { get { return this.First(); } }
        public Point End { get { return this.Last(); } }
    }
}
