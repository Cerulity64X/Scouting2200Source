using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Scouting2200
{
    public interface IStatEntry
    {
        string GetStat();
        void Reset();
        ColumnDefinition LabelColumn { get; }
        ColumnDefinition StatColumn { get; }
    }
}
