namespace Project.ResponseModels;

public class GetCreatedPayment
{
    public int Id { get; set; }
    public double DepositSize { get; set; }
    public DateTime PaymentDate { get; set; }
    public int ClientId { get; set; }
    public int AgreementId { get; set; }
    public double AlreadyPaid { get; set; }
    public double AgreementPrice { get; set; }
}