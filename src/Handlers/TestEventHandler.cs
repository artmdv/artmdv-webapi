using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Gallery.Contracts.Events;

namespace Handlers
{
    public class TestEventHandler
    {
        public void Handle(TestEvent @event)
        {
            Console.WriteLine("RABBIT CONSUMER    " + @event.TestProp);
        }
    }
}
