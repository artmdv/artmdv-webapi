using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data.access
{
    public interface IImagesDataAccess
    {
        Guid Save();

        FileStream Get(Guid guid);
    }
}
