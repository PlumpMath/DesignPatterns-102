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
        private GraphShape _shape;

        public DrawCommand(List<GraphShape>shapeList, GraphShape shape)
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
