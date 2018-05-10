namespace Meticulos.Api.App.Items
{
    public class ItemSearchRequest
    {
        public string TypeId { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
        public string Json { get; set; }
    }
}
