using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatternsV2._0
{
    class RemoveCommand : ICommand
    {
        private readonly List<Draw> _shapeList;
        private readonly Draw _shape;

        public RemoveCommand(List<Draw> shapeList, Draw shape)
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
