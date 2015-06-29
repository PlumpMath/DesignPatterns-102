using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatternsV2._0
{
    class MoveCommand
    {
        private readonly List<GraphShape> _shapeList;
        private GraphShape _shape;
        private Point _startShapePoint;
        private Point _endShapePoint;
        private Point _startMoveMousePoint;


        public MoveCommand(List<GraphShape>shapeList, GraphShape shape, Point startShapePoint, Point endShapePoint, Point startMoveMousePoint)
        {
            _shapeList = shapeList;
            _shape = shape;
            _startShapePoint = startShapePoint;
            _endShapePoint = endShapePoint;
            _startMoveMousePoint = startMoveMousePoint;
        }

        public void Do()
        {
            _shape.StartPoint = _endShapePoint;
        }

        public void Undo()
        {
            
        }
    }
}
