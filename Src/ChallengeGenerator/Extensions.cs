using System.Text;

namespace ChallengeGenerator;

static public class Extensions
{
    static public string ZipStr<T>(this IEnumerable<T> _Enumerable, string _Separator = ", ") {
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
            { SB.Append($"{_Separator}{X}"); }
        }
            
        return SB.ToString();
    }
    static public string ZipStr<T>(this IEnumerable<T> _Enumerable, Func<T, string> _Selector, string _Separator = ", ") {
        return ZipStr(_Enumerable.Select(_Selector), _Separator);
    }
}
