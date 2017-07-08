namespace artmdv_webapi.Areas.v2.Query

{
    public interface IQuery<TModel>
    {
        TModel Get();
    }
}