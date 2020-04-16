namespace ERService.Settings.Manager
{
    public interface ICompanyInfoConfig
    {
        string CompanyCity { get; set; }
        string CompanyHouseNumber { get; set; }
        string CompanyName { get; set; }
        string CompanyNIP { get; set; }
        string CompanyPostCode { get; set; }
        string CompanyStreet { get; set; }
    }
}