using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using contracts;
using MongoDB.Bson;

namespace data_access
{
    public class ImagesMongo : MongoBase, IImagesDataAccess
    {
        public ImagesMongo() : base("images")
        {
        }

        public Image Get(string id)
        {
            return GetByFileName(id);
        }

        public new async Task<IList<Image>> GetAll()
        {
            return await base.GetAll();
        }

        public void Delete(string id)
        {
            base.Delete(ObjectId.Parse(id));
        }

        public string Save(Stream fileStream, string filename)
        {
            return UploadFile(fileStream, filename);
        }
    }
}
