namespace Hyper.Domain.Features.Products;

/// <summary>
/// داده‌های اولیه برای درخت محصول و ویژگی‌ها
/// این کلاس برای seed کردن داده‌های اولیه محصولات، دسته‌بندی‌ها و ویژگی‌ها استفاده می‌شود
/// </summary>
public static class ProductTreeSeedData
{
    /// <summary>
    /// ایجاد درخت محصول و ویژگی‌ها برای یک Tenant
    /// </summary>
    public static class ProductCategories
    {
        public const string AccountServices = "AccountServices";
        public const string CardServices = "CardServices";
        public const string MoneyTransfer = "MoneyTransfer";
        public const string Facilities = "Facilities";
        public const string FacilityGrant = "FacilityGrant";
        public const string InstallmentRepayment = "InstallmentRepayment";
        public const string ValueAdded = "ValueAdded";
    }

    /// <summary>
    /// کلیدهای محصولات
    /// </summary>
    public static class Products
    {
        // خدمات حساب
        public const string AccountOpening = "AccountOpening";
        public const string AccountStatement = "AccountStatement";
        public const string AccountBalance = "AccountBalance";

        // خدمات کارت
        public const string CardIssuance = "CardIssuance";
        public const string CardReissuance = "CardReissuance";
        public const string CardCancellation = "CardCancellation";
        public const string CardPinChange = "CardPinChange";

        // انتقال وجه
        public const string CardToCard = "CardToCard";
        public const string Purchase = "Purchase";
        public const string Shaba = "Shaba";
        public const string Pal = "Pal";
        public const string AccountToAccount = "AccountToAccount";

        // اعطای تسهیلات
        public const string FacilityGrantBNPL = "FacilityGrantBNPL";
        public const string FacilityGrantKalaano = "FacilityGrantKalaano";
        public const string FacilityGrantPremium = "FacilityGrantPremium";

        // بازپرداخت اقساط
        public const string InstallmentRepaymentBNPL = "InstallmentRepaymentBNPL";
        public const string InstallmentRepaymentKalaano = "InstallmentRepaymentKalaano";
        public const string InstallmentRepaymentPremium = "InstallmentRepaymentPremium";

        // ارزش افزوده
        public const string MobileCharge = "MobileCharge";
        public const string InternetPackage = "InternetPackage";
    }

    /// <summary>
    /// کلیدهای ویژگی‌ها سطح اکوسیستم (Tenant)
    /// </summary>
    public static class TenantAttributes
    {
        public const string Amount = "Amount";
        public const string Currency = "Currency";
        public const string Profit = "Profit";
        public const string Quantity = "Quantity";
    }

    /// <summary>
    /// کلیدهای ویژگی‌ها سطح دسته‌بندی
    /// </summary>
    public static class CategoryAttributes
    {
        // خدمات حساب
        public const string AccountType = "AccountType";
        public const string MinimumBalance = "MinimumBalance";

        // خدمات کارت
        public const string CardType = "CardType";
        public const string CardNumber = "CardNumber";

        // انتقال وجه
        public const string TransferType = "TransferType";
        public const string DestinationAccount = "DestinationAccount";
        public const string DestinationShaba = "DestinationShaba";

        // تسهیلات
        public const string InstallmentProvider = "InstallmentProvider";
        public const string InstallmentAmount = "InstallmentAmount";
        public const string InstallmentCount = "InstallmentCount";
        public const string InstallmentPeriod = "InstallmentPeriod";
        public const string ThirdPartyCompany = "ThirdPartyCompany";  // شرکت سفارش‌دهنده تسهیلات (برای BNPL و کالانو)
        public const string ThirdPartyProductCode = "ThirdPartyProductCode";  // کد محصول آن شرکت

        // بازپرداخت اقساط
        public const string DueDate = "DueDate";
        public const string DaysToDueDate = "DaysToDueDate";
        public const string RepaymentStatus = "RepaymentStatus";
        public const string InstallmentNumber = "InstallmentNumber";

        // ارزش افزوده
        public const string Provider = "Provider";
        public const string ServiceType = "ServiceType";
        public const string MobileNumber = "MobileNumber";
    }

    /// <summary>
    /// مقادیر مجاز برای ویژگی‌ها
    /// </summary>
    public static class AllowedValues
    {
        public static class AccountType
        {
            public const string Digital = "Digital";
            public const string Premium = "Premium";
            public const string Normal = "Normal";
        }

        public static class CardType
        {
            public const string Normal = "Normal";
            public const string Gold = "Gold";
            public const string Platinum = "Platinum";
        }

        public static class TransferType
        {
            public const string CardToCard = "CardToCard";
            public const string Purchase = "Purchase";
            public const string Shaba = "Shaba";
            public const string Pal = "Pal";
            public const string AccountToAccount = "AccountToAccount";
        }

        public static class InstallmentProvider
        {
            public const string BNPL = "BNPL";
            public const string Kalaano = "Kalaano";
            public const string Premium = "Premium";
        }

        public static class RepaymentStatus
        {
            public const string OnTime = "0";      // پرداخت در سررسید
            public const string Early = "1";       // زودتر از موعد
            public const string Late = "-1";       // دیرکرد
        }

        public static class MobileProvider
        {
            public const string HamrahAval = "HamrahAval";
            public const string Irancell = "Irancell";
            public const string Rightel = "Rightel";
        }

        public static class ServiceType
        {
            public const string Charge = "Charge";
            public const string InternetPackage = "InternetPackage";
        }
    }
}

