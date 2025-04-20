using System;
using System.Linq;
using System.Text;

namespace PipesTester;

static public class Extensions
{
    static public byte[] AddValue(this string _A, string _B) {

        byte[] ListA = Encoding.ASCII.GetBytes(_A);
        byte[] ListB = Encoding.ASCII.GetBytes(_B);

        return ListA.Zip(ListB)
                    .Select(X => Convert.ToByte(X.First + X.Second))
                    .ToArray();
    }
}
