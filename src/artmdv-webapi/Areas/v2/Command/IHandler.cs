namespace artmdv_webapi.Areas.v2.Command
{
    public interface IHandler<TModel>
    {
        void Handle(TModel model);
    }
}
