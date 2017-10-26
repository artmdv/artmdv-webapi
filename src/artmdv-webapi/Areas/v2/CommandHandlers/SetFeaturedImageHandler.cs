using System;
using System.Threading.Tasks;
using artmdv_webapi.Areas.v2.Command;
using artmdv_webapi.Areas.v2.Models;
using artmdv_webapi.Areas.v2.Repository;

namespace artmdv_webapi.Areas.v2.CommandHandlers
{
    public class SetFeaturedImageHandler: IHandler<SetFeaturedImageCommand, object>
    {
        public IFeaturedImageRepository Repository { get; }

        public SetFeaturedImageHandler(IFeaturedImageRepository repository)
        {
            Repository = repository;
        }
        
        public Task<object> HandleAsync(SetFeaturedImageCommand model)
        {
            var image = new FeaturedImage
            {
                Date = DateTime.Now,
                ImageId = model.Id
            };

            Repository.Save(image);

            return Task.FromResult<object>(null);
        }
    }
}
