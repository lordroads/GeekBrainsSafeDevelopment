namespace CardStorageService.Utils.IndexWords;

public class Lexer
{
    public IEnumerable<string> GetTokens(string text)
    {
        int start = -1;

        for (int i = 0; i < text.Length; i++)
        {
            if (char.IsLetterOrDigit(text[i]))
            {
                if (start == -1)
                {
                    start = i;
                }
            }
            else
            {
                if (start >= 0)
                {
                    yield return GetToken(text, i, start);
                    start = -1;
                }
            }
        }
    }

    private string GetToken(string text, int length, int start)
    {
        return text
            .Substring(start, length - start)
            .Normalize()
            .ToLowerInvariant();
    }
}