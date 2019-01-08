using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PivotApplication
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        public static class PersonService
        {
            public static List<Person> ReadFile(string filepath)
            {
                var lines = File.ReadAllLines(filepath);

                var data = from l in lines.Skip(1)
                           let split = l.Split(',','\t')
                           select new Person
                           {
                               SYMBOL = split[1],
                               EXPIRYDATE = Convert.ToDateTime(split[2]),
                               HIGH = double.Parse(split[4]),
                               LOW = double.Parse(split[5]),
                               CLOSE = double.Parse(split[6]),
                               ID = Convert.ToInt32(split[8]),
                           };

                return data.ToList();
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

                //DeleteRowFromGrid();
                IList rows = DataGridTest.SelectedItems;
                PopulateGridData(((Person)rows[0]).SYMBOL, ((Person)rows[0]).HIGH, ((Person)rows[0]).LOW, ((Person)rows[0]).CLOSE);
            }
            catch (Exception)
            {
            }
        }

        private void PopulateGridData(string sYMBOL, double hIGH, double lOW, double cLOSE)
        {
            // double pivotPoint = (hIGH + cLOSE + lOW) / 3;
            double pivotPoint = (hIGH + cLOSE + lOW) / 3;
           
            //TODO:check for case 1:
            double diffPivot = Math.Abs(cLOSE - pivotPoint);
            if (diffPivot > cLOSE * .5 / 100 && diffPivot - cLOSE * 1 / 100 < 0.09 )
            {
                //It's valid pivot
                //now we have 2 condition 

                //TODO:check for case 2:
                if (pivotPoint < cLOSE)
                {
                    NewMethod(pivotPoint, hIGH, "BUY", "SELL", sYMBOL,cLOSE);
                }
                else
                {

                    NewMethod(pivotPoint, lOW, "SELL", "BUY", sYMBOL, cLOSE);
                }
            }
            else
            {
                //remove data form grid.

                System.Windows.MessageBox.Show("Pivot point is not valid for today");
            }
        }
        public void NewMethod(double sellValue, double buyVlaue, string temp1, string temp2, string scriptName,double close)
        {
            //then high will become buy and pivot point will become sell 
            //for BUY
            string strMessage = "-----" + scriptName + "-----\n";
            strMessage += "Pivot Point is: " + sellValue.ToString()+"\n";;
            strMessage += "--------------------------------\n";
            strMessage += temp1 + "\n"; ;
            strMessage += "STOP LOSS = " + close + "\n";
            double pointValue = buyVlaue - close;
            strMessage += "Entry Point:" + buyVlaue + "\n"; ;
            strMessage += "Point Value = " + pointValue + "\n"; ;
            strMessage += "Target1 = " + (buyVlaue + pointValue) + "\n";
            strMessage += "Target2 = " + (buyVlaue + 2 * pointValue) + "\n";
            strMessage += "Target3 = " + (buyVlaue + 3 * pointValue) + "\n";
            strMessage += "--------------------------------\n";
            strMessage += temp2 + "\n";
            strMessage += "STOP LOSS = " + close + "\n";
            pointValue = sellValue - close;
            strMessage += "Entry Point:" + sellValue + "\n"; ;
            strMessage += "Point Value = " + pointValue + "\n"; ;
            strMessage += "Target1 = " + (sellValue + pointValue) + "\n";
            strMessage += "Target2 = " + (sellValue + (2 * pointValue)) + "\n";
            strMessage += "Target3 = " + (sellValue + (3 * pointValue)) + "\n";
            strMessage += "--------------------------------\n";
            System.Windows.MessageBox.Show("" + strMessage);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //using (WebClient wc = new WebClient())
            //{
            //    wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadFileCompleted);
            //    wc.DownloadFileAsync(new Uri(("https://www.nseindia.com/content/historical/EQUITIES/2017/JUL/cm11JUL2017bhav.csv")), @"c:\temp\myfile.csv");
            //}
            //DataContext = PersonService.ReadFile(@"c:\temp\test.csv");
            //DataContext = PersonService.ReadFile(@"c:\temp\test.csv");
        }

        private void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("Download complete");
               
        }

        int limitTo = 0;
        int limitFrom = 0;
        private void DeleteRowFromGrid()
        {
            DataGridTest.Items.Clear();

            for (int i = 1; i < dgBhavCopy.Items.Count; i++)
            {//high close low
                try
                {

                    double pivotPoint = (Convert.ToDouble(((Person)dgBhavCopy.Items[i]).HIGH) + Convert.ToDouble(((Person)dgBhavCopy.Items[i]).CLOSE) + Convert.ToDouble(((Person)dgBhavCopy.Items[i]).LOW)) / 3;

                    //TODO:check for case 1:
                    double diffPivot = Math.Abs(Convert.ToDouble(((Person)dgBhavCopy.Items[i]).CLOSE) - pivotPoint);
                    if (diffPivot > Convert.ToDouble(((Person)dgBhavCopy.Items[i]).CLOSE) * .5 / 100 && 
                        diffPivot - Convert.ToDouble(((Person)dgBhavCopy.Items[i]).CLOSE) * 1 / 100 < 0.09
                        && ((Person)dgBhavCopy.Items[i]).ID > 20000)
                    {
                        //TODO:condition for avoiding small stop loss
                        //if (Convert.ToDouble(((Person)dgBhavCopy.Items[i]).LOW) > limitFrom 
                        //    && Convert.ToDouble(((Person)dgBhavCopy.Items[i]).LOW) < limitTo)
                        {
                            var data = new Person
                            {
                                SYMBOL = ((Person)dgBhavCopy.Items[i]).SYMBOL,
                                HIGH = ((Person)dgBhavCopy.Items[i]).HIGH,
                                LOW = ((Person)dgBhavCopy.Items[i]).LOW,
                                CLOSE = ((Person)dgBhavCopy.Items[i]).CLOSE
                            };

                            DataGridTest.Items.Add(data);
                        }
                    }
                    else
                    {

                    }
                }
                catch (Exception)
                {

                }
                lblQualifiedRow.Content = "Total Qualified row :"+ DataGridTest.Items.Count;
            }
            dgBhavCopy.Visibility = Visibility.Hidden;
        }

        public void UpdateCSV()
        {
            string[] values = File.ReadAllLines(@"c:\temp\test.csv");
            StreamWriter Writer = new StreamWriter(@"c:\temp\testNew.csv", false);
            for (int i = 1; i < values.Length; i++)
            {
                //replacing null with " " (space) 
                //check if pivot is valid
                //high close low
                double pivotPoint = (Convert.ToDouble(values[i].Split(',')[3]) + Convert.ToDouble(values[i].Split(',')[4]) + Convert.ToDouble(values[i].Split(',')[5]) / 3);

                //TODO:check for case 1:
                double diffPivot = Math.Abs(Convert.ToDouble(values[i].Split(',')[4]) - pivotPoint);
                if (diffPivot > Convert.ToDouble(values[i].Split(',')[4]) * .5 / 100 && diffPivot - Convert.ToDouble(values[i].Split(',')[4]) * 1 / 100 < 0.09)
                {
                    //Writer.WriteLine(values[i].Replace(" "));
                }
                else
                {
                    //string[] temp = values[i].Split(',');
                    //for (int j = temp.Length-1; j >=0 ; j--)
                    //{
                    //    try
                    //    {
                    //        Writer.WriteLine(values[i].Replace(temp[j], ""));
                    //    }
                    //    catch (Exception)
                    //    {

                    //    }
                    //}
                }
            }

                Writer.Close();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            //string selectedFolder = string.Empty;
            //FolderBrowserDialog selectFolderDialog = new FolderBrowserDialog();
            //selectFolderDialog.ShowNewFolderButton = true;
            //if (selectFolderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    selectedFolder = selectFolderDialog.SelectedPath;
            //    DataContext = PersonService.ReadFile(selectedFolder);
            //}

            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".csv";
            dlg.Filter = "CSV Files (*.csv)|*.csv" ;//|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                DataContext = PersonService.ReadFile(dlg.FileName);
            }

            try
            {
                //limitFrom = Convert.ToInt32(textBox.Text);
                //limitTo = Convert.ToInt32(textBox1.Text);
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show("Please set value for limitfrom and limit to textbox(or 0,0)");
                return;
            }
            DeleteRowFromGrid();
        }
    }
}
