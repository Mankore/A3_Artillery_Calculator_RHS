using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace A3_Arty_Calc
{
    /// <summary>
    /// Interaction logic for MapWindow.xaml
    /// </summary>
    public partial class MapWindow : Window
    {
        static List<CoordHeight> coordList = new List<CoordHeight>();
        public MapWindow()
        {
            InitializeComponent();
            initList();
        }

        private void initList(string filename = "sahrani.txt")
        {
            foreach (string line in System.IO.File.ReadLines("../../coordinates/" + filename))
            {
                string[] data = line.Split(' ');
                int x = -1, y = -1;
                double height = 9999;
                try
                {

                x = Convert.ToInt32(data[0]);
                y = Convert.ToInt32(data[1]);
                height = double.Parse(data[2], CultureInfo.InvariantCulture);
                } catch
                {
                    Console.WriteLine("Error while converting values");
                }
                coordList.Add(new CoordHeight(x, y, height));
            }
            Console.WriteLine("List initialised");
            Console.WriteLine(coordList.Count);
            Console.WriteLine(coordList[100].toString());
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image image = e.OriginalSource as Image;
            if (image != null && e.ClickCount >= 2)
            {
                Point coordinates = e.GetPosition(image);
                
                double iWidth = image.Width;
                double iHeight = image.Height;

                double xPercent = coordinates.X / iWidth;
                double yPercent = (iHeight - coordinates.Y) / iHeight;

                double armaX = Math.Round(coordList[coordList.Count - 1].x * xPercent);
                double armaY = Math.Round(coordList[coordList.Count - 1].x * yPercent);

                double roundedArmaX = Math.Round(armaX / 10) * 10;
                double roundedArmaY = Math.Round(armaY / 10) * 10;

                CoordHeight foundCoord = coordList.Find(item => item.x == roundedArmaX && item.y == roundedArmaY);
                Console.WriteLine(foundCoord.toString());
            }
        }
    }
}
