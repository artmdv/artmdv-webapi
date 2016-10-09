using System;

namespace Gallery.Contracts.Commands
{
    public abstract class CommandBase: ICommand
    {
        public CommandBase()
        {
            CommandDate = DateTime.Now;
        }

        public DateTime CommandDate { get; set; }
    }
}
