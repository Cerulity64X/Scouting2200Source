using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Scouting2200
{
	public enum EntryType
    {
		Numeric,
		Boolean,
		PlainTxt,
		SelectOne,
		SelectNum,
		Invisible
    }
	public partial class MainPage : ContentPage
	{
		int count = 0;
		public bool IsRed = false;
		public static int[] teleopPieces = new int[6];
		public static List<IStatEntry> stats = new List<IStatEntry>();
		public static Dictionary<string, string[]> RadioEntries = new Dictionary<string, string[]>()
		{
			{
				"Auto End",
				new[]
				{
					"Dock",
					"Dock & Engage"
				}
			},
            {
				"Teleop End",
                new[]
                {
					"Park",
					"Dock",
					"Dock & Engage"
                }
            },
            {
				"Loading Station Choice",
                new[]
                {
					"Single (Ramp)",
					"Double (Platforms)"
                }
            }
		};
		List<ILayoutBuilder> layoutDefinition = new List<ILayoutBuilder>()
		{
			new StatBuilder("Match Number", EntryType.Numeric, "The number of the current match.") { Clearable = false },

			new ColorStack(Color.Red.MultiplyAlpha(0.1), new ILayoutBuilder[]
			{
				new LabelBuilder("Auto"),
				new StatBuilder("Mobility", EntryType.Boolean, "Whether or not the robot has left its community during autonomous."),
				new GridBuilder(
					new[]
					{
						new ILayoutBuilder[]
						{
							new ColorFillBuilder(Color.Yellow.MultiplyAlpha(0.30)).With(new StatBuilder("L1 Cone", EntryType.Numeric, "How many cones have been placed at the bottom level of the scoring area during auto.", true)),
							new ColorFillBuilder(Color.Yellow.MultiplyAlpha(0.45)).With(new StatBuilder("L2 Cone", EntryType.Numeric, "How many cones have been placed at the middle level of the scoring area during auto.", true)),
							new ColorFillBuilder(Color.Yellow.MultiplyAlpha(0.60)).With(new StatBuilder("L3 Cone", EntryType.Numeric, "How many cones have been placed at the top level of the scoring area during auto.", true)),
						},
						new[]
						{
							new ColorFillBuilder(Color.Purple.MultiplyAlpha(0.30)).With(new StatBuilder("L1 Cube", EntryType.Numeric, "How many cubes have been placed at the bottom level of the scoring area during auto.", true)),
							new ColorFillBuilder(Color.Purple.MultiplyAlpha(0.45)).With(new StatBuilder("L2 Cube", EntryType.Numeric, "How many cubes have been placed at the middle level of the scoring area during auto.", true)),
							new ColorFillBuilder(Color.Purple.MultiplyAlpha(0.60)).With(new StatBuilder("L3 Cube", EntryType.Numeric, "How many cubes have been placed at the top level of the scoring area during auto.", true)),
						}
					}
				),
				new StatBuilder("Auto End", EntryType.SelectOne, "Dock: The robot is on the charging station, but not level\nDock & Engage: The robot is on the charging station and is level."),
			}),
			new ColorStack(Color.Lime.MultiplyAlpha(0.1), new ILayoutBuilder[]
			{
				new LabelBuilder("TeleOp"),
				new GridBuilder(
					new[]
					{
						new[]
						{
							new ColorFillBuilder(Color.Yellow.MultiplyAlpha(0.30)).With(new StatBuilder("L1 Cone", EntryType.Numeric, "How many cones have been placed at the bottom level of the scoring area during teleop.", true) { NumberChanged = i => { teleopPieces[0] = i; } }),
							new ColorFillBuilder(Color.Yellow.MultiplyAlpha(0.45)).With(new StatBuilder("L2 Cone", EntryType.Numeric, "How many cones have been placed at the middle level of the scoring area during teleop.", true) { NumberChanged = i => { teleopPieces[1] = i; } }),
							new ColorFillBuilder(Color.Yellow.MultiplyAlpha(0.60)).With(new StatBuilder("L3 Cone", EntryType.Numeric, "How many cones have been placed at the top level of the scoring area during teleop.", true) { NumberChanged = i => { teleopPieces[2] = i; } }),
						},
						new[]
						{
							new ColorFillBuilder(Color.Purple.MultiplyAlpha(0.30)).With(new StatBuilder("L1 Cube", EntryType.Numeric, "How many cubes have been placed at the bottom level of the scoring area during teleop.", true) { NumberChanged = i => { teleopPieces[3] = i; } }),
							new ColorFillBuilder(Color.Purple.MultiplyAlpha(0.45)).With(new StatBuilder("L2 Cube", EntryType.Numeric, "How many cubes have been placed at the middle level of the scoring area during teleop.", true) { NumberChanged = i => { teleopPieces[4] = i; } }),
							new ColorFillBuilder(Color.Purple.MultiplyAlpha(0.60)).With(new StatBuilder("L3 Cube", EntryType.Numeric, "How many cubes have been placed at the top level of the scoring area during teleop.", true) { NumberChanged = i => { teleopPieces[5] = i; } }),
						}
					}
				),

				new StatBuilder("Teleop End", EntryType.SelectOne, "Park: The robot is completely within its community.\nDock: The robot is on the charging station, but not level.\nDock & Engage: The robot is on the charging station and is level."),
			}),

			new ColorStack(Color.Blue.MultiplyAlpha(0.1), new ILayoutBuilder[]
			{
				new LabelBuilder("Overall"),
				new StatBuilder("Defence Played", EntryType.Boolean, "Whether or not the robot played defence (focused on inhibiting other teams)."),
				new StatBuilder("Driver Skill", EntryType.Numeric, "Rating from 1-5 on how goated the robot's driver was.") { NumMin = 1, NumMax = 5 },
				new StatBuilder("Fouls", EntryType.Numeric, "How many fouls the robot collected."),
				new StatBuilder("Tech Fouls", EntryType.Numeric, "How many technical fouls the robot collected."),

				new StatBuilder("Loading Station Choice", EntryType.SelectNum, "Which loading station the robot grabbed from.\nSingle: The ramp where game pieces fall into the arena\nDouble: The sliding platforms on which game pieces are placed on"),

				new StatBuilder("Notes", EntryType.PlainTxt, "General information about the robot."),
				new StatBuilder("Total Pieces", EntryType.Invisible, "(Calculated) The total amount of pieces in TeleOp.\nIf you're seeing this, something went wrong\nPlease tell Ethan. wablalbalblfdlbkasdlsjfldsa") { InvisImpl = () => teleopPieces.Sum().ToString() },
				new StatBuilder("Died", EntryType.Boolean, "Whether or not the robot died at all during the match.")
			}),
        };
		public MainPage()
		{
			InitializeComponent();
			TeamNum.ValueChanged += (sender, args) =>
			{
				if (args.NewTextValue == "")
                {
					TitleText.Text = "Scouting";
                }
                else
                {
					TitleText.Text = $"Scouting ({args.NewTextValue})";
                }
			};
            foreach (ILayoutBuilder def in layoutDefinition)
            {
				IViewAdder view = def.Build();
				if (view != null)
                {
					view.AddTo(StatStack.Children);
                }
            }
			BindingContext = this;
		}

        private void CountUp(object sender, EventArgs e)
		{
			count++;
			Button btn = (Button)sender;
			Console.WriteLine(e.ToString());
			btn.Text = $"You clicked {count} times";
		}

        private void GenerateQR(object sender, EventArgs e)
        {
			//string csvHeader = string.Join(",", stats.Select((stat) => stat.StatName));
			string csvOutput = $"{TeamNum.GetStat()}\t" + string.Join("\t", stats.Select((stat) => stat.GetStat())) + $"\t{(IsRed ? "Red" : "Blue")}";
            Console.WriteLine(csvOutput);
			string teamNum = TeamNum.GetStat();
			App.Current.MainPage.Navigation.PushAsync(new QrView(csvOutput, teamNum, IsRed));
		}

        private void ClearStats(object sender, EventArgs e)
        {
			bool clear = false;
			Device.BeginInvokeOnMainThread(async () =>
			{
				clear = await DisplayAlert("Stat Clear", "Clear team stats?", "Yes", "No");
				if (clear)
				{
					TeamNum.Reset();
					foreach (IStatEntry entry in stats)
					{
						entry.Reset();
					}
				}
			});
        }

        private void MakeBlue(object sender, CheckedChangedEventArgs e)
        {
			IsRed = false;
			TitleFrame.BackgroundColor = Color.Blue;
		}

        private void MakeRed(object sender, CheckedChangedEventArgs e)
		{
			IsRed = true;
			TitleFrame.BackgroundColor = Color.Red;
		}
    }
}
