using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Scouting2200
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NumericRadioStatEntry : ContentView, IStatEntry
	{
		[Obsolete]
		public static readonly BindableProperty StatNameBindable =
			BindableProperty.Create<NumericStatEntry, string>(w => w.StatName, default);
		public string GetStat() => StatValue;
		public string CheckedName;
		public int CheckedIdx;
		public List<RadioButton> Radios = new List<RadioButton>();
		public ColumnDefinition LabelColumn => LabelCol;
		public ColumnDefinition StatColumn => EntryCol;
		public string Description = "";
		public bool Clearable = true;
		public string StatName
		{
			get { return (string)GetValue(StatNameBindable); }
			set { SetValue(StatNameBindable, value); }
		}
		public string StatValue
		{
            get
            {
                foreach ((RadioButton radio, int idx) in Radios.Enumerate())
                {
					if (radio.IsChecked) { return idx.ToString(); }
                }
				return "0";
            }
			private set
			{
				if (int.TryParse(value, out int idx) && idx < Radios.Count) Radios[idx].IsChecked = true;
			}
		}
		public NumericRadioStatEntry(string[] statNames)
		{
			InitializeComponent();
			foreach ((string name, int idx) in new One<string>("None").Wrap().Concat(statNames).Enumerate())
			{
				RadioButton radio = new RadioButton() { Value = (name, idx), VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center };
				if (idx == 0)
				{
					radio.IsChecked = true;
				}
				radio.CheckedChanged += ChangeVal;
				Radios.Add(radio);
				TapGestureRecognizer tgr = new TapGestureRecognizer();
				tgr.Tapped += Describe;
				Label lab = new Label()
				{
					Text = name,
					FontSize = 20,
					VerticalOptions = LayoutOptions.Center
				};
				lab.GestureRecognizers.Add(tgr);
				RadioGrid.Children.Add(lab, 0, idx);
				RadioGrid.Children.Add(radio, 1, idx);
            }
			Content.BindingContext = this;
		}

        private void ChangeVal(object sender, CheckedChangedEventArgs e)
        {
			(CheckedName, CheckedIdx) = ((string, int))((RadioButton)sender).Value;
        }

        protected override void OnPropertyChanged(string propertyName)
		{
			base.OnPropertyChanged(propertyName);

			/*if (propertyName == StatNameBindable.PropertyName)
			{
				StatLabel.Text = StatName;
				StatEntryField.Placeholder = StatName;
			}*/
		}
		public void ForceNameChange()
		{
			OnPropertyChanged(StatName);
		}

		public event EventHandler<TextChangedEventArgs> ValueChanged = (s, e) => { };

		public void Reset() { if (Clearable) Radios[0].IsChecked = true; }

		private void Describe(object sender, EventArgs e)
		{
			Device.BeginInvokeOnMainThread(async () =>
			{
				await App.Current.MainPage.DisplayAlert(StatName, Description, "Ok");
			});
		}
	}
}