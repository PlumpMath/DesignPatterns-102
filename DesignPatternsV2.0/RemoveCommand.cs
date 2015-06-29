using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatternsV2._0
{
    class RemoveCommand : ICommand
    {
        private readonly List<GraphShape> _shapeList;
        private readonly GraphShape _shape;

        public RemoveCommand(List<GraphShape> shapeList, GraphShape shape)
        {
            _shapeList = shapeList;
            _shape = shape;
        }

        public void Do()
        {
            _shapeList.Remove(_shape);
        }

        public void Undo()
        {
            _shapeList.Add(_shape);
        }
    }
}
