using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace artmdv_webapi.Areas.v2.Infrastructure
{
    public class LocalDirectory:IDirectory
    {
        public bool Exists(string path)
        {
            return Directory.Exists(path);
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }
    }
}
