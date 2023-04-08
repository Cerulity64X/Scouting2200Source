using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Scouting2200
{
	public partial class NumericStatEntry : ContentView, IStatEntry
	{
		[Obsolete]
		public static readonly BindableProperty StatNameBindable =
			BindableProperty.Create<NumericStatEntry, string>(w => w.StatName, default);
		public string GetStat() => StatValue;
		public ColumnDefinition LabelColumn => LabelCol;
		public ColumnDefinition StatColumn => EntryCol;
		bool SilenceChange = false;
		public event Action<int> NumChanged = null;
		public string Description { get; set; } = "";
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
                Console.WriteLine(StatEntryField.Text);
				if (string.IsNullOrEmpty(StatEntryField.Text))
                {
					return "0";
                }
				return StatEntryField.Text;
			}
			private set
            {
				StatEntryField.Text = value;
            }
        }
		public int Minimum = 0;
		public int Maximum = int.MaxValue;
		public NumericStatEntry()
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

        private void Increment(object sender, EventArgs e)
		{
			int toIncrement;

			if (int.TryParse(StatEntryField.Text, out int fieldInput))
			{
				toIncrement = fieldInput;
			}
            else
            {
				toIncrement = 0;
            }
			toIncrement++;
			StatEntryField.Text = toIncrement.Clamp(Minimum, Maximum).ToString();
		}

        private void Decrement(object sender, EventArgs e)
		{
			int toDecrement;

			if (int.TryParse(StatEntryField.Text, out int fieldInput))
			{
				toDecrement = fieldInput;
			}
			else
			{
				toDecrement = 0;
			}

			toDecrement--;
			StatEntryField.Text = toDecrement.Clamp(Minimum, Maximum).ToString();
		}

        private void EnforceNumericInput(object sender, TextChangedEventArgs e)
		{
			if (true || !SilenceChange)
			{
				Entry entry = (Entry)sender;
				// stack (or heap) overflows if this is not done
				StatEntryField.TextChanged -= EnforceNumericInput;
				if (!string.IsNullOrEmpty(e.NewTextValue))
				{
					bool parsed = int.TryParse(e.NewTextValue, out int v);
					if (parsed && v.WithinII(Minimum, Maximum))
					{
                        // parse success on non empty positive # string
                        Console.WriteLine("success");
						entry.Text = v.ToString();
						ValueChanged(sender, e);
						if (NumChanged != null)
						{
							NumChanged(v);
						}
					}
					else
					{
                        // parse fail or value was negative on non empty string
                        Console.WriteLine("fail");
						entry.Text = e.OldTextValue ?? Minimum.ToString();
					}
				}
                else
                {
                    // empty string, will automatically fail parsing but we want to have empty inputs (will be converted to 0 on QR generation)
                    Console.WriteLine("empty");
					entry.Text = "";
					ValueChanged(sender, e);
					if (NumChanged != null)
					{
						NumChanged(0);
					}
				}
				StatEntryField.TextChanged += EnforceNumericInput;
			}
        }

		public event EventHandler<TextChangedEventArgs> ValueChanged = (s, e) => { };

        public void Reset()
        {
			SilenceChange = false;
			if (Clearable)
			{
				StatEntryField.Text = "";
			}
        }

        private void Describe(object sender, EventArgs e)
        {
			Device.BeginInvokeOnMainThread(async () =>
			{
				await App.Current.MainPage.DisplayAlert(StatName, Description, "Ok");
			});
		}
    }
}