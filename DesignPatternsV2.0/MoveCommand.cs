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

        public GraphShape Shape;
        public Point StartShapePoint;
        public Point EndShapePoint;
        public Point StartMoveMousePoint;

        private GraphShape _shape;

        public MoveCommand(GraphShape shape)
        {
            
            _shape = shape;

        }

        public void Do()
        {
            
        }

        public void Undo()
        {
            
        }
    }
}
