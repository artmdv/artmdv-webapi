using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using artmdv_webapi.Models;
using contracts;
using MongoDB.Bson;

namespace data_access
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
