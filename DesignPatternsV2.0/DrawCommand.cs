using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatternsV2._0
{
    class DrawCommand : ICommand
    {
        private readonly List<GraphShape> _shapeList;
        private GraphShape.Shape _shape;
        private Point _startPoint;
        private Point _endPoint;




        public DrawCommand(List<GraphShape>shapeList, GraphShape.Shape shape, Point startPoint, Point endPoint)
        {
            _shapeList = shapeList;
            _shape = shape;
            _startPoint = startPoint;
            _endPoint = endPoint;
        }

        public void Do()
        {
            _shapeList.Add(new GraphShape(_shape,_startPoint.X,_startPoint.Y,_endPoint.X,_endPoint.Y));
        }

        public void Undo()
        {
            
        }
    }
}
