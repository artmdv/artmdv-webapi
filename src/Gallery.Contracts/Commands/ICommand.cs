using System;

namespace Gallery.Contracts.Commands
{
    public interface ICommand
    {
        DateTime CommandDate { get; set; }
    }
}
