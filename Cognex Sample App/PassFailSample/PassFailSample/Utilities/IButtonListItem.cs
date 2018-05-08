using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PassFailSample.Utilities
{
    public interface IButtonListItem<out T> where T : Button
    {
    }
}