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

    public partial class MainWindow : Window    
    {
        List<float> ScoresList = new List<float>();
        int RunNum = 1;
        string Prime = "2048000000";
        bool IsRunning = false;

        public void Exec(string Value, List<float>ScoresList)
        {

            Process process = new Process();
            process.StartInfo.FileName = "PYPrime_Workload.exe";
            process.StartInfo.Arguments = Value;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            IsRunning = !IsRunning;
            process.Start();

            this.Dispatcher.Invoke(() =>
            {
                Progress.IsIndeterminate = true;
            });

            string output = process.StandardOutput.ReadToEnd();       
            float Score = float.Parse(output.Split()[1]);
            

            if (int.Parse(output.Split()[0]) != 2047999957)
            {
                MessageBox.Show("Output Invalid!");
            }
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    Scores.Items.Add($"Run completed in: {Score} s");
                });

                ScoresList.Add(Score);
            }

            process.WaitForExit();
            IsRunning = !IsRunning;
            this.Dispatcher.Invoke(() =>
            {
                Progress.IsIndeterminate = false;
            });
        }

        public MainWindow()
        {
            InitializeComponent();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (IsRunning == false)
            {
                Thread thread = new Thread(() => Exec(Prime, ScoresList));               
                thread.Start();
            }
            else
            {
                MessageBox.Show("Thread Already running!");
            }


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
