﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace artmdv_webapi.Areas.v2.Infrastructure
{
    public interface IDirectory
    {
        bool Exists(string path);
        void CreateDirectory(string path);
    }
}
