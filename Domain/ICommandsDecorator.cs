using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSimulator
{
    interface ICommandsDecorator : ICommands
    {
        ICommands _inner { get; }
    }
}
