﻿using System.IO;

namespace artmdv_webapi.Areas.v2.Infrastructure
{
    public class LocalFile: IFile
    {
        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public FileStream Create(string path)
        {
            return File.Create(path);
        }

        public FileStream Open(string path)
        {
            return File.Open(path, FileMode.Open, FileAccess.Read);
        }

        public void Delete(string filename)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
        }
    }
}
