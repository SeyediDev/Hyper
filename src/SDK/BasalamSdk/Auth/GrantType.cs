namespace Basalam.SDK.Auth;

public enum GrantType
{
    ClientCredentials,
    AuthorizationCode,
    PersonalToken
}

public enum Scope
{
    All = 0,
    CustomerProfileRead = 1,
    CustomerProfileWrite = 2,
    VendorProfileRead = 4,
    VendorProfileWrite = 8,
    ProductRead = 16,
    ProductWrite = 32,
    ParcelRead = 64,
    ParcelWrite = 128,
    OrderRead = 256,
    OrderWrite = 512,
    ChatRead = 1024,
    ChatWrite = 2048
}
