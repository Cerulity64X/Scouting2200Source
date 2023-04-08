using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Scouting2200
{
	public partial class TextStatEntry : ContentView, IStatEntry
	{
		[Obsolete]
		public static readonly BindableProperty StatNameBindable =
			BindableProperty.Create<TextStatEntry, string>(w => w.StatName, default);
		public ColumnDefinition LabelColumn => null;
		public ColumnDefinition StatColumn => null;
		public string Description = "";
		public bool Clearable = true;
		public string GetStat()
		{
			if (string.IsNullOrEmpty(StatValue))
			{
				return EmptyReplacement;
			}
			else
			{
				return StatValue;
			}
		}
		public string StatName
		{
			get { return (string)GetValue(StatNameBindable); }
			set { SetValue(StatNameBindable, value); }
		}
		public string StatValue
		{
			get => StatEntryField.Text;
			private set => StatEntryField.Text = value;
		}
		public string EmptyReplacement { get; set; } = "";
		public TextStatEntry()
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
				StatEntryField.Placeholder = StatName;
			}
		}
		public void ForceNameChange()
		{
			OnPropertyChanged(StatName);
		}
		public void Reset() { if (Clearable) StatEntryField.Text = ""; }
        private void Changed(object sender, TextChangedEventArgs e)
		{
			ValueChanged(sender, e);
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