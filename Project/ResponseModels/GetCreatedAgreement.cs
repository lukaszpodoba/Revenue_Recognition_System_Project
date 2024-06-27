namespace Project.ResponseModels;

public class GetCreatedAgreement
{
    public int Id { get; set; }
    public double Price { get; set; }
    public double CurrentDeposited { get; set; }
    public DateTime PaymentFrom { get; set; }
    public DateTime PaymentUntil { get; set; }
    public bool Signed { get; set; }
    public string CurrentSoftwareVersion { get; set; }
    public DateTime EndOfVersionSupport { get; set; }
    public int ClientId { get; set; }
    public int SoftwareId { get; set; }
}