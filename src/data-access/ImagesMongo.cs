using System;
using System.IO;
using MongoDB.Bson;

namespace data_access
{
    public class ImagesMongo : MongoBase, IImagesDataAccess
    {
        public ImagesMongo(string password) : base("images")
        {
            Authorize(password);
        }

        public FileStream Get(Guid guid)
        {
            throw new NotImplementedException();
        }

        public ObjectId Save(Stream fileStream, string filename)
        {
            return UploadFile(fileStream, filename);
        }
    }
}
