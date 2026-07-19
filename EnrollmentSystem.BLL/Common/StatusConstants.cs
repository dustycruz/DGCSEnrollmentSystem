// File: EnrollmentSystem.BLL/Common/StatusConstants.cs
namespace EnrollmentSystem.BLL.Common;

public static class ApplicationStatuses
{
    public const string Pending = "Pending";
    public const string UnderReview = "Under Review";
    public const string AdditionalRequirements = "Additional Requirements";
    public const string Approved = "Approved";       // enrollment section unlocks
    public const string Enrolled = "Enrolled";       // student account created
    public const string Rejected = "Rejected";

    public static readonly string[] All =
        { Pending, UnderReview, AdditionalRequirements, Approved, Enrolled, Rejected };
}

public static class PaymentStatuses
{
    public const string Pending = "Pending";
    public const string Verified = "Verified";
    public const string Rejected = "Rejected";
}

public static class PaymentPurposes
{
    public const string ApplicationFee = "ApplicationFee";
    public const string EnrollmentFee = "EnrollmentFee";
}

public static class EnrollmentStatuses
{
    public const string Pending = "Pending";
    public const string Enrolled = "Enrolled";
    public const string Rejected = "Rejected";
}