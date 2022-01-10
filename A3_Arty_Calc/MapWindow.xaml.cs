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
        static CoordHeight artyCoord = new CoordHeight(0, 0, 0);
        static Ellipse artyEllipse;
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
                CoordHeight coords = new CoordHeight(-1, -1, -1);
                try
                {
                    coords.x = Convert.ToInt32(data[0]);
                    coords.y = Convert.ToInt32(data[1]);
                    coords.height = double.Parse(data[2], CultureInfo.InvariantCulture);
                    if (coords.x >= 10000) coords.x = coords.x / 10;
                    if (coords.y >= 10000) coords.y = coords.y / 10;
                }
                catch
                {
                    Console.WriteLine("Error while converting values");
                }
                coordList.Add(coords);
            }
            Console.WriteLine("List initialised");
            Console.WriteLine(coordList.Count);
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

                bool isShiftPressed = Keyboard.IsKeyDown(Key.LeftShift);

                Ellipse ellipse = createEllipse(e.GetPosition(Cnv), isShiftPressed, foundCoord);

                if (isShiftPressed)
                {
                    artyCoord = foundCoord;
                    Console.WriteLine($"Current arty coordinates: {artyCoord.toString()}");

                    Cnv.Children.Remove(artyEllipse);
                    artyEllipse = ellipse;
                }
            }
        }

        private Ellipse createEllipse(Point coords, bool isShiftPressed, CoordHeight foundCoord)
        {
            Brush friendlyBrush = Brushes.DarkBlue;
            Brush targetBrush = Brushes.Red;

            Ellipse ellipse = new Ellipse();

            const double ellipseSize = 0.5;
            ellipse.Fill = isShiftPressed ? friendlyBrush : targetBrush;
            ellipse.Stroke = isShiftPressed ? friendlyBrush : targetBrush;
            ellipse.StrokeThickness = 0.1;
            ellipse.Opacity = 0.5;

            ellipse.Width = ellipseSize;
            ellipse.Height = ellipseSize;

            Cnv.Children.Add(ellipse);

            Canvas.SetLeft(ellipse, coords.X - (ellipseSize / 2));
            Canvas.SetTop(ellipse, coords.Y - (ellipseSize / 2));

            ellipse.MouseEnter += (object sender, MouseEventArgs e) =>
            {
                //Console.WriteLine("Enter");
            };

            ellipse.MouseLeave += (object sender, MouseEventArgs e) =>
            {
                //Console.WriteLine("Leave");
            };

            if (isShiftPressed)
            {
                ellipse.ToolTip = $"Arty Coordinates: {foundCoord.x} {foundCoord.y} {foundCoord.height}";
            } else
            {
                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                
                string Artillery = mainWindow.Artillery_Selector.Text;
                double xBattery = artyCoord.x;
                double yBattery = artyCoord.y;
                double altBattery = artyCoord.height;

                double xTarget = foundCoord.x;
                double yTarget = foundCoord.y;
                double altTarget = foundCoord.height;

                Console.WriteLine($"{xBattery} {yBattery} {altBattery}\n{xTarget} {yTarget} {altTarget}");

                string Charge = mainWindow.Charge_Selector.Text;
                string Shell = mainWindow.Shell_Selector.Text;

                bool isTopDown = Convert.ToBoolean(mainWindow.High_Arc_Checkbox.IsChecked);

                double elevation, tof, range, bearing, altDiff, initSpeed, exitAngle, apex, dist;
                range = Logic.getRange(xBattery, yBattery, xTarget, yTarget);
                bearing = Logic.getBearing(xBattery, yBattery, xTarget, yTarget);
                altDiff = Logic.getAltitudeDiff(altBattery, altTarget);
                Artillery Arty = Array.Find(mainWindow.ArtilleryList, item => item.Name == Artillery);
                FireMode fMode = Array.Find(Arty.modes, item => item.name == Charge);
                ShellType shell = Array.Find(Arty.shellTypes, item => item.name == Shell);
                initSpeed = shell.initSpeed * fMode.artilleryCharge;

                (elevation, tof, exitAngle, apex, dist) = Logic.getAngleSolutionForRange2(range, initSpeed, altDiff, Arty, shell, isTopDown);

                ellipse.ToolTip = $"Arty: {Arty.Name}, Shell: {shell.name}, fMode: {fMode.name}\n" +
                    $"Elevation: {elevation:f3}, tof: {tof:f3}, eAngle: {exitAngle:f3}\n" +
                    $"apex: {apex:f3}, dist: {dist:f3}";
            }

            return ellipse;
        }
    }
}
