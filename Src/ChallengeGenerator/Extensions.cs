using System.Text;

namespace ChallengeGenerator;

static public class Extensions
{
    static public string ZipStr<T>(this IEnumerable<T> _Enumerable) {
        StringBuilder SB = new();

        bool IsFirstElement = true;

        foreach (T X in _Enumerable)
        {
            if (IsFirstElement)
            {
                SB.Append($"{X}"); 
                IsFirstElement = false;
            }
            else
            { SB.Append($", {X}"); }
        }
            
        return SB.ToString();
    }
}
