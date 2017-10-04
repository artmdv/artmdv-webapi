using System.IO;

namespace artmdv_webapi.Areas.v2.Infrastructure
{
    public interface IFile
    {
        bool Exists(string path);
        FileStream Create(string path);
        FileStream Open(string path);
        void Delete(string imageFilename);
    }
}
