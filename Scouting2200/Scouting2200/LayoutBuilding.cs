using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Scouting2200
{
    public interface ILayoutBuilder
    {
        IViewAdder Build();
    }
    public interface IViewAdder
    {
        void AddTo(IList<View> view);
    }
    public class NoAdder : IViewAdder
    {
        public NoAdder() { }
        public void AddTo(IList<View> view) { }
    }
    public class SingleViewAdder : IViewAdder
    {
        View view;
        public SingleViewAdder(View view)
        {
            this.view = view;
        }
        public void AddTo(IList<View> views)
        {
            views.Add(view);
        }
    }
    public class MultiViewAdder : IViewAdder
    {
        IEnumerable<View> Views;
        public MultiViewAdder(IEnumerable<View> views)
        {
            Views = views;
        }
        public void AddTo(IList<View> views)
        {
            foreach (View view in Views)
            {
                views.Add(view);
            }
        }
        public MultiViewAdder With(params View[] other) => new MultiViewAdder(Enumerable.Concat(Views, other));
    }
    public class MultiBuilder : ILayoutBuilder
    {
        IEnumerable<ILayoutBuilder> builders;
        public MultiBuilder(IEnumerable<ILayoutBuilder> builders)
        {
            this.builders = builders;
        }
        public IViewAdder Build()
        {
            List<View> views = new List<View>();
            foreach (ILayoutBuilder builder in builders)
            {
                builder.Build().AddTo(views);
            }
            return new MultiViewAdder(views);
        }
    }
    public class StatBuilder : ILayoutBuilder
    {
        EntryType EType;
        string Name;
        bool ShrinkLeft = false;
        string Description;
        public bool Clearable = true;
        public int NumMin = 0;
        public int NumMax = int.MaxValue;
        public Action<int> NumberChanged = null;
        public Func<string> InvisImpl = null;
        public static Dictionary<string, string> EmptyTextSentinels = new Dictionary<string, string>()
        {
            { "Notes", "No notes" }
        };
        EventHandler<TextChangedEventArgs> TextChanged;
        public StatBuilder(string name, EntryType et, string desc, bool shrinkLeft = false, EventHandler<TextChangedEventArgs> ev = null)
        {
            Name = name;
            EType = et;
            ShrinkLeft = shrinkLeft;
            TextChanged = ev;
            Description = desc;
        }
        public IViewAdder Build()
        {
            IStatEntry entry = null;
            switch (EType)
            {
                case EntryType.Numeric:
                    NumericStatEntry sentry = new NumericStatEntry() { StatName = Name, Description = Description, Clearable = Clearable, Minimum = NumMin, Maximum = NumMax };
                    if (NumberChanged != null)
                    {
                        sentry.NumChanged += NumberChanged;
                    }
                    entry = sentry;
                    break;
                case EntryType.Boolean:
                    entry = new BooleanStatEntry() { StatName = Name, Description = Description, Clearable = Clearable };
                    break;
                case EntryType.PlainTxt:
                    string replacement = EmptyTextSentinels.GetOrDefault(Name, "None");
                    entry = new TextStatEntry() { StatName = Name, EmptyReplacement = replacement, Description = Description, Clearable = Clearable };
                    break;
                case EntryType.SelectOne:
                    entry = new SelectOneStatEntry(MainPage.RadioEntries[Name]) { StatName = Name, Description = Description, Clearable = Clearable };
                    break;
                case EntryType.SelectNum:
                    entry = new NumericRadioStatEntry(MainPage.RadioEntries[Name]) { StatName = Name, Description = Description, Clearable = Clearable };
                    break;
                case EntryType.Invisible:
                    entry = new InvisibleStat(InvisImpl);
                    break;
            }
            if (entry != null)
            {
                if (ShrinkLeft)
                {
                    if (entry.LabelColumn != null) entry.LabelColumn.Width = new GridLength(1, GridUnitType.Star);
                    if (entry.StatColumn != null) entry.StatColumn.Width = new GridLength(2, GridUnitType.Star);
                }
                MainPage.stats.Add(entry);
                if (entry is View)
                {
                    return ((View)entry).Adder();
                }
            }
            return new NoAdder();
        }
    }
    public class GridBuilder : ILayoutBuilder
    {
        int GridHeight;
        int GridWidth;
        ILayoutBuilder[][] Columns;
        public GridBuilder(ILayoutBuilder[][] columns)
        {
            GridHeight = columns.Select(column => column.Length).Max();
            GridWidth = columns.Length;
            Columns = columns;
        }
        public IViewAdder Build()
        {
            Grid gout = new Grid()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            for (int i = 0; i < GridHeight; i++)
            {
                gout.RowDefinitions.Add(new RowDefinition());
            }
            for (int i = 0; i < GridWidth; i++)
            {
                gout.ColumnDefinitions.Add(new ColumnDefinition());
            }
            foreach ((ILayoutBuilder[] column, int x) in Columns.Enumerate())
            {
                foreach ((ILayoutBuilder builder, int y) in column.Enumerate())
                {
                    foreach (View v in builder.Build().Collect())
                    {
                        gout.Children.Add(v, x, GridHeight - y - 1);
                    }
                }
            }
            return gout.Adder();
        }
    }
    public class LabelBuilder : ILayoutBuilder
    {
        string Text;
        public LabelBuilder(string txt)
        {
            Text = txt;
        }
        public IViewAdder Build() => new Label()
        {
            Text = Text,
            HorizontalOptions = LayoutOptions.Center,
            FontSize = 30
        }.Adder();
    }
    public class ColorFillBuilder : ILayoutBuilder
    {
        Color color;
        public ColorFillBuilder(Color clr)
        {
            color = clr;
        }
        public IViewAdder Build() => new BoxView()
        {
            Color = color,
            HorizontalOptions = LayoutOptions.FillAndExpand,
            VerticalOptions = LayoutOptions.FillAndExpand,

        }.Adder();
    }
    public class ColorStack : ILayoutBuilder
    {
        Color color;
        IEnumerable<ILayoutBuilder> builders;
        public ColorStack(Color clr, IEnumerable<ILayoutBuilder> contained)
        {
            color = clr;
            builders = contained;
        }
        public IViewAdder Build()
        {
            StackLayout stout = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                BackgroundColor = color,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(10, 0)
            };
            foreach (ILayoutBuilder builder in builders)
            {
                builder.Build().AddTo(stout.Children);
            }
            return stout.Adder();
        }
    }
}
