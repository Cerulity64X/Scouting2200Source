using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Scouting2200
{
    public class InvisibleStat : IStatEntry
    {
        public ColumnDefinition LabelColumn => null;
        public ColumnDefinition StatColumn => null;
        public Func<string> StatImpl = null;
        public Action ResetImpl = null;
        public InvisibleStat(Func<string> impl)
        {
            StatImpl = impl;
        }
        public string GetStat() => StatImpl is null ? "0" : StatImpl();

        public void Reset() { if (ResetImpl != null) { ResetImpl(); } }
    }
}
