using System;
using artmdv_webapi.Areas.v2.Models;
using artmdv_webapi.Areas.v2.Repository;

namespace artmdv_webapi.Areas.v2.Command
{
    public class SetFeaturedImageHandler: IHandler<SetFeaturedImageCommand>
    {
        public IFeaturedImageRepository Repository { get; }

        public SetFeaturedImageHandler(IFeaturedImageRepository repository)
        {
            Repository = repository;
        }
        
        public void Handle(SetFeaturedImageCommand model)
        {
            var image = new FeaturedImage
            {
                Date = DateTime.Now,
                ImageId = model.Id
            };

            Repository.Save(image);
        }
    }
}
