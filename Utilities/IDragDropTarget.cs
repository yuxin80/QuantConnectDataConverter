using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoilSimulater.Utilities
{
    interface IDragDropTarget
    {
        Boolean CanDrop(object item);

        void Drop(object item);

        void CopyDrop(object item);
    }
}
