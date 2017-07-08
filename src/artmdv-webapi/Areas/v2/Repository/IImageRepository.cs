using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using artmdv_webapi.Areas.v2.Models;

namespace artmdv_webapi.Areas.v2.Repository
{
    public interface IImageRepository: IRepository
    {
        string Create(Image image, Stream imagecontent, Stream thumbContent);
        string CreateImage(Stream imageContent, string fileName);
        string CreateThumb(string filename, Stream thumbContent);
        void Delete(string id);
        Image Get(string id);
        Task<List<Image>> GetAllByTag(string tag);
        Stream GetAnnotationContent(string id);
        string GetAnnotationPath(Image image);
        Stream GetByContentId(string id);
        Stream GetImageContent(string id);
        Stream GetInvertedContent(string id);
        string GetInvertedPath(Image image);
        string GetPath(Image image);
        string GetRevisionPath(Revision image);
        Stream GetThumbContent(string id);
        Image Update(Image image);
    }
}