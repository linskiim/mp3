using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace WpfApp6
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MediaPlayer mp;
        FolderBrowserDialog folderBrowserDialog;
        List<TagLib.File> list;
        string[] files_list;
        DispatcherTimer timer;
        //List<string> authors;
        

        public MainWindow()
        {
            InitializeComponent();

            mp = new MediaPlayer();

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 1);
            timer.Tick += timer_Tick;
            volume_slider.Value = 20;
            
        }

        private void Button_folder_Click(object sender, RoutedEventArgs e)
        {
            if (list == null || list.ToArray().Length == 0)
            {
                folderBrowserDialog = new FolderBrowserDialog();
                folderBrowserDialog.ShowDialog();

                if (folderBrowserDialog.SelectedPath != "")
                {
                    list = new List<TagLib.File>();
                    files_list = Directory.GetFiles(folderBrowserDialog.SelectedPath);

                    foreach (var item in files_list)
                    {
                        list.Add(TagLib.File.Create(item));

                    }
                    Content_listBox.ItemsSource = list;
                    
                }

            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            SliderTimer.Value = SliderTimer.Value + 1;

            TimeSpan time = new TimeSpan(0, 0, 0, (int)SliderTimer.Value);
            if ( time.Seconds < 10)
                start_time.Text = "0" + time.Minutes.ToString() + ":" + "0" + time.Seconds.ToString();
            if (time.Seconds >= 10)
                start_time.Text = "0" + time.Minutes.ToString() + ":" +  time.Seconds.ToString();
        }

        private void buttonCancelText_Click(object sender, RoutedEventArgs e)
        {
            textbox_search.Text = "";
        }

        private void play_Button_Click(object sender, RoutedEventArgs e)
        {
            timer.Start();
            mp.Play();
            play_Button.Visibility = Visibility.Hidden;
            stop_Button.Visibility = Visibility.Visible;

            if (list[Content_listBox.SelectedIndex].Tag.Pictures.Length >= 1)
            {
                TagLib.IPicture pic = list[Content_listBox.SelectedIndex].Tag.Pictures[0];
                MemoryStream ms = new MemoryStream(pic.Data.Data);
                ms.Seek(0, SeekOrigin.Begin);

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = ms;
                bitmap.EndInit();

                img.Source = bitmap;

            }


            if (start_time == tot_time)
            {
                Next();
            }

        }


        private void stop_Button_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            mp.Pause();
            stop_Button.Visibility = Visibility.Collapsed;
            play_Button.Visibility = Visibility.Visible;
        }


        private void next_Button_Click(object sender, RoutedEventArgs e)
        {
            Next();
        }

        private void back_Button_Click(object sender, RoutedEventArgs e)
        {
            Back();
        }

        private void Next()
        {
            Content_listBox.SelectedIndex++;

            timer.Start();
            mp.Play();
            play_Button.Visibility = Visibility.Hidden;
            stop_Button.Visibility = Visibility.Visible;
        }
        private void Back()
        {
            Content_listBox.SelectedIndex--;

            timer.Start();
            mp.Play();
            play_Button.Visibility = Visibility.Hidden;
            stop_Button.Visibility = Visibility.Visible;
        }

        bool state;
        private void SliderTimer_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                state = true;
            if (e.LeftButton == MouseButtonState.Released && state == true)
            {
                mp.Position = new TimeSpan(0, 0, 0, (int)SliderTimer.Value);
                state = false;
            }
        }


        private void Content_listBox_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
                SliderTimer.Maximum = list.ToArray()[Content_listBox.SelectedIndex].Properties.Duration.TotalSeconds;
                tot_time.Text = list.ToArray()[Content_listBox.SelectedIndex].Properties.Duration.ToString("mm\\:ss");
                SliderTimer.Value = 0;

                mp.Open(new Uri(files_list[Content_listBox.SelectedIndex]));

                timer.Start();
                mp.Play();
                play_Button.Visibility = Visibility.Hidden;
                stop_Button.Visibility = Visibility.Visible;
            
        }

        private void Content_listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            img.Source = null;

            mp.Stop();
            timer.Stop();
            start_time.Text = "00:00";
            stop_Button.Visibility = Visibility.Hidden;
            play_Button.Visibility = Visibility.Visible;


            SliderTimer.Maximum = list.ToArray()[Content_listBox.SelectedIndex].Properties.Duration.TotalSeconds;
            tot_time.Text = list.ToArray()[Content_listBox.SelectedIndex].Properties.Duration.ToString("mm\\:ss");
            SliderTimer.Value = 0;

            mp.Open(new Uri(files_list[Content_listBox.SelectedIndex]));


            if (list[Content_listBox.SelectedIndex].Tag.Title.Length != 0)
            {
                
                song_Namelabel.Content = list[Content_listBox.SelectedIndex].Tag.Title;
            }
            else
            {
                song_Namelabel.Content = "Unknown Title";
            }

            if(list[Content_listBox.SelectedIndex].Tag.Performers.Length >= 1)
            {
                song_Authorlabel.Content = String.Join(", ", list[Content_listBox.SelectedIndex].Tag.Performers);
            }
            else
            {
                song_Authorlabel.Content = "Unknown Performer";
            }

            if (list[Content_listBox.SelectedIndex].Tag.Pictures.Length >= 1)
            {
                TagLib.IPicture pic = list[Content_listBox.SelectedIndex].Tag.Pictures[0];
                MemoryStream ms = new MemoryStream(pic.Data.Data);
                ms.Seek(0, SeekOrigin.Begin);

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = ms;
                bitmap.EndInit();

                img.Source = bitmap;
            }

            else
            {
                img.Source = BitmapFrame.Create(new Uri(@"C:\Users\misha\OneDrive\Desktop\WpfApp6\WpfApp6\images\error.png"));
            }
        }


        private void textbox_seach_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textbox_search.Text.Length > 0)
            { 
                textblock_search.Visibility = Visibility.Collapsed;
                buttonCancelText.Visibility = Visibility.Visible;
            }
            else 
            {
                textblock_search.Visibility = Visibility.Visible;
                buttonCancelText.Visibility = Visibility.Collapsed;
                textblock_search.Text = "Search";
            }
        }


        private void volume_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mp.Volume = Convert.ToDouble(volume_slider.Value) / 100 ;
        }


        private void myMusic_Checked(object sender, RoutedEventArgs e)
        {
            blue.Visibility = Visibility.Visible;
        }

        private void Recently_Played_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}











































































































































































































































