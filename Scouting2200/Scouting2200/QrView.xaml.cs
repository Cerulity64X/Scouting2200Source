using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using XImage = Xamarin.Forms.Image;
using XColor = Xamarin.Forms.Color;

namespace Scouting2200
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QrView : ContentPage
    {
        public QrView(string csv, string teamNumber, bool isRed)
        {
            InitializeComponent();
            QRCodeGenerator gen = new QRCodeGenerator();
            QRCodeData data = gen.CreateQrCode(csv, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qr = new PngByteQRCode(data);

            XImage image = new XImage();
            image.Source = ImageSource.FromStream(() => new MemoryStream(qr.GetGraphic(20)));

            MainLayout.Children.Add(image);
            TeamNum.Text = teamNumber;
            XColor teamColor = isRed ? XColor.Red : XColor.Blue;
            TeamNum.TextColor = XColor.Gray.Times(0.75).Plus(teamColor.Times(0.25));
        }

        private void GoBack(object sender, EventArgs e)
        {
            App.Current.MainPage.Navigation.RemovePage(this);
        }
    }
}