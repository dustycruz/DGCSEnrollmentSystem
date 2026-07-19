// File: EnrollmentSystem.BLL/Common/PaymentInstructionSettings.cs
namespace EnrollmentSystem.BLL.Common;

public class PaymentInstructionSettings
{
    public string BankName { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string GcashNumber { get; set; } = string.Empty;
    public string ApplicationFee { get; set; } = string.Empty;
    public string EnrollmentFee { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}