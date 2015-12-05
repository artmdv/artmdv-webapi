using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data.access
{
    internal class ImagesMongo : MongoBase, IImagesDataAccess
    {
        public FileStream Get(Guid guid)
        {
            throw new NotImplementedException();
        }

        public Guid Save()
        {
            throw new NotImplementedException();
        }
    }
}
