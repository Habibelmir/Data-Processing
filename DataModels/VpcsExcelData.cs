namespace ProcessServices.DataModels
{
    public class VpcsExcelData
    {
        public string ReportedBy { get; set; }     // Correspond à "VPC rapporté par"
        public string CPVFocus { get; set; }       // Correspond à "Ce CPV porte sur"
        public string VPCType { get; set; }        // Correspond à "Type de VPC effectué"
    }
}
