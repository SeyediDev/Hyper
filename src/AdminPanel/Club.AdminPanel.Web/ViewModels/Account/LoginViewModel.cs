namespace Hyper.AdminPanel.Web.ViewModels.Account;

public class LoginViewModel
{
    //[Required]
    //[Display(Name = "نام کاربری")]
    public string? UserName { get; set; }

    public int CountryCode { get; set; } = 98;

    //[Required]
    //[DataType(DataType.Password)]
    //[Display(Name = "کلمه عبور")]
    public string? Password { get; set; }

    //[Display(Name = "من را به خاطر بسپار?")]
    public bool RememberMe { get; set; }
}

public class VerifyLoginViewModel
{
    //[Required]
    //[Display(Name = "نام کاربری")]
    public string? UserName { get; set; }

    public int CountryCode { get; set; } = 98;

    public string? Code { get; set; }
    //[Display(Name = "من را به خاطر بسپار?")]
    public bool RememberMe { get; internal set; }
}
