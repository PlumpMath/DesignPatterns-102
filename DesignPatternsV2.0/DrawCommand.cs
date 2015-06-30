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
        private readonly List<Draw> _shapeList;
        private Draw _shape;

        public DrawCommand(List<Draw>shapeList, Draw shape)
        {
            _shapeList = shapeList;
            _shape = shape;
        }

        public void Do()
        {
            _shapeList.Add(_shape);
        }

        public void Undo()
        {
            _shapeList.Remove(_shape);
        }
    }
}
