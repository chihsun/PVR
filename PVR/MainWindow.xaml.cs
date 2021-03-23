using System;
using System.Collections.Generic;
using System.IO;
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

namespace PVR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region FIELD
        public string ABIReport;
        public ABIPVR PVRData = new();
        #endregion
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BT_Clear_Click(object sender, RoutedEventArgs e)
        {
            TraverseVisualTree(My_Window);
            PVRData = new ABIPVR();
        }

        private void My_Window_Loaded(object sender, RoutedEventArgs e)
        {
            Age.Focus();
        }

        private void Report_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"\ABIReport.txt"))
                ABIReport = File.ReadAllText(System.AppDomain.CurrentDomain.BaseDirectory + @"\ABIReport.txt");
            else
            {
                MessageBox.Show("找不到範例檔", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return;
            }
            if (int.TryParse(RSBP.Text, out PVRData.rsbp)
                & int.TryParse(LSBP.Text, out PVRData.lsbp)
                & int.TryParse(RDBP.Text, out PVRData.rdbp)
                & int.TryParse(LDBP.Text, out PVRData.ldbp))
            {
                if (PVRData.bp_p)
                    ABIReport = ABIReport.Replace("<<UBP>>", "Blood pressure is difference more than 15 mmHg between bilateral brachial blood pressure");
                else
                    ABIReport = ABIReport.Replace("<<UBP>>", "No obvious difference between bilateral brachial blood pressure");
            }
            else if (PVRData.rsbp <= 0 && PVRData.lsbp <= 0 && PVRData.rdbp <= 0 && PVRData.ldbp <= 0)
                ABIReport = ABIReport.Replace("<<UBP>>", "No obvious difference between bilateral brachial blood pressure").Replace("Right & Left: <<RBP>> mmHg　&　<<LBP>> mmHg", string.Empty);
            else
                ABIReport = ABIReport.Replace("<<UBP>>", string.Empty);
            ABIReport = ABIReport.Replace("<<RBP>>", string.Format($"{(PVRData.rsbp > 0 && PVRData.rdbp > 0 ? PVRData.rsbp.ToString() + "/" + PVRData.rdbp.ToString() : "---/---")}"));
            ABIReport = ABIReport.Replace("<<LBP>>", string.Format($"{(PVRData.lsbp > 0 && PVRData.ldbp > 0 ? PVRData.lsbp.ToString() + "/" + PVRData.ldbp.ToString() : "---/---")}")); 

            ABIReport = ABIReport.Replace("<<RABI>>", PVRData.rabi_r !=0 ? string.Format("{0:0.00}", PVRData.rabi_r) : "---").Replace("<<LABI>>", PVRData.labi_r !=0 ? string.Format("{0:0.00}", PVRData.labi_r) : "---");

            ABIReport = ABIReport.Replace("<<RPWV>>", PVRData.rpwv_r != 0 ? PVRData.rpwv_r.ToString() : "----").Replace("<<LPWV>>", PVRData.lpwv_r != 0 ? PVRData.lpwv_r.ToString() : "----");

            ABIReport = ABIReport.Replace("<<RPVR>>", PVRData.rpvr_r > 0 ? PVRData.rpvr_r.ToString() : "--").Replace("<<LPVR>>", PVRData.lpvr_r > 0 ? PVRData.lpvr_r.ToString() : "--");

            ABIReport = ABIReport.Replace("<<RUT>>", PVRData.rut > 0 ? PVRData.rut.ToString() : "---").Replace("<<LUT>>", PVRData.lut > 0 ? PVRData.lut.ToString() : "---");
            /*
             * Conclusion
             */
            List<string> Conclusion = new();
            if (!string.Equals(PVRData.abiconclude(PVRData.rabi_r), "Normal"))
                Conclusion.Add(PVRData.abiconclude(PVRData.rabi_r) + " over right lower limbs");
            if (!string.Equals(PVRData.abiconclude(PVRData.labi_r), "Normal"))
                Conclusion.Add(PVRData.abiconclude(PVRData.labi_r) + " over left lower limbs");
            if (Conclusion.Count == 0)
            {
                if (PVRData.rpwv_stiff && PVRData.lpwv_stiff)
                    Conclusion.Add("Arterial stiffness over both lower limbs");
                else if (PVRData.rpwv_stiff)
                    Conclusion.Add("Arterial stiffness over right lower limbs");
                else if (PVRData.lpwv_stiff)
                    Conclusion.Add("Arterial stiffness over left lower limbs");
            }
            if (Conclusion.Count == 0)
                Conclusion.Add("No evidence of arterial stenosis over lower limbs");
            ABIReport = ABIReport.Replace("<<Conclusion>>", string.Join(", ", Conclusion));
            /*
            if (PVRData.rabi_stenosis || PVRData.labi_stenosis)
            {
                if (PVRData.rabi_stenosis && PVRData.labi_stenosis)
                {
                    ABIReport = ABIReport.Replace("<<Conclusion>>", "Arterial stenosis over both lower limbs");
                }
                else if (PVRData.rabi_stenosis)
                {
                    ABIReport = ABIReport.Replace("<<Conclusion>>", "Arterial stenosis over right lower limbs");
                }
                else
                {
                    ABIReport = ABIReport.Replace("<<Conclusion>>", "Arterial stenosis over left lower limbs");
                }
            }
            else if (PVRData.rpwv_stiff || PVRData.lpwv_stiff)
            {
                if (PVRData.rpwv_stiff && PVRData.lpwv_stiff)
                {
                    ABIReport = ABIReport.Replace("<<Conclusion>>", "Arterosclerotic change or Arterial stiffness over both lower limbs");
                }
                else if (PVRData.rpwv_stiff)
                {
                    ABIReport = ABIReport.Replace("<<Conclusion>>", "Arterosclerotic change or Arterial stiffness over right lower limbs");
                }
                else
                {
                    ABIReport = ABIReport.Replace("<<Conclusion>>", "Arterosclerotic change or Arterial stiffness over left lower limbs");
                }
            }
            else
            {
                ABIReport = ABIReport.Replace("<<Conclusion>>", "No evidence of arterial stenosis over lower limbs");
            }
            */
            if (!Directory.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"db"))
                Directory.CreateDirectory(System.AppDomain.CurrentDomain.BaseDirectory + @"db");
            //string PtTpr = JsonConvert.SerializeObject(HUTRP, Formatting.Indented);
            //File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + @"db\" + HUTRP.ID.ToString() + "_" + DateTime.Now.ToString("yyyyMMdd") + ".json", PtTpr);
            File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + @"db\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt", ABIReport);

            // Copy Final Comment to Clipboard
            try
            {
                //Clipboard.SetText(GetAbb(textRange.Text.Trim()));
                Clipboard.SetDataObject(ABIReport.Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            MessageBox.Show("匯出成功", "匯出", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
        }

        private void BTN_TOP_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = !this.Topmost;
            if (this.Topmost)
                BTN_TOP.Background = Brushes.LightBlue;
            else
                BTN_TOP.Background = SystemColors.WindowBrush;
        }
    }
    public class ABIPVR
    {
        public int ID;
        public int Age;
        public int rsbp;
        public int rdbp;
        public int rrr;
        public int lsbp;
        public int ldbp;
        public int lrr;
        public double rabi_r;
        public double labi_r;
        public int rpwv_r;
        public int lpwv_r;
        public int rpvr_r;
        public int lpvr_r;
        public int rut;
        public int lut;
        public bool bp_p
        {
            get
            {
                if (rsbp <= 0 || lsbp <= 0 || rdbp <= 0 || ldbp <= 0)
                    return false;
                if (Math.Abs(rsbp - lsbp) >= 15 || Math.Abs(rdbp - ldbp) >= 15)
                    return true;
                return false;
            }
        }
        public bool rabi_stenosis
        {
            get
            {
                if (rabi_r > 0 && rabi_r <= 0.9)
                    return true;
                return false;
            }
        }
        public bool labi_stenosis
        {
            get
            {
                if (labi_r > 0 && labi_r <= 0.9)
                    return true;
                return false;
            }
        }
        public string abiconclude(double abi_result)
        {
            string result;
            switch (abi_result)
            {
                case <= 0:
                    result = "Not Check";
                    break;
                case <= 0.4:
                    result = "Severe stenosis";
                    break;
                case <= 0.7:
                    result = "Moderate stenosis";
                    break;
                case <= 0.9:
                    result = "Mild stenosis";
                    break;
                case > 1.4:
                    result = "Calcified vessels";
                    break;
                default:
                    result = "Normal";
                    break;
            }
            return result;
        }
        public bool rpwv_stiff
        {
            get
            {
                if (Age <= 0)
                    return false;
                if (Age < 40 && rpwv_r > 1400)
                    return true;
                if (Age >= 40 && Age < 50 && rpwv_r > 1600)
                    return true;
                if (Age >= 50 && Age < 60 && rpwv_r > 1800)
                    return true;
                if (Age >= 60 && Age < 70 && rpwv_r > 2000)
                    return true;
                if (Age > 70 && rpwv_r > 2200)
                    return true;
                if (rpvr_r >= 45)
                    return true;
                if (rut >= 180)
                    return true;
                return false;
            }
        }
        public bool lpwv_stiff
        {
            get
            {
                if (Age <= 0)
                    return false;
                if (Age < 40 && lpwv_r > 1400)
                    return true;
                if (Age >= 40 && Age < 50 && lpwv_r > 1600)
                    return true;
                if (Age >= 50 && Age < 60 && lpwv_r > 1800)
                    return true;
                if (Age >= 60 && Age < 70 && lpwv_r > 2000)
                    return true;
                if (Age > 70 && lpwv_r > 2200)
                    return true;
                if (lpvr_r >= 45)
                    return true;
                if (lut >= 180)
                    return true;
                return false;
            }
        }
    }
}
