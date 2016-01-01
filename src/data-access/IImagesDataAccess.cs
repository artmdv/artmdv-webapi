using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using contracts;
using MongoDB.Bson;

namespace data_access
{
    public interface IImagesDataAccess
    {
        string Save(Stream fileStream, string filename);

        Image Get(string id);
        Task<IList<Image>> GetAll();
        void Delete(string id);
    }
}
