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

    public partial class MainWindow    
    {

        List<float> ScoresList = new List<float>();
        int RunNum = 0;
        string Prime = "2048000000";
        string StressPrime = "32768000000";
        long ExpValB = 2047999957;
        long ExpValS = 32767999997;
        bool IsRunning = false;

        async Task Exec(string Value, List<float> ScoresList, long ExpVal)

        {
            Process process = new Process();
            process.StartInfo.FileName = "python-3.9.5-embed-amd64/PYPrime_Workload.exe";
            process.StartInfo.Arguments = Value;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            IsRunning = !IsRunning;
            process.Start();
            process.WaitForExit();

            string output = process.StandardOutput.ReadToEnd();       
            float Score = float.Parse(output.Split()[1]);
            

            if (int.Parse(output.Split()[0]) != ExpVal)
            {
                MessageBox.Show("Output Invalid!");
            }
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    Scores.Items.Add($"Run {RunNum} completed in: {Score} s");
                });

                ScoresList.Add(Score);
            }

            
            IsRunning = !IsRunning;
            this.Dispatcher.Invoke(() =>
            {
                Progress.IsIndeterminate = false;
            });

        }

        public MainWindow()
        {
            InitializeComponent();
            SourceChord.FluentWPF.ResourceDictionaryEx.GlobalTheme = SourceChord.FluentWPF.ElementTheme.Light;
        }


        private async void Start_Click(object sender, RoutedEventArgs e)
        {

            if (IsRunning == false)
            {
                RunNum++;
                if (Loop.IsChecked == true)
                {
                    while (Loop.IsChecked == true)
                    {
                        if (PerfMode.IsChecked == false)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                Progress.IsIndeterminate = true;
                            });
                        }

                        else
                        { }
                        await Task.Factory.StartNew(() => Exec(Prime, ScoresList, ExpValB));
                    }
                }
                else if (Stress.IsChecked == true)
                {
                    Loop.IsChecked = true;
                    while (Loop.IsChecked == true)
                    {
                        if (PerfMode.IsChecked == false)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                Progress.IsIndeterminate = true;
                            });
                        }

                        else
                        { }
                        await Task.Factory.StartNew(() => Exec(StressPrime, ScoresList, ExpValS));
                    }
                }

                else
                {
                    if (PerfMode.IsChecked == false)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            Progress.IsIndeterminate = true;
                        });
                    }

                    else
                    { }
                    await Task.Factory.StartNew(() => Exec(Prime, ScoresList, ExpValB));                  
                }


            }

            else
            {
                MessageBox.Show("Thread Already running!");
            }

        }

        void Mean_Click(object sender, RoutedEventArgs e)

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

        private void Window_Closing(object sender, EventArgs e)
        {
            foreach (var process in Process.GetProcessesByName("PYPrime_Workload"))
            {
                process.Kill();
            }
        }        

    }   

}