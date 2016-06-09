using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System
{
    public class Polygon : List<Point>
    {
        public Polygon()
        {
        }

        public Polygon(ICollection<Point> pts)
        {
            this.AddRange(pts);
            ConvertPoints();
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
        
        public double Area
        {
            get
            {
                ConvertPoints();
                if (this.First().Type == PointType.None)
                {
                    double area = 0.0;
                    var temp = new Point();
                    temp.X = 0;
                    temp.Y = 0;
                    for (var i = 0; i < this.Count - 1; ++i)
                        area += cross(temp, this[i], this[i + 1]);
                    area += cross(temp, this[this.Count - 1], this[0]);
                    area = area / 2.0;
                    return Math.Abs(area);
                }
                else
                {
                    var tmp = new List<Point>();
                    foreach (var x in this)
                    {
                        var t = x.ToWgsPoint();
                        var a = t.X * 111.11111111;
                        var b = 110 * Math.Cos(t.Y);
                        tmp.Add(new Point { X = a, Y = b });
                    }
                    double area = 0.0;
                    var temp = new Point();
                    temp.X = 0;
                    temp.Y = 0;
                    for (var i = 0; i < tmp.Count - 1; ++i)
                        area += cross(temp, tmp[i], tmp[i + 1]);
                    area += cross(temp, tmp[tmp.Count - 1], tmp[0]);
                    area = area / 2.0;
                    return Math.Abs(area);
                }
            }
        }

        private double cross(Point A, Point B, Point C)
        {
            return (B.X - A.X) * (C.Y - A.Y) - (B.Y - A.Y) * (C.X - A.X);
        }

        public double Circumference
        {
            get
            {
                ConvertPoints();
                double c = 0;
                for (var i = 0; i < this.Count - 1; i++)
                    c += this[i].GetDistance(this[i + 1]);
                c += this[0].GetDistance(this.Last());
                return c;
            }
        }

        public Edge GetEdge()
        {
            ConvertPoints();
            return new Edge
            {
                MaxX = this.Max(x => x.X),
                MinX = this.Min(x => x.X),
                MaxY = this.Max(x => x.Y),
                MinY = this.Min(x => x.Y)
            };
        }

        private static double isLeft(Point P0, Point P1, Point P2)
        {
            var ret = ((P1.X - P0.X) * (P2.Y - P0.Y) - (P2.X - P0.X) * (P1.Y - P0.Y));
            return ret;
        }

        public bool IsInPolygon(Point point)
        {
            ConvertPoints();
            int wn = 0, j = 0;
            for (var i = 0; i < this.LongCount(); i++)
            {
                if (i == this.LongCount() - 1)
                    j = 0;
                else
                    j = j + 1;


                if (this[i].Y <= point.Y)
                {
                    if (this[j].Y > point.Y)
                    {
                        if (isLeft(this[i], this[j], point) > 0)
                        {
                            wn++;
                        }
                    }
                }
                else
                {
                    if (this[j].Y <= point.Y)
                    {
                        if (isLeft(this[i], this[j], point) < 0)
                        {
                            wn--;
                        }
                    }
                }
            }
            if (wn == 0)
                return false;
            else
                return true;
        }
    }
}
