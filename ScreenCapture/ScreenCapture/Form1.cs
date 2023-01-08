using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenCapture
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

        const uint MOUSEMOVE = 0x0001;      // 마우스 이동
        const uint ABSOLUTEMOVE = 0x8000;   // 전역 위치
        const uint LBUTTONDOWN = 0x0002;    // 왼쪽 마우스 버튼 눌림
        const uint LBUTTONUP = 0x0004;      // 왼쪽 마우스 버튼 떼어짐
        const uint RBUTTONDOWN = 0x0008;    // 오른쪽 마우스 버튼 눌림
        const uint RBUTTONUP = 0x00010;      // 오른쪽 마우스 버튼 떼어짐

        private readonly string configPath = Directory.GetCurrentDirectory() + "\\config.ini";

        private string currentPath;

        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        public Form1()
        {
            InitializeComponent();

            LoadPath();

            timer.Interval = 20;
            timer.Tick += TimerTick;
            timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            Point p = Cursor.Position;
            label1.Text = p.ToString();
        }

        private void SetPath(string path)
        {
            currentPath = path;
            textBox1.Text = path;
            folderBrowserDialog1.SelectedPath = path;
            File.WriteAllText(configPath, path);
        }

        private void LoadPath()
        {
            if (File.Exists(configPath))
            {
                string currentPath = File.ReadAllText(configPath, Encoding.UTF8);
                SetPath(currentPath);
                return;
            }

            string currentDirectory = Directory.GetCurrentDirectory();
            SetPath(currentDirectory);
        }

        private void ScreenCapture(string outputFilename)
        {
            // 주화면의 크기 정보 읽기
            Rectangle rect = Screen.PrimaryScreen.Bounds;
            // 2nd screen = Screen.AllScreens[1]

            // 픽셀 포맷 정보 얻기 (Optional)
            int bitsPerPixel = Screen.PrimaryScreen.BitsPerPixel;
            PixelFormat pixelFormat = PixelFormat.Format32bppArgb;
            if (bitsPerPixel <= 16)
            {
                pixelFormat = PixelFormat.Format16bppRgb565;
            }
            if (bitsPerPixel == 24)
            {
                pixelFormat = PixelFormat.Format24bppRgb;
            }

            // 화면 크기만큼의 Bitmap 생성
            Bitmap bmp = new Bitmap(rect.Width, rect.Height, pixelFormat);

            // Bitmap 이미지 변경을 위해 Graphics 객체 생성
            using (Graphics gr = Graphics.FromImage(bmp))
            {
                // 화면을 그대로 카피해서 Bitmap 메모리에 저장
                gr.CopyFromScreen(rect.Left, rect.Top, 0, 0, rect.Size);
            }

            // Bitmap 데이타를 파일로 저장
            bmp.Save(outputFilename);
            bmp.Dispose();
        }

        private void LButtonClick()
        {
            Thread.Sleep(200);
            mouse_event(LBUTTONDOWN, 0, 0, 0, 0);
            Thread.Sleep(200);
            mouse_event(LBUTTONUP, 0, 0, 0, 0);
            Thread.Sleep(200);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                SetPath(folderBrowserDialog1.SelectedPath);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            int count = (int)numericUpDown1.Value;
            MessageBox.Show(count.ToString());
            for (int i = 0; i < count; i++)
            {
                string num = string.Format("{0:D5}", i + 1);
                ScreenCapture(currentPath + $"\\test{num}.bmp");

                Cursor.Position = new Point(1870, 500);
                LButtonClick();
                
            }
            //ScreenCapture(currentPath + "\\test.bmp");
        }
    }
}
