using System.Text;

namespace StacklyBackend.Utils.DataGenerator;

enum StringType
{
    Alphanumeric,
    Numeric,
    Alphabetic
}

enum StringCase
{
    Uppercase,
    Lowercase,
    Mixed
}

class Generator
{
    public static List<int> GenerateRandomNumbers(int count, int min = 0, int max = 9, bool unique = false)
    {
        if (count <= 0)
        {
            throw new ArgumentException("Count must be greater than zero.", nameof(count));
        }

        if (min >= max)
        {
            throw new ArgumentException("Min must be less than Max.", nameof(min));
        }

        Random random = new Random();
        List<int> numbers = new List<int>();

        for (int i = 0; i < count; i++)
        {
            int num = random.Next(min, max);
            if (unique && numbers.Contains(num))
            {
                i--; // Decrement i to retry this position if the string is not unique
                continue;

            }
            numbers.Add(num);
        }

        return numbers;
    }

    public static List<float> GetRandomFloats(int count, float min = 0.0f, float max = 1.0f, bool unique = false)
    {
        if (count <= 0)
        {
            throw new ArgumentException("Count must be greater than zero.", nameof(count));
        }

        if (min >= max)
        {
            throw new ArgumentException("Min must be less than Max.", nameof(min));
        }

        Random random = new Random();
        List<float> numbers = new List<float>();

        for (int i = 0; i < count; i++)
        {
            float num = (float)(random.NextDouble() * (max - min) + min);
            if (unique && numbers.Contains(num))
            {
                i--; // Decrement i to retry this position if the string is not unique
                continue;

            }
            numbers.Add((float)(num));
        }

        return numbers;
    }

    public static string GetRandomString(StringType stringType, StringCase charCase, int length)
    {
        return GetRandomStrings(stringType, charCase, 1, length)[0];
    }

    public static List<string> GetRandomStrings(StringType stringType, StringCase charCase, int count, int length, bool unique = false)
    {
        if (count <= 0)
        {
            throw new ArgumentException("Count must be greater than zero.", nameof(count));
        }

        if (length <= 0)
        {
            throw new ArgumentException("Length must be greater than zero.", nameof(length));
        }

        const string chars = "abcdefghijklmnopqrstuvwxyz";
        const string numbers = "0123456789";

        Random random = new Random();
        List<string> strings = new List<string>();

        for (int i = 0; i < count; i++)
        {
            StringBuilder sb = new StringBuilder();
            for (int j = 0; j < length; j++)
            {
                char randomChar = chars[random.Next(0, chars.Length - 1)];
                randomChar = charCase switch
                {
                    StringCase.Uppercase => char.ToUpper(randomChar),
                    StringCase.Lowercase => char.ToLower(randomChar),
                    StringCase.Mixed => random.Next(0, 2) == 0 ? char.ToUpper(randomChar) : char.ToLower(randomChar),
                    _ => throw new ArgumentOutOfRangeException(nameof(charCase), "Invalid case specified.")
                };

                char randomNum = numbers[random.Next(0, numbers.Length - 1)];
                sb.Append(stringType switch
                {
                    StringType.Alphanumeric => random.Next(0, 2) == 0 ? randomNum : randomChar,
                    StringType.Numeric => randomNum,
                    StringType.Alphabetic => randomChar,
                    _ => throw new ArgumentOutOfRangeException(nameof(stringType), "Invalid type specified.")
                });

            }
            string result = sb.ToString();
            if (unique && strings.Contains(result))
            {
                i--; // Decrement i to retry this position if the string is not unique
                continue;

            }
            strings.Add(result);
        }

        return strings;
    }
}