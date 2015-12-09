using System;
using System.IO;
using MongoDB.Bson;

namespace data_access
{
    public interface IImagesDataAccess
    {
        ObjectId Save(Stream fileStream, string filename);

        FileStream Get(Guid guid);
    }
}
