using System.Text;
using System.Text.RegularExpressions;

namespace Hyper.Domain.Features.Rewards;

public interface ISerialGenerator
{
    public List<string> GenerateSerials(string template, int count);
    public string GenerateSerial(string template, int counter);
}

public class SerialGenerator : ISerialGenerator
{
    private static readonly Random _random = new Random();
    private const string UppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string LowercaseLetters = "abcdefghijklmnopqrstuvwxyz";
    private const string Digits = "0123456789";
    private const string Alphanumeric = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    public List<string> GenerateSerials(string template, int count)
    {
        List<string> results = [];

        for (int i = 1; i <= count; i++)
        {
            results.Add(GenerateSerial(template, i));
        }

        return results;
    }

    public string GenerateSerial(string template, int counter)
    {
        // الگوی regex برای شناسایی {Xn} و {!}
        var pattern = @"\{(\w)(\d*)\}|\{!\}";
        var regex = new Regex(pattern);

        return regex.Replace(template, match =>
        {
            if (match.Value == "{!}")
            {
                // شمارنده
                return counter.ToString();
            }
            else
            {
                // استخراج نوع کاراکتر و تعداد
                char charType = match.Groups[1].Value[0];
                string countStr = match.Groups[2].Value;

                int count = string.IsNullOrEmpty(countStr) ? 1 : int.Parse(countStr);

                return GenerateDynamicPart(charType, count);
            }
        });
    }

    private static string GenerateDynamicPart(char charType, int count)
    {
        var sb = new StringBuilder();
        string sourceChars = charType switch
        {
            '#' => Digits,
            '@' => UppercaseLetters,
            '&' => LowercaseLetters,
            '*' => Alphanumeric,
            _ => throw new ArgumentException($"نوع کاراکتر نامعتبر: {charType}")
        };

        for (int i = 0; i < count; i++)
        {
            sb.Append(sourceChars[_random.Next(sourceChars.Length)]);
        }

        return sb.ToString();
    }
}