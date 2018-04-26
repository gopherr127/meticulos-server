namespace Meticulos.Api.App.ItemTypes
{
    public class ItemTypeSearchRequest
    {
        public string Name { get; set; }
        public bool? IsForPysicalItems { get; set; }
        public bool? AllowNestedItems { get; set; }
    }
}
