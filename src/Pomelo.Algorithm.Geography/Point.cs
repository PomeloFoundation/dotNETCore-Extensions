using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System
{
    public class Point
    {
        public double X { get; set; }

        public double Y { get; set; }

        public Point ToWgsPoint()
        {
            if (this.Type == PointType.Baidu)
            {
                return bd_to_wgs(this);
            }
            else if (this.Type == PointType.GCJ)
            {
                return gcj_decrypt_exact(this);
            }
            else if (this.Type == PointType.WGS)
            {
                return this;
            }
            else
            {
                throw new NotSupportedException("不支持将类型为None的点转换为WGS坐标");
            }
        }

        public Point ToGcjPoint()
        {
            var p = ToWgsPoint();
            return gcj_encrypt(this);
        }

        public Point ToBaiduPoint()
        {
            var p = ToWgsPoint();
            return wgs_to_bd(p);
        }

        public double GetDistance(Point p)
        {
            if ((this.Type != PointType.None || p.Type != PointType.None) && this.Type != p.Type)
                throw new NotSupportedException("不支持这项操作");
            if (this.Type == PointType.None)
            {
                return Math.Sqrt((p.X - this.X) * (p.X - this.X) - (p.Y - this.Y) * (p.Y - this.Y));
            }
            else
            {
                var p1 = this.ToWgsPoint();
                var p2 = p.ToWgsPoint();
                return distance(p1, p2);
            }
        }

        public PointType Type { get; set; }

        private static double x_pi = Math.PI * 3000.0 / 180.0;

        private static Point Delta(double lat, double lon)
        {
            var a = 6378245.0;
            var ee = 0.00669342162296594323;
            var dLat = TransformLat(lon - 105.0, lat - 35.0);
            var dLon = TransformLon(lon - 105.0, lat - 35.0);
            var radLat = lat / 180.0 * Math.PI;
            var magic = Math.Sin(radLat);
            magic = 1 - ee * magic * magic;
            var sqrtMagic = Math.Sqrt(magic);
            dLat = (dLat * 180.0) / ((a * (1 - ee)) / (magic * sqrtMagic) * Math.PI);
            dLon = (dLon * 180.0) / (a / sqrtMagic * Math.Cos(radLat) * Math.PI);
            return new Point { X = dLon, Y = dLat };
        }

        private static double TransformLat(double x, double y)
        {
            var ret = 300.0 + x + 2.0 * y + 0.1 * x * x + 0.1 * x * y + 0.1 * Math.Sqrt(Math.Abs(x));
            ret += (20.0 * Math.Sin(6.0 * x * Math.PI) + 20.0 * Math.Sin(2.0 * x * Math.PI)) * 2.0 / 3.0;
            ret += (20.0 * Math.Sin(x * Math.PI) + 40.0 * Math.Sin(x / 3.0 * Math.PI)) * 2.0 / 3.0;
            ret += (150.0 * Math.Sin(x / 12.0 * Math.PI) + 300.0 * Math.Sin(x / 30.0 * Math.PI)) * 2.0 / 3.0;
            return ret;
        }

        private static double TransformLon(double x, double y)
        {
            var ret = 300.0 + x + 2.0 * y + 0.1 * x * x + 0.1 * x * y + 0.1 * Math.Sqrt(Math.Abs(x));
            ret += (20.0 * Math.Sin(6.0 * x * Math.PI) + 20.0 * Math.Sin(2.0 * x * Math.PI)) * 2.0 / 3.0;
            ret += (20.0 * Math.Sin(x * Math.PI) + 40.0 * Math.Sin(x / 3.0 * Math.PI)) * 2.0 / 3.0;
            ret += (150.0 * Math.Sin(x / 12.0 * Math.PI) + 300.0 * Math.Sin(x / 30.0 * Math.PI)) * 2.0 / 3.0;
            return ret;
        }

        private static Point wgs_to_bd(Point p)
        {
            var tmp = bd_encrypt(p);
            var tmp2 = gcj_encrypt(tmp);
            return tmp2;
        }
        private static Point bd_to_wgs(Point p)
        {
            var tmp = gcj_decrypt(p);
            var tmp2 = bd_decrypt(tmp);
            return tmp2;
        }

        //WGS-84 to GCJ-02
        private static Point gcj_encrypt(Point p)
        {
            if (outOfChina(p))
                return new Point { X = p.X, Y = p.Y, Type = PointType.GCJ };

            var d = Delta(p.X, p.Y);
            return new Point { X = p.X + d.X, Y = p.Y + d.Y, Type = PointType.GCJ };
        }
        //GCJ-02 to WGS-84
        private static Point gcj_decrypt(Point p)
        {
            if (outOfChina(p))
                return new Point { X = p.X, Y = p.Y, Type = PointType.WGS };

            var d = Delta(p.X, p.Y);
            return new Point { X = p.X - d.X, Y = p.Y - d.Y, Type = PointType.WGS };
        }
        //GCJ-02 to WGS-84 exactly
        private static Point gcj_decrypt_exact(Point p)
        {
            var initDelta = 0.01;
            var threshold = 0.000000001;
            var dLat = initDelta;
            var dLon = initDelta;
            var mLat = p.X - dLat;
            var mLon = p.Y - dLon;
            var pLat = p.X + dLat;
            var pLon = p.Y + dLon;
            double wgsLat, wgsLon;
            var i = 0;
            while (true)
            {
                wgsLat = (mLat + pLat) / 2;
                wgsLon = (mLon + pLon) / 2;
                var tmp = gcj_encrypt(new Point { X = wgsLon, Y = wgsLat });
                dLat = tmp.X - p.X;
                dLon = tmp.Y - p.Y;
                if ((Math.Abs(dLat) < threshold) && (Math.Abs(dLon) < threshold))
                    break;

                if (dLat > 0) pLat = wgsLat; else mLat = wgsLat;
                if (dLon > 0) pLon = wgsLon; else mLon = wgsLon;

                if (++i > 10000) break;
            }
            return new Point { X = wgsLon, Y = wgsLat, Type = PointType.WGS };
        }
        //GCJ-02 to BD-09
        private static Point bd_encrypt(Point p)
        {
            var x = p.X;
            var y = p.Y;
            var z = Math.Sqrt(x * x + y * y) + 0.00002 * Math.Sin(y * x_pi);
            var theta = Math.Atan2(y, x) + 0.000003 * Math.Cos(x * x_pi);
            var bdLon = z * Math.Cos(theta) + 0.0065;
            var bdLat = z * Math.Sin(theta) + 0.006;
            return new Point { X = bdLon, Y = bdLat, Type = PointType.Baidu };
        }
        //BD-09 to GCJ-02
        private static Point bd_decrypt(Point p)
        {
            var x = p.X - 0.0065;
            var y = p.Y - 0.006;
            var z = Math.Sqrt(x * x + y * y) - 0.00002 * Math.Sin(y * x_pi);
            var theta = Math.Atan2(y, x) - 0.000003 * Math.Cos(x * x_pi);
            var gcjLon = z * Math.Cos(theta);
            var gcjLat = z * Math.Sin(theta);
            return new Point { X = gcjLon, Y = gcjLat, Type = PointType.GCJ };
        }
        private static double distance(Point a, Point b)
        {
            var earthR = 6371000;
            var x = Math.Cos(a.Y * Math.PI / 180) * Math.Cos(b.Y * Math.PI / 180) * Math.Cos((a.X - b.X) * Math.PI / 180);
            var y = Math.Sin(a.Y * Math.PI / 180) * Math.Sin(b.Y * Math.PI / 180);
            var s = x + y;
            if (s > 1)
                s = 1;
            if (s < -1)
                s = -1;
            var alpha = Math.Acos(s);
            var distance = alpha * earthR;
            return distance;
        }
        private static bool outOfChina(Point p)
        {
            if (p.X < 72.004 || p.X > 137.8347)
                return true;
            if (p.Y < 0.8293 || p.Y > 55.8271)
                return true;
            return false;
        }
    }
}
