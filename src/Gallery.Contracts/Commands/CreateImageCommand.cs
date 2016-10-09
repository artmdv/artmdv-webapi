using System.IO;

namespace Gallery.Contracts.Commands
{
    public class CreateImageCommand: CommandBase
    {
        public FileStream Image { get; set; }
        public string Title { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
    }
}
