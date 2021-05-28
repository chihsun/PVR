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
using Tesseract;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace PVR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region FIELD
        public string ABIReport;
        public ABIPVR PVRData = new();
        #endregion
        public MainWindow()
        {
            InitializeComponent();
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;
        }

        private void BT_Clear_Click(object sender, RoutedEventArgs e)
        {
            TraverseVisualTree(My_Window);
            PVRData = new ABIPVR();
            Age.Focus();
        }

        private void My_Window_Loaded(object sender, RoutedEventArgs e)
        {
            Age.Focus();
            Loaddatabase();
            if (alldata.Count > 0)
            {
                var datedata = alldata.Select(x => x.StudyTime.ToString("yyyyMMdd")).ToList().Distinct().ToList();
                datedata.Sort((x, y) => -x.CompareTo(y));
                ComB_Date.ItemsSource = datedata;
                ComB_Date.SelectedIndex = 0;
            }
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
            if (int.TryParse(RSBP.Text, out PVRData.RSBP)
                & int.TryParse(LSBP.Text, out PVRData.LSBP)
                & int.TryParse(RDBP.Text, out PVRData.RDBP)
                & int.TryParse(LDBP.Text, out PVRData.LDBP))
            {
                if (PVRData.bp_p)
                    ABIReport = ABIReport.Replace("<<UBP>>", "Blood pressure is difference more than 15 mmHg between bilateral brachial blood pressure");
                else
                    ABIReport = ABIReport.Replace("<<UBP>>", "No obvious difference between bilateral brachial blood pressure");
            }
            else if (PVRData.RSBP <= 0 && PVRData.LSBP <= 0 && PVRData.RDBP <= 0 && PVRData.LDBP <= 0)
                ABIReport = ABIReport.Replace("<<UBP>>", "No obvious difference between bilateral brachial blood pressure").Replace("Right & Left: <<RBP>> mmHg　&　<<LBP>> mmHg", string.Empty);
            else
                ABIReport = ABIReport.Replace("<<UBP>>", string.Empty);
            ABIReport = ABIReport.Replace("<<RBP>>", string.Format($"{(PVRData.RSBP > 0 && PVRData.RDBP > 0 ? PVRData.RSBP.ToString() + "/" + PVRData.RDBP.ToString() : "---/---")}"));
            ABIReport = ABIReport.Replace("<<LBP>>", string.Format($"{(PVRData.LSBP > 0 && PVRData.LDBP > 0 ? PVRData.LSBP.ToString() + "/" + PVRData.LDBP.ToString() : "---/---")}"));

            ABIReport = ABIReport.Replace("<<RABI>>", PVRData.rabi_r != 0 ? string.Format("{0:0.00}", PVRData.rabi_r) : "---").Replace("<<LABI>>", PVRData.labi_r != 0 ? string.Format("{0:0.00}", PVRData.labi_r) : "---");

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
        private void BTN_OCR_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                InitialDirectory = Environment.CurrentDirectory,
                Title = "選取圖檔",
                Filter = "JPEG files (*.jpg;*.png)|*.jpg;*.png"
            };
            if (dlg.ShowDialog() == true)
            {
                var imgPath = dlg.FileName;
                IImageFormat format;
                SixLabors.ImageSharp.Image img = SixLabors.ImageSharp.Image.Load(imgPath, out format);
                var clone = img.Clone(
                        i => i.Crop(new SixLabors.ImageSharp.Rectangle(900, 200, 300, 90)));
                var engine = new TesseractEngine("tessdata", "eng", EngineMode.Default);
                var ms = new MemoryStream();
                clone.Save(ms, format);
                string strResult = engine.Process(Pix.LoadFromMemory(ms.ToArray())).GetText();
                //string strResult = ImageToText(imgPath);

                if (!string.IsNullOrEmpty(strResult))
                {
                    Clipboard.SetDataObject(strResult.Trim());
                }
            }
        }

        public string ImageToText(string imgPath)
        {
            using (var engine = new TesseractEngine("tessdata", "eng", EngineMode.Default))
            {
                using (var img = Pix.LoadFromFile(imgPath))
                {
                    using (var page = engine.Process(img))
                    {
                        return page.GetText();
                    }
                }
            }
        }
        public List<ABIPVR> alldata = new();
        private void BTN_ALL_Click(object sender, RoutedEventArgs e)
        {
            //string Sample = String.Empty;
            string[] FileCollection = Directory.GetFiles(Environment.CurrentDirectory + @"\data", "*.csv", SearchOption.AllDirectories);
            if (FileCollection.Length <= 0)
                return;
            //List<string> FileNames = new List<string>();
            //foreach (var x in FileCollection)
            //    FileNames.Add(System.IO.Path.GetFileName(x));
            //Sample = File.ReadAllText("Sample.txt");
            alldata.Clear();
            Loaddatabase();
            for (int i = 0; i < FileCollection.Length; i++)
            {            
                if (!DateTime.TryParseExact(System.IO.Path.GetFileName(FileCollection[i]).Split('_')[1]
                    + System.IO.Path.GetFileName(FileCollection[i]).Split('_')[2].Replace(".csv", ""),
                    "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime studytime))
                    continue;
                if (!int.TryParse(System.IO.Path.GetFileName(FileCollection[i]).Split('_')[0], out int id))
                    continue;
                if (alldata.FirstOrDefault(x => x.ID == id && x.StudyTime == studytime) != null)
                    continue;
                if ((DateTime.Now - studytime).TotalDays >= 30)
                    continue;
                string[] newabi = File.ReadAllText(FileCollection[i]).Split(',');
                if (newabi.Length < 17)
                    continue;
                var imgPath = FileCollection[i].Replace("csv", "jpg");
                IImageFormat format;
                var img = SixLabors.ImageSharp.Image.Load(imgPath, out format);
                var clone = img.Clone(
                        i => i.Crop(new SixLabors.ImageSharp.Rectangle(900, 200, 300, 90)));
                var ms = new MemoryStream();
                clone.Save(ms, format);
                var engine = new TesseractEngine("tessdata", "eng", EngineMode.Default);
                string strResult = engine.Process(Pix.LoadFromMemory(ms.ToArray())).GetText();
                var match = Regex.Match(strResult, @"\d+");
                if (!match.Success)
                    continue;
                if (!int.TryParse(match.Groups[0].Value, out int age) || age < 10)
                    continue;
                ABIPVR abidata = new()
                {
                    rpwv_r = int.TryParse(newabi[0], out int count) ? count : 0,
                    lpwv_r = int.TryParse(newabi[1], out count) ? count : 0,
                    RSBP = double.TryParse(newabi[5], out double counts) ? Convert.ToInt32(Math.Round(counts, 0, MidpointRounding.AwayFromZero)) : 0,
                    RDBP = double.TryParse(newabi[6], out counts) ? Convert.ToInt32(Math.Round(counts, 0, MidpointRounding.AwayFromZero)) : 0,
                    LSBP = double.TryParse(newabi[8], out counts) ? Convert.ToInt32(Math.Round(counts, 0, MidpointRounding.AwayFromZero)) : 0,
                    LDBP = double.TryParse(newabi[9], out counts) ? Convert.ToInt32(Math.Round(counts, 0, MidpointRounding.AwayFromZero)) : 0,
                    RSBP_A = double.TryParse(newabi[11], out counts) ? Convert.ToInt32(Math.Round(counts, 0, MidpointRounding.AwayFromZero)) : 0,
                    RDBP_A = double.TryParse(newabi[12], out counts) ? Convert.ToInt32(Math.Round(counts, 0, MidpointRounding.AwayFromZero)) : 0,
                    LSBP_A = double.TryParse(newabi[14], out counts) ? Convert.ToInt32(Math.Round(counts, 0, MidpointRounding.AwayFromZero)) : 0,
                    LDBP_A = double.TryParse(newabi[15], out counts) ? Convert.ToInt32(Math.Round(counts, 0, MidpointRounding.AwayFromZero)) : 0,
                    rabi_r = double.TryParse(newabi[3], out counts) ? counts : 0,
                    labi_r = double.TryParse(newabi[4], out counts) ? counts : 0,
                    StudyTime = studytime,
                    ID = id,
                    Age = age
                };
                alldata.Add(abidata);
                /*
                List<double> abidata = new List<double>();
                for (int j = 0; j < newabi.Length; j++)
                {
                    if (j <= 4)
                        abidata.Add(Convert.ToDouble(newabi[j]));
                    else
                        abidata.Add(Math.Round(Convert.ToDouble(newabi[j]), 0, MidpointRounding.AwayFromZero));
                }
                /*
                ///if (i == 0)
                ///    rTB_00.Document.Blocks.Clear();
                if (abidata.Count < 17)
                    continue;
                string Result = Sample.Replace("<<RABI>>", abidata[3].ToString()).Replace("<<LABI>>", abidata[4].ToString()).Replace("<<RPWV>>", abidata[0].ToString()).Replace("<<LPWV>>", abidata[1].ToString());
                //Result = Result.Replace("<<RBSBP>>", String.Format("{0:000}", (int)abidata[5])).Replace("<<RBDBP>>", String.Format("{0:000}", (int)abidata[7])).Replace("<<LBSBP>>", String.Format("{0:000}", (int)abidata[8])).Replace("<<LBDBP>>", String.Format("{0:000}", (int)abidata[10]));
                Result = Result.Replace("<<RBSBP>>", String.Format("{0,3}", abidata[5].ToString())).Replace("<<RBDBP>>", String.Format("{0,3}", abidata[7].ToString())).Replace("<<LBSBP>>", String.Format("{0,3}", abidata[8].ToString())).Replace("<<LBDBP>>", String.Format("{0,3}", abidata[10].ToString()));
                Result = Result.Replace("<<RASBP>>", String.Format("{0,3}", abidata[11].ToString())).Replace("<<RADBP>>", String.Format("{0,3}", abidata[13].ToString())).Replace("<<LASBP>>", String.Format("{0,3}", abidata[14].ToString())).Replace("<<LADBP>>", String.Format("{0,3}", abidata[16].ToString()));
                string comment = String.Empty;
                if (abidata[3] <= 0.9 && abidata[4] <= 0.9)
                    comment = "Arteral Stenosis over bilateral lower limbs.";
                else if (abidata[3] < 0.9)
                    comment = "Arteral Stenosis over Right lower limbs";
                else if (abidata[4] <= 0.9)
                    comment = "Arteral Stenosis over Left lower limbs";
                else
                    comment = "No Evidence of Arteral Stenosis over bilateral lower limbs.";
                Result = Result.Replace("<<RESULT>>", comment);
                ///rTB_00.Document.Blocks.Add(new Paragraph(new Run(FileNames[i])));
                ///rTB_00.Document.Blocks.Add(new Paragraph(new Run(Environment.NewLine)));
                ///rTB_00.Document.Blocks.Add(new Paragraph(new Run(Result)));
                ///rTB_00.Document.Blocks.Add(new Paragraph(new Run(Environment.NewLine)));
                File.WriteAllText(FileCollection[i].Replace("csv", "txt"), Result);
                */
            }
            alldata.Sort((x, y) => x.ID.CompareTo(y.ID));
            if (alldata.Count > 0)
                MessageBox.Show("載入成功: 共載入 " + alldata.Count + "份資料。");
            Savedatabase();
        }
        private void LoadData()
        {
            if (alldata.Count <= 0 || !int.TryParse(ID.Text, out int id) || id <= 0)
                return;
            var abidata = alldata.Where(x => x.ID == id).ToList();
            if (abidata == null || abidata.Count <= 0)
                return;
            abidata.Sort((x, y) => -x.StudyTime.CompareTo(y.StudyTime));
            var abipvr = abidata.FirstOrDefault();
            if (abipvr == null)
                return;
            RSBP.Text = abipvr.RSBP.ToString();
            RDBP.Text = abipvr.RDBP.ToString();
            LSBP.Text = abipvr.LSBP.ToString();
            LDBP.Text = abipvr.LDBP.ToString();
            RSBP_A.Text = abipvr.RSBP_A.ToString();
            RDBP_A.Text = abipvr.RDBP_A.ToString();
            LSBP_A.Text = abipvr.LSBP_A.ToString();
            LDBP_A.Text = abipvr.LDBP_A.ToString();
            RABI.Text = abipvr.rabi_r.ToString();
            LABI.Text = abipvr.labi_r.ToString();
            RPWV.Text = abipvr.rpwv_r.ToString();
            LPWV.Text = abipvr.lpwv_r.ToString();
            Age.Text = abipvr.Age.ToString();
            LPWV.Focus();
        }
        private void ClearData()
        {
            RSBP.Text = RDBP.Text = LSBP.Text = LDBP.Text = RSBP_A.Text = RDBP_A.Text = LSBP_A.Text = LDBP_A.Text
                = RABI.Text = LABI.Text = RPWV.Text = LPWV.Text = string.Empty;
        }
        private void Savedatabase()
        {
            string PtTpr = JsonConvert.SerializeObject(alldata, Formatting.Indented);
            File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + @"db\database.json", PtTpr);
            File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + @"db\database_" + DateTime.Now.ToString("yyyyMMdd") + ".json", PtTpr);
        }
        private void Loaddatabase()
        {
            alldata.Clear();
            if (!File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"db\database.json"))
                return;
            alldata = JsonConvert.DeserializeObject<List<ABIPVR>>(File.ReadAllText(System.AppDomain.CurrentDomain.BaseDirectory + @"db\database.json"));
        }
        private void comB_ID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comB_ID.SelectedIndex < 0)
                return;
            ID.Text = Convert.ToInt32(comB_ID.SelectedValue).ToString();
            Age.Text = alldata.FirstOrDefault(x => x.ID == Convert.ToInt32(comB_ID.SelectedValue)).Age.ToString();
            
            var imgPath = System.AppDomain.CurrentDomain.BaseDirectory + @"data\" + ComB_Date.SelectedValue.ToString();
            string[] FileCollection = Directory.GetFiles(imgPath, "*.jpg", SearchOption.AllDirectories);
            if (FileCollection.Length <= 0)
                return;
            string ptPath = string.Empty;
            foreach (var x in FileCollection)
            {
                if (x.Contains(comB_ID.SelectedValue.ToString()))
                {
                    ptPath = x;
                }
            }
            if (ptPath == string.Empty)
                return;
            if (!File.Exists(ptPath))
                return;
            try
            {
                var img = SixLabors.ImageSharp.Image.Load(ptPath);
                var clone = img.Clone(
                        i => i.Crop(new SixLabors.ImageSharp.Rectangle(215, 1150, 420, 400)));
                var ms = new MemoryStream();
                clone.SaveAsJpeg(ms);
                ms.Position = 0;
                var imgsource = new BitmapImage();
                imgsource.BeginInit();
                imgsource.StreamSource = ms;
                imgsource.EndInit();
                IMG_00.Source = imgsource;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void ComB_Date_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            comB_ID.ItemsSource = alldata.Where(x => x.StudyTime.ToString("yyyyMMdd") == ComB_Date.SelectedValue.ToString()).Select(x => x.ID).ToList();
            comB_ID.SelectedIndex = 0;
        }
    }
    public class ABIPVR
    {
        public int ID;
        public int Age;
        public int RSBP;
        public int RDBP;
        public int RPR;
        public int LSBP;
        public int LDBP;
        public int LPR;
        public int RSBP_A;
        public int RDBP_A;
        public int RPR_A;
        public int LSBP_A;
        public int LDBP_A;
        public int LPR_A;
        public double rabi_r;
        public double labi_r;
        public int rpwv_r;
        public int lpwv_r;
        public int rpvr_r;
        public int lpvr_r;
        public int rut;
        public int lut;
        public DateTime StudyTime;
        public bool bp_p
        {
            get
            {
                if (RSBP <= 0 || LSBP <= 0 || RDBP <= 0 || LDBP <= 0)
                    return false;
                if (Math.Abs(RSBP - LSBP) >= 15 || Math.Abs(RDBP - LDBP) >= 15)
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
