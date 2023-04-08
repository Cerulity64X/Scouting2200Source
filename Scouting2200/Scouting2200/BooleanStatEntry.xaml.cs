using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Scouting2200
{
	public partial class BooleanStatEntry : ContentView, IStatEntry
	{
		public static readonly BindableProperty StatNameBindable =
			BindableProperty.Create<BooleanStatEntry, string>(w => w.StatName, default);
		public ColumnDefinition LabelColumn => LabelCol;
		public ColumnDefinition StatColumn => EntryCol;
		public string Description = "";
		public bool Clearable = true;
		public string GetStat() => StatValue;
		public string StatName
		{
			get { return (string)GetValue(StatNameBindable); }
			set { SetValue(StatNameBindable, value); }
		}
		public string StatValue
		{
			get => StatEntryField.IsChecked ? "1" : "0";
			private set
			{
				if (int.TryParse(value, out int val))
				{
					StatEntryField.IsChecked = val != 0;
				}
			}
		}
		public BooleanStatEntry()
		{
			InitializeComponent();
			Content.BindingContext = this;
		}
		protected override void OnPropertyChanged(string propertyName)
		{
			base.OnPropertyChanged(propertyName);

			if (propertyName == StatNameBindable.PropertyName)
			{
				StatLabel.Text = StatName;
			}
		}
		public void ForceNameChange()
		{
			OnPropertyChanged(StatName);
		}

		public void Reset() { if (Clearable) StatEntryField.IsChecked = false; }

		private void Changed(object sender, CheckedChangedEventArgs e)
		{
			ValueChanged(
				sender,
				new TextChangedEventArgs(
					(!e.Value).ToString(),
					e.Value.ToString()
				)
			);
		}
		public event EventHandler<TextChangedEventArgs> ValueChanged = (s, e) => { };

		private void Describe(object sender, EventArgs e)
		{
			Device.BeginInvokeOnMainThread(async () =>
			{
				await App.Current.MainPage.DisplayAlert(StatName, Description, "Ok");
			});
		}
	}
}