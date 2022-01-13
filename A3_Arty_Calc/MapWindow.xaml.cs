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
        static Ellipse triggerEllipse;
        static Brush friendlyBrush = Brushes.DarkBlue;
        static Brush targetBrush = Brushes.Red;
        static Brush triggerBrush = Brushes.Yellow;
        public MapWindow()
        {
            InitializeComponent();
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            string SelectedMap = mainWindow.Map_Selector.Text;
            initList(SelectedMap.ToLower() + ".txt");
            var uri = new Uri("pack://application:,,,/img/" + SelectedMap + "_opt.png");
            //var uri = new Uri("./img/" + SelectedMap + "_opt.png", UriKind.Relative);
            var bitmap = new BitmapImage(uri);
            Map_Image.Source = bitmap;
        }

        private void initList(string filename = "sahrani.txt")
        {
            if (coordList.Count != 0) coordList.Clear();
            foreach (string line in System.IO.File.ReadLines("./coordinates/" + filename))
            {
                string[] data = line.Split(' ');
                CoordHeight coords = new CoordHeight(-1, -1, -1);
                try
                {
                    coords.x = Convert.ToInt32(data[0]);
                    coords.y = Convert.ToInt32(data[1]);
                    coords.height = double.Parse(data[2], CultureInfo.InvariantCulture);
                    if (coords.x >= 10) coords.x = coords.x / 10;
                    if (coords.y >= 10) coords.y = coords.y / 10;
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
            bool isShiftPressed = Keyboard.IsKeyDown(Key.LeftShift);
            bool isCtrlPressed = Keyboard.IsKeyDown(Key.LeftCtrl);
            bool isAltPressed = Keyboard.IsKeyDown(Key.LeftAlt);
            Image image = e.OriginalSource as Image;
            
            if (image != null && (isCtrlPressed || isShiftPressed || isAltPressed))
            {
                Point coordinates = e.GetPosition(image);
                CoordHeight foundCoord = getArmaCoords(coordinates, image);
                Console.WriteLine(foundCoord.toString());

                if (isAltPressed)
                {
                    createTriggerEllipse(e.GetPosition(Cnv));
                }
                else
                {
                    Ellipse ellipse = createMark(e.GetPosition(Cnv), isShiftPressed, foundCoord);
                    MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

                    if (isShiftPressed)
                    {
                        artyCoord = foundCoord;
                        Console.WriteLine($"Current arty coordinates: {artyCoord.toString()}");

                        Cnv.Children.Remove(artyEllipse);
                        artyEllipse = ellipse;

                        clearTargets();

                        // Set main window Arty Coordinates
                        mainWindow.Battery_X.Text = Convert.ToString(artyCoord.x);
                        mainWindow.Battery_Y.Text = Convert.ToString(artyCoord.y);
                        mainWindow.Battery_Alt.Text = Convert.ToString(artyCoord.height);

                    } else
                    {
                        // Set main Window Target coordinates
                        mainWindow.Target_X.Text = Convert.ToString(foundCoord.x);
                        mainWindow.Target_Y.Text = Convert.ToString(foundCoord.y);
                        mainWindow.Target_Alt.Text = Convert.ToString(foundCoord.height);
                    }
                }
            }
        }

        private CoordHeight getArmaCoords(Point coordinates, Image image)
        {
            double iWidth = image.Width;
            double iHeight = image.Height;

            double xPercent = coordinates.X / iWidth;
            double yPercent = (iHeight - coordinates.Y) / iHeight;

            double armaX = Math.Round(coordList[coordList.Count - 1].x * xPercent);
            double armaY = Math.Round(coordList[coordList.Count - 1].x * yPercent);

            double roundedArmaX = Math.Round(armaX / 10) * 10;
            double roundedArmaY = Math.Round(armaY / 10) * 10;

            //Console.WriteLine($"coordinates.x:{coordinates.X}, coordinates.y:{coordinates.Y}");
            //Console.WriteLine($"iWidth:{iWidth}, iHeight:{iHeight}");
            //Console.WriteLine($"xPercent:{xPercent}, yPercent:{yPercent}");
            //Console.WriteLine($"armaX:{armaX}, armaY:{armaY}");
            //Console.WriteLine($"roundedArmaX:{roundedArmaX}, roundedArmaY:{roundedArmaY}");

            CoordHeight foundCoord = coordList.Find(item => item.x == armaX && item.y == armaY);
            return foundCoord;
        }

        private Ellipse createMark(Point coords, bool isShiftPressed, CoordHeight foundCoord)
        {
            Ellipse ellipse = createEllipse(coords, 0.5, isShiftPressed ? friendlyBrush : targetBrush, 0.75);

            ellipse.MouseEnter += (object sender, MouseEventArgs e) =>
            {
                //Console.WriteLine("Enter");
            };

            ellipse.MouseLeave += (object sender, MouseEventArgs e) =>
            {
                ToolTip ttEllipse = ToolTipService.GetToolTip(ellipse) as ToolTip;
                ttEllipse.IsOpen = false;
            };

            ToolTip tt = new ToolTip();

            ToolTipService.SetInitialShowDelay(ellipse, 0);
            ToolTipService.SetIsEnabled(ellipse, true);
            string ttText = "";


            if (isShiftPressed)
            {
                double areaFlatness = checkIfFlatAround(foundCoord);
                ttText = $"Arty Coordinates: {foundCoord.x} {foundCoord.y} {foundCoord.height}\n" +
                    $"Flatness: {areaFlatness:f5}";
            }
            else
            {
                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

                string Artillery = mainWindow.Artillery_Selector.Text;
                double xBattery = artyCoord.x;
                double yBattery = artyCoord.y;
                double altBattery = artyCoord.height;

                double xTarget = foundCoord.x;
                double yTarget = foundCoord.y;
                double altTarget = foundCoord.height;

                Console.WriteLine($"\nxBattery:{xBattery}, yBattery:{yBattery}, xTarget:{xTarget},yTarget:{yTarget}\n");

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

                (elevation, tof, exitAngle, apex, dist) = Logic.getAngleSolutionForRange2(range, initSpeed, altDiff, Arty, shell, false);

                double tdElevation, tdTof, tdExitAngle, tdApex, tdDist;

                (tdElevation, tdTof, tdExitAngle, tdApex, tdDist) = Logic.getAngleSolutionForRange2(range, initSpeed, altDiff, Arty, shell, true);

                ttText = $"Arty: {Arty.Name}, Shell: {shell.name}, fMode: {fMode.name}\n" +
                    $"Elevation: {elevation:f3}, Tof: {tof:f3}, eAngle: {exitAngle:f3}\n" +
                    $"apex: {apex:f3}, range: {range:f3}, targetHeight: {altTarget}\n" +
                    $"Top-Down:\nElevation: {tdElevation:f3}, Tof: {tdTof:f3}, eAngle: {tdExitAngle:f3}";
            }

            tt.Content = ttText;
            tt.IsOpen = true;
            ToolTipService.SetToolTip(ellipse, tt);

            return ellipse;
        }

        private Ellipse createTriggerEllipse(Point coords)
        {
            Ellipse ellipse = createEllipse(coords, 29.5, triggerBrush, 0.25);
            ellipse.IsHitTestVisible = false;

            if (triggerEllipse != null) Cnv.Children.Remove(triggerEllipse);
            triggerEllipse = ellipse;

            return ellipse;
        }

        private Ellipse createEllipse(Point coords, double size, Brush brush, double opacity)
        {
            Ellipse ellipse = new Ellipse();

            ellipse.Fill = brush;
            ellipse.Stroke = brush;
            ellipse.StrokeThickness = 0.1;
            ellipse.Opacity = opacity;

            ellipse.Width = size;
            ellipse.Height = size;

            Cnv.Children.Add(ellipse);

            Canvas.SetLeft(ellipse, coords.X - (size / 2));
            Canvas.SetTop(ellipse, coords.Y - (size / 2));

            return ellipse;
        }

        private void ClearPoints_Button_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<Ellipse> collection = Cnv.Children.OfType<Ellipse>();
            Ellipse[] ellipseArr = collection.ToArray();

            for (int i = ellipseArr.Count() - 1; i >= 0; i--)
            {
                Cnv.Children.Remove(ellipseArr[i]);
            }
        }

        private void clearTargets()
        {
            IEnumerable<Ellipse> collection = Cnv.Children.OfType<Ellipse>();
            Ellipse[] ellipseArr = collection.ToArray();

            for (int i = ellipseArr.Count() - 1; i >= 0; i--)
            {
                if (ellipseArr[i] != artyEllipse && ellipseArr[i] != triggerEllipse)
                {
                    Cnv.Children.Remove(ellipseArr[i]);
                }
            }
        }

        private double checkIfFlatAround(CoordHeight coord, bool isStrict = false)
        {
            double midDeviation = 999;
            double altSumm = 0;
            CoordHeight latestCoord = coordList[coordList.Count - 1];

            if (coord.x > 0 && coord.y > 0 && coord.x + 2 <= latestCoord.x && coord.y + 2 <= latestCoord.y)
            {
                for (int i = coord.x - 1; i < coord.x + 2; i++)
                {
                    for (int j = coord.y - 1; j < coord.y + 2; j++)
                    {
                        CoordHeight foundCoord = coordList.Find(item => item.x == i && item.y == j);
                        altSumm += Math.Abs(coord.height - foundCoord.height);
                        if (isStrict && altSumm > 0) return 999;
                    }
                }
            }

            midDeviation = altSumm / 8;
            //Console.WriteLine($"midDeviation: {midDeviation}");
            return midDeviation;
        }

        private void ClearTargets_Button_Click(object sender, RoutedEventArgs e)
        {
            clearTargets();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                Point p = Mouse.GetPosition(Cnv);
                HitTestResult hitTestResult = VisualTreeHelper.HitTest(Cnv, p);
                Ellipse ellipseToDelete = hitTestResult.VisualHit as Ellipse;
                if (ellipseToDelete == null || ellipseToDelete == triggerEllipse)
                    return;

                Cnv.Children.Remove(ellipseToDelete);
                e.Handled = true;
            }
        }

        private void findFlatCoordinates()
        {
            foreach (CoordHeight item in coordList)
            {
                if (item.height >= 0)
                {
                    double deviation = checkIfFlatAround(item, true);
                    if (deviation < 0.02) Console.WriteLine($"Coords:{item.toString()}, dev: {deviation}");
                }
            }
        }

        private void FindFlat_Button_Click(object sender, RoutedEventArgs e)
        {
            findFlatCoordinates();
        }
    }
}
