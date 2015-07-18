using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatternsV2._0
{
    interface ICommand
    {
        void Do();

        void Undo();
    }
}
