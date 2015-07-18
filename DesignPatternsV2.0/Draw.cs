using System;
using System.Drawing;

namespace DesignPatternsV2._0
{
    public class Draw
    {
        public enum Shape
        {
            Rectangle,
            Ellipse
        }

        public Point EndPoint;
        public Shape shape;
        public Point StartPoint;

        public Draw()
        {
            
        }



        public Draw(int x1, int y1, int x2, int y2)
        {
            if (x1 < x2)
            {
                StartPoint.X = x1;
                EndPoint.X = StartPoint.X + x2 - x1;
            }
            else
            {
                StartPoint.X = x2;
                EndPoint.X = StartPoint.X + x1 - x2;
            }
            if (y1 < y2)
            {
                StartPoint.Y = y1;
                EndPoint.Y = StartPoint.Y + y2 - y1;
            }
            else
            {
                StartPoint.Y = y2;
                EndPoint.Y = StartPoint.Y + y1 - y2;
            }
        }

        public void Print()
        {
            Console.WriteLine(@"Draw");
        }

        //public Draw(Shape shape, Point p1, Point p2)
        //{
        //    if (p1.X < p2.X)
        //    {
        //        StartPoint.X = p1.X;
        //        EndPoint.X = p2.X;
        //    }
        //    else
        //    {
        //        StartPoint.X = p2.X;
        //        EndPoint.X = p1.X;
        //    }
        //    if (p1.Y < p2.Y)
        //    {
        //        StartPoint.Y = p1.Y;
        //        EndPoint.Y = p2.Y;
        //    }
        //    else
        //    {
        //        StartPoint.Y = p2.Y;
        //        EndPoint.Y = p1.Y;
        //    }
        //    //StartPoint = p1;
        //    //EndPoint = p2;
        //    this.shape = shape;
        //}

        public Rectangle GetNormalizedRectangle()
        {
            var p1 = StartPoint;
            var p2 = EndPoint;
            var normalizedRect = new Rectangle();
            if (p1.X < p2.X)
            {
                normalizedRect.X = p1.X;
                normalizedRect.Width = p2.X - p1.X;
            }
            else
            {
                normalizedRect.X = p2.X;
                normalizedRect.Width = p1.X - p2.X;
            }
            if (p1.Y < p2.Y)
            {
                normalizedRect.Y = p1.Y;
                normalizedRect.Height = p2.Y - p1.Y;
            }
            else
            {
                normalizedRect.Y = p2.Y;
                normalizedRect.Height = p1.Y - p2.Y;
            }

            return normalizedRect;
        }
    }
}