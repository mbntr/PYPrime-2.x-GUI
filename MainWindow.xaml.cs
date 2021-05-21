using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace PYPrime_GUI
{

    public class PYPrime
    {
        public static string Exec(string Value)
        {
            Process process = new Process();
            process.StartInfo.FileName = "PYPrime_Workload.exe";
            process.StartInfo.Arguments = Value;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return output;

        }
    }

    public partial class MainWindow : Window    
    {
        List<float> ScoresList = new List<float>();
        int RunNum = 1;
        string Prime = "2048000000";

        public MainWindow()
        {
            InitializeComponent();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            float Score = float.Parse(PYPrime.Exec(Prime).Split()[1]);

            Scores.Items.Add($"Run {RunNum} completed in: {Score} s");
            ScoresList.Add(Score);
            RunNum = RunNum + 1;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)

        {
            try
            {
                float Mean = ScoresList.Average();
                MessageBox.Show($"Average completion Time: { Math.Round(Mean, 3).ToString()} s");
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("No Results!");

            }
            
        }
    }
}
