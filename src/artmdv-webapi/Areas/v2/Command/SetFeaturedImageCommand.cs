using MongoDB.Bson;

namespace artmdv_webapi.Areas.v2.Command
{
    public class SetFeaturedImageCommand: ICommand
    {
        public ObjectId Id { get; }

        public SetFeaturedImageCommand(string id)
        {
            Id = ObjectId.Parse(id);
        }
    }
}
