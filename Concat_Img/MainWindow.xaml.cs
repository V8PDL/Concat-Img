using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Concat_Img
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string Path = Path_box.Text + @"\";
            if (Path == @"\")
            {
                Path = Path.Insert(0, System.IO.Directory.GetCurrentDirectory());
            }
            string Save_to;
            if (Save_there.IsChecked.Value)
                Save_to = Path;
            else
                Save_to = Path_to_Save.Text + @"\";

            List<string> Names = new List<string>();
            char[] separators = { ',', ' ' };
            Names.AddRange(Names_box.Text.Split(separators, StringSplitOptions.RemoveEmptyEntries).ToList());
            for (int i = 0; i < Names.Count; i++)
                Names[i] = Names[i].Insert(0, Path);
            if (All_in_dir.IsChecked.Value)
            {
                if (Even_in_subdir.IsChecked.Value)
                {
                    List<string> s = Get_Images(Path, true);
                    if (s != null)
                        Names.AddRange(s);
                    else
                    {
                        return;
                    }
                }
                else
                {
                    List<string> s = Get_Images(Path, false);
                    if (s != null)
                        Names.AddRange(s);
                    else
                    {
                        return;
                    }
                }
            }
            if (Names.Count == 0)
            {
                MessageBox.Show("Error");
                return;
            }

            Thread thread = new Thread(() => Go(Names, Save_to));
            thread.Start();
        }
        private void Go(List<string> Names, string Save_to)
        {
            List<Image> images = new List<Image>();

            try
            {
                int Width = 0, Height = 0, tempMax;
                foreach (string name in Names)
                {
                    images.Add(Image.FromFile(name));
                    tempMax = images.Last().Width;
                    Height += images.Last().Height;
                    if (tempMax > Width)
                        Width = tempMax;
                }
                Image result = new Bitmap(Width, Height);
                Graphics graphics = Graphics.FromImage(result);

                int Height_now = 0;
                foreach(Image image in images)
                {
                    graphics.DrawImage(image, 0, Height_now);
                    Height_now += image.Height;
                }
                result.Save(Save_to + Height + ".png", System.Drawing.Imaging.ImageFormat.Png);

                result.Dispose();
            }

            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        private void All_in_dir_Checked(object sender, RoutedEventArgs e)
        {
            Even_in_subdir.IsEnabled = true;
        }

        private void All_in_dir_Unchecked(object sender, RoutedEventArgs e)
        {
            Even_in_subdir.IsEnabled = false;
        }
        private List<string> Get_Images(string path, bool recursively)
        {
            List<string> File_Paths;
            try
            {
                File_Paths = System.IO.Directory.GetFiles(path, @"*.jpg").ToList();
                File_Paths.AddRange(System.IO.Directory.GetFiles(path, @"*.png").ToList());
                if (recursively)
                    foreach (string dir in System.IO.Directory.GetDirectories(path))
                    {
                        File_Paths.AddRange(Get_Images(dir, true));
                    }
                return File_Paths;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + '\n' + e.Source);
                return null;
            }
        }

        private void Save_there_Unchecked(object sender, RoutedEventArgs e)
        {
            Path_to_Save.IsEnabled = true;
        }

        private void Save_there_Checked(object sender, RoutedEventArgs e)
        {
            Path_to_Save.IsEnabled = false;
        }
    }
}
