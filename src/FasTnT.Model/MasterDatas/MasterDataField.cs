namespace FasTnT.Model.MasterDatas
{
    public class MasterDataField
    {
        public int Id { get; set; }
        public int? InternalParentId { get; set; }
        public string MasterdataId { get; set; }
        public string MasterdataType { get; set; }
        public string ParentId { get; set; }
        public string Name { get; set; }
        public string Namespace { get; set; }
        public string Value { get; set; }
    }
}
