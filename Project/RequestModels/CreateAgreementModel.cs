namespace Project.RequestModels;

public class CreateAgreementModel
{ 
    public DateTime PaymentFrom { get; set; }
    public DateTime PaymentUntil { get; set; }
    public int YearsOfVersionSupport { get; set; }
}