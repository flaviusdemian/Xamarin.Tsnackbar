using Java.Lang;
using String = System.String;

namespace Component.TSnackbar
{
    public class CharsequenceHelper
    {
        public static String ToString(ICharSequence charSequence)
        {
            return charSequence.ToString();
        }

        public static ICharSequence ToCharSequence(string value)
        {
            return new Java.Lang.String(value);
        }
    }
}