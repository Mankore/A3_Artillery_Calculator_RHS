using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Globalization;
using System.IO;

namespace A3_Arty_Calc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        Artillery DefaultArty = new Art_2S1_Direct();

        public Artillery[] ArtilleryList = new Artillery[] {
            new Art_2S1_Direct(),
            new Art_2S1(),
            new Art_M109A6(),
            new Art_2S3M1(),
        };

        List<TargetLogItem> logItems = new List<TargetLogItem>();
        public MainWindow()
        {
            InitializeComponent();

            logItems.Add(new TargetLogItem { CoordLog = "2450,1900,3/1625,1875,46/Full Charge" });
            logItems.Add(new TargetLogItem { CoordLog = "2450,1900,3/2015,1375,22/Full Charge" });
            logItems.Add(new TargetLogItem { CoordLog = "1676,1691,46/2296,1888,5/Full Charge" });
            logItems.Add(new TargetLogItem { CoordLog = "2450,1900,3/2032,1304,124/Full Charge" });
            logItems.Add(new TargetLogItem { CoordLog = "2450,1900,3/2248,1652,14/Full Charge" });
            logItems.Add(new TargetLogItem { CoordLog = "2450,1900,3/2107,1929,15/Full Charge" });
            icSolutionLog.ItemsSource = logItems;

            Array.ForEach(DefaultArty.shellTypes, item => Shell_Selector.Items.Add(item.name));
            Shell_Selector.SelectedItem = Shell_Selector.Items[0];

            Array.ForEach(DefaultArty.modes, item => Charge_Selector.Items.Add(item.name));
            Charge_Selector.SelectedItem = Charge_Selector.Items[0];

            Array.ForEach(ArtilleryList, item => Artillery_Selector.Items.Add(item.Name));
            Artillery_Selector.SelectedItem = Artillery_Selector.Items[0];

            string[] pdfFiles = Directory.GetFiles(@"coordinates", "*.txt").Select(System.IO.Path.GetFileName).ToArray();
            foreach (var item in pdfFiles)
            {
                Map_Selector.Items.Add(item.Replace(".txt", ""));
            }
            Map_Selector.SelectedItem = Map_Selector.Items[0];
        }

        private void Compute_Button_Click(object sender, RoutedEventArgs e)
        {
            bool isInputEmptyCondition = String.IsNullOrEmpty(Battery_X.Text) || String.IsNullOrEmpty(Battery_Y.Text) || String.IsNullOrEmpty(Battery_Alt.Text) || String.IsNullOrEmpty(Target_X.Text) || String.IsNullOrEmpty(Target_Y.Text) || String.IsNullOrEmpty(Target_Alt.Text);
            if (isInputEmptyCondition)
            {
                SolutionTextBox.Text = "You need to fill out all fields first!";
                return;
            }

            SolutionTextBox.Text = "Computing..";
            string Artillery = Artillery_Selector.Text;
            double xBattery = Convert.ToDouble(Battery_X.Text);
            double yBattery = Convert.ToDouble(Battery_Y.Text);
            double altBattery = Convert.ToDouble(Battery_Alt.Text);

            double xTarget = Convert.ToDouble(Target_X.Text);
            double yTarget = Convert.ToDouble(Target_Y.Text);
            double altTarget = Convert.ToDouble(Target_Alt.Text);

            string Charge = Charge_Selector.Text;
            string Shell = Shell_Selector.Text;

            bool isTopDown = Convert.ToBoolean(High_Arc_Checkbox.IsChecked);

            double elevation, tof, range, bearing, altDiff, initSpeed, exitAngle, apex, dist;
            range = Logic.getRange(xBattery, yBattery, xTarget, yTarget);
            bearing = Logic.getBearing(xBattery, yBattery, xTarget, yTarget);
            altDiff = Logic.getAltitudeDiff(altBattery, altTarget);
            Artillery Arty = Array.Find(ArtilleryList, item => item.Name == Artillery);
            FireMode fMode = Array.Find(Arty.modes, item => item.name == Charge);
            ShellType shell = Array.Find(Arty.shellTypes, item => item.name == Shell);
            initSpeed = shell.initSpeed * fMode.artilleryCharge;

            double angleCorrection = 0;
            if (!String.IsNullOrEmpty(AngleCorrection.Text))
            {
                angleCorrection = double.Parse(AngleCorrection.Text);
            }

            (elevation, tof, exitAngle, apex, dist) = Logic.getAngleSolutionForRange2(range, initSpeed, altDiff, Arty, shell, isTopDown);

            double dispersionElevation = elevation + 0.2;
            double dispersionDistance = 0;
            double filler = 0;
            (dispersionDistance, filler, filler, filler) = Logic.simulateForAngle2(initSpeed, Logic.toRadians(dispersionElevation), Arty, shell, altDiff);
            elevation += angleCorrection;
            SolutionTextBox.Text = $"Solution for {Charge} of {Arty.Name}\nElevation: {elevation:F3}, Bearing: {bearing:F3}\nRange: {range:F3}, ToF: {tof:F2}, exitAngle: {exitAngle:F2}\nDistnc: {dist:f3}, apex: {apex:F3}\n+0.2 Elevation = Range: {dispersionDistance - range:f3}\n-0.07 Elevation = {elevation - 0.07:f3} ";

            string coordinatesText = $"{Battery_X.Text},{Battery_Y.Text},{Battery_Alt.Text}/{Target_X.Text},{Target_Y.Text},{Target_Alt.Text}/{Charge}";


            if (logItems.Count >= 10)
            {
                logItems.RemoveAt(9);
            }
            logItems.Insert(0, new TargetLogItem() { CoordLog = coordinatesText });
            icSolutionLog.Items.Refresh();
        }

        private void Load_Button_Click(object sender, RoutedEventArgs e)
        {
            var siblings = ((sender as FrameworkElement).Parent as Panel).Children;
            var textBlock = siblings.OfType<TextBlock>().First();
            string text = textBlock.Text;

            string[] coords = text.Split('/');
            string[] coordsLeft = coords[0].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] coordsRight = coords[1].Split(',');
            string Charge = coords[2];

            Battery_X.Text = coordsLeft[0];
            Battery_Y.Text = coordsLeft[1];
            Battery_Alt.Text = coordsLeft[2];

            Target_X.Text = coordsRight[0];
            Target_Y.Text = coordsRight[1];
            Target_Alt.Text = coordsRight[2];

            foreach (var item in Charge_Selector.Items)
            {
                if (item.ToString() == Charge)
                {
                    Charge_Selector.SelectedItem = item;
                }
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Regex regex = new Regex("^[0-9]{0,4}$");
            string fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);

            e.Handled = !regex.IsMatch(fullText);
        }

        private void CorrectionValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Regex regex = new Regex("^-?[0-9]*[,]?[0-9]*$");
            string fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);

            e.Handled = !regex.IsMatch(fullText);
        }

        private void Compute_Table_Click(object sender, RoutedEventArgs e)
        {
            bool considerAltitude = Convert.ToBoolean(Altitude_Checkbox.IsChecked);
            double angleStep = 0.1;
            string Charge = Charge_Selector.Text;
            string Shell = Shell_Selector.Text;
            string Artillery = Artillery_Selector.Text;

            Artillery Arty = Array.Find(ArtilleryList, item => item.Name == Artillery);
            FireMode fMode = Array.Find(Arty.modes, item => item.name == Charge);
            ShellType shell = Array.Find(Arty.shellTypes, item => item.name == Shell);
            List<SimulatedAngle> angleList = new List<SimulatedAngle>();

            double altBattery, altTarget, altDiff = 0;

            if (considerAltitude)
            {
                altBattery = Convert.ToDouble(Battery_Alt.Text);
                altTarget = Convert.ToDouble(Target_Alt.Text);
                altDiff = Logic.getAltitudeDiff(altBattery, altTarget);
            }

            bool isEmptyInputCondition = String.IsNullOrEmpty(Battery_Alt.Text) || String.IsNullOrEmpty(Target_Alt.Text);

            for (double i = Arty.minAngle; i < Arty.maxAngle; i += angleStep)
            {
                //if (Math.Round(i, 1) % 5 != 0)
                //{
                //    continue;
                //}

                double range, tof, exitAngle, apex;
                if (considerAltitude && !isEmptyInputCondition)
                {
                    (range, tof, exitAngle, apex) = Logic.simulateForAngle2(shell.initSpeed * fMode.artilleryCharge, Logic.toRadians(i), Arty, shell, altDiff);
                }
                else
                {
                    (range, tof, exitAngle, apex) = Logic.simulateForAngle2(shell.initSpeed * fMode.artilleryCharge, Logic.toRadians(i), Arty, shell);
                }


                angleList.Add(new SimulatedAngle() { Angle = Math.Round(i, 2), Range = Math.Round(range, 2), ExitAngle = Math.Round(exitAngle, 2), Tof = Math.Round(tof, 2), Apex = Math.Round(apex, 2) });
            }
            ArtilleryTableGrid.ItemsSource = angleList;
        }

        private void Artillery_Selector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(Artillery_Selector.Text)) return;
            Shell_Selector.Items.Clear();
            Charge_Selector.Items.Clear();
            string Artillery = e.AddedItems[0].ToString();
            Artillery Arty = Array.Find(ArtilleryList, item => item.Name == Artillery);

            Array.ForEach(Arty.shellTypes, item => Shell_Selector.Items.Add(item.name));
            Shell_Selector.SelectedItem = Shell_Selector.Items[0];

            Array.ForEach(Arty.modes, item => Charge_Selector.Items.Add(item.name));
            Charge_Selector.SelectedItem = Charge_Selector.Items[0];
        }

        private void Open_Map_Button_Click(object sender, RoutedEventArgs e)
        {
            MapWindow mapWindow = new MapWindow();
            mapWindow.Show();

        }
    }
}
