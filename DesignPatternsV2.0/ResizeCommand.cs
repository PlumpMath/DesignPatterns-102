using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatternsV2._0
{
    class ResizeCommand : ICommand
    {
        private readonly Draw _shape;
        private readonly Point _startLocation;
        private readonly Point _endLocation;
        public ResizeCommand(Draw shape, Point startLocation, Point endLocation)
        {
            _shape = shape;
            _startLocation = startLocation;
            _endLocation = endLocation;
        }

        public void Do()
        {
            _shape.StartPoint = _endLocation;
        }

        public void Undo()
        {
            _shape.StartPoint = _startLocation;
        }
    }
}
