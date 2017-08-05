using System.IO;

namespace artmdv_webapi.Areas.v2.Infrastructure
{
    public class LocalFile: IFile
    {
        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public FileStream Create(string path)
        {
            return File.Create(path);
        }
    }
}
