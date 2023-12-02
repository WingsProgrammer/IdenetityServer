namespace MS.Services.Identitty.Domain.Shared.Models;

public class VerifyCode
{
    public long Id { get; set; }
    public string MobileNumber { get; set; }
    public string VerifyCodeValue { get; set; }
    public bool Used { get; set; }
    public string? SendDate { get; set; }
    public string? SendStatus { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime ExpireDate { get; set; }

    public static string GenerateVerifyCode()
    {
        Random rand = new();
        var generate = rand.Next(11111,99999);
        return generate.ToString();
    }
}