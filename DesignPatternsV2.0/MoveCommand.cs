using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatternsV2._0
{
    class MoveCommand : ICommand
    {
        private readonly List<Draw> _shapeList;
        private Draw _shape;
        private Point _startMousePoint;
        private Point _endMousePoint;
        private Point _startMoveMousePoint;


        public MoveCommand(List<Draw>shapeList, Draw shape, Point startMousePoint, Point endMousePoint)
        {
            _shapeList = shapeList;
            _shape = shape;
            _startMousePoint = startMousePoint;
            _endMousePoint = endMousePoint;
        }

        public void Do()
        {

            _shape.StartPoint = new Point(_shape.StartPoint.X + _endMousePoint.X - _startMousePoint.X,
                        _shape.StartPoint.Y + _endMousePoint.Y - _startMousePoint.Y);
            _shape.EndPoint = new Point(_shape.EndPoint.X + _endMousePoint.X - _startMousePoint.X,
                _shape.EndPoint.Y + _endMousePoint.Y - _startMousePoint.Y);

        }

        public void Undo()
        {
            _shape.StartPoint = new Point(_shape.StartPoint.X - _endMousePoint.X + _startMousePoint.X,
                        _shape.StartPoint.Y - _endMousePoint.Y + _startMousePoint.Y);
            _shape.EndPoint = new Point(_shape.EndPoint.X - _endMousePoint.X + _startMousePoint.X,
                _shape.EndPoint.Y - _endMousePoint.Y + _startMousePoint.Y);
        }
    }
}
