using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace artmdv_webapi.Areas.v2.Infrastructure
{
    public interface IFile
    {
        bool Exists(string path);
        FileStream Create(string path);
    }
}
