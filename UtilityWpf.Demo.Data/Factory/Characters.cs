using System.Collections;
using System.Linq;
using System.Windows;
using UtilityWpf.Demo.Data.Model;

namespace UtilityWpf.Demo.Data.Factory
{
    public class Characters
    {

        public static IEnumerable Value => (Application.Current.FindResource("Characters") as IEnumerable).Cast<Character>();
    }
}
