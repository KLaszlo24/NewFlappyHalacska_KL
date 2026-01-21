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
using System.Windows.Threading;

namespace NewFlappyHalacska
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        DispatcherTimer timer=new DispatcherTimer();
        
        double score;
        int gravitacio;
        bool jatekvege;
        Rect FlappyhalHitbox;
        public MainWindow()
        {
            InitializeComponent();
            timer.Tick += MainEventTimer;
            timer.Interval = TimeSpan.FromMilliseconds(20);
            StartGame();
        }

		private void MainEventTimer(object sender, EventArgs e)
		{
			txtScore.Content = "Score: " + score;

			FlappyhalHitbox = new Rect(Canvas.GetLeft(FlappyHal), Canvas.GetTop(FlappyHal), FlappyHal.Width - 12, FlappyHal.Height);

			Canvas.SetTop(FlappyHal, Canvas.GetTop(FlappyHal) + gravitacio);

			if (Canvas.GetTop(FlappyHal) < -30 || Canvas.GetTop(FlappyHal) + FlappyHal.Height > 460)
			{
				EndGame();
			}


			foreach (var x in MyCanvas.Children.OfType<Image>())
			{
				if ((string)x.Tag == "obs1" || (string)x.Tag == "obs2" || (string)x.Tag == "obs3")
				{
					Canvas.SetLeft(x, Canvas.GetLeft(x) - 5);

					if (Canvas.GetLeft(x) < -100)
					{
						Canvas.SetLeft(x, 800);

						score += .5;
					}

					Rect PillarHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

					if (FlappyhalHitbox.IntersectsWith(PillarHitBox))
					{
						EndGame();
					}
				}

				if ((string)x.Tag == "clouds")
				{
					Canvas.SetLeft(x, Canvas.GetLeft(x) - 1);

					if (Canvas.GetLeft(x) < -250)
					{
						Canvas.SetLeft(x, 550);

						score += .5;
					}

				}


			}


		}

		private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                FlappyHal.RenderTransform = new RotateTransform(-20, FlappyHal.Width/2, FlappyHal.Height/2);
                gravitacio = -8;
            }

            if (e.Key == Key.R && jatekvege==true)
            {
                StartGame();
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            FlappyHal.RenderTransform = new RotateTransform(5, FlappyHal.Width / 2, FlappyHal.Height / 2);
            gravitacio = 8;
        }

        private void StartGame()
        {
            MyCanvas.Focus();

            int temp = 300;
            score = 0;

            jatekvege= false;
            Canvas.SetTop(FlappyHal, 190);

            foreach(var x in MyCanvas.Children.OfType<Image>())
            {
                if((string)x.Tag == "obs1")
                {
                    Canvas.SetLeft(x, 500);
                }
                if ((string)x.Tag == "obs2")
                {
                    Canvas.SetLeft(x, 800);
                }
                if ((string)x.Tag == "obs3")
                {
                    Canvas.SetLeft(x, 1100);
                }
                if ((string)x.Tag == "cloud")
                {
                    Canvas.SetLeft(x, 300 + temp);
                    temp = 800;
                }
            }

            timer.Start();
        }

        private void EndGame()
        {
            timer.Stop();
            jatekvege = true;
            txtScore.Content += " Jatek Vege! Nyomj R-t az újrajátszáshoz";
        }
    }
}
