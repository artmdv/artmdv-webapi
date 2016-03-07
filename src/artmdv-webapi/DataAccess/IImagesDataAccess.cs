using System.Collections.Generic;
using System.IO;
using artmdv_webapi.Models;

namespace artmdv_webapi.DataAccess
{
    public interface IImagesDataAccess
    {
        string SaveImage(Stream fileStream, string filename);
        string Create(AstroImage image);
        Image Get(string id);
        IList<AstroImage> GetAll();
        void Delete(string id);
        void DeleteImage(string id);
    }
}
