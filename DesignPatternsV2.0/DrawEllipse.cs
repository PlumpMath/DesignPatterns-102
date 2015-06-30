using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatternsV2._0
{
    class DrawEllipse : Draw
    {
        public DrawEllipse(int x1, int y1, int x2, int y2) : base(x1, y1, x2, y2)
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
    }
}
