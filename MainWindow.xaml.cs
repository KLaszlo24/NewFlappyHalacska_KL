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
	public enum Difficulty
	{
		Easy,
		Normal,
		Hard
	}

	public partial class MainWindow : Window
	{
		DispatcherTimer timer = new DispatcherTimer();
		DispatcherTimer gameOverTimer = new DispatcherTimer();

		double score;
		int gravitacio;
		int gravityStrength;
		bool jatekvege;

		Rect FlappyhalHitbox;
		Difficulty currentDifficulty = Difficulty.Easy;

		private ImageBrush easyBackground;
		private ImageBrush normalBackground;
		private ImageBrush hardBackground;

		private Random rand = new Random();
		private int raindropCount = 150;

		MediaPlayer easyMusic = new MediaPlayer();
		MediaPlayer normalMusic = new MediaPlayer();
		MediaPlayer hardMusic = new MediaPlayer();

		public MainWindow()
		{
			InitializeComponent();

			easyBackground = new ImageBrush(new BitmapImage(
				new Uri("pack://application:,,,/images/Houses.jpg")));
			normalBackground = new ImageBrush(new BitmapImage(
				new Uri("pack://application:,,,/images/dark.jpg")));
			hardBackground = new ImageBrush(new BitmapImage(
				new Uri("pack://application:,,,/images/max.jpg")));

			easyMusic.Open(new Uri("sounds/easy.mp3", UriKind.Relative));
			normalMusic.Open(new Uri("sounds/normal1.mp3", UriKind.Relative));
			hardMusic.Open(new Uri("sounds/hard.mp3", UriKind.Relative));

			easyMusic.Volume = 10.5;
			normalMusic.Volume = 10.5;
			hardMusic.Volume = 10.5;

			timer.Interval = TimeSpan.FromMilliseconds(20);
			timer.Tick += MainEventTimer;
		}

		private void StartButton_Click(object sender, RoutedEventArgs e)
		{
			SetDifficulty((Difficulty)DifficultyBox.SelectedIndex);

			MainMenu.Visibility = Visibility.Collapsed;
			GameOverScreen.Visibility = Visibility.Collapsed;
			MyCanvas.Visibility = Visibility.Visible;

			StartGame();
		}

		private void StopAllMusic()
		{
			easyMusic.Stop();
			normalMusic.Stop();
			hardMusic.Stop();
		}

		private void SetDifficulty(Difficulty diff)
		{
			currentDifficulty = diff;
			StopAllMusic();

			switch (diff)
			{
				case Difficulty.Easy:
					gravityStrength = 6;
					MyCanvas.Background = easyBackground;
					easyMusic.Position = TimeSpan.Zero;
					easyMusic.Play();
					break;

				case Difficulty.Normal:
					gravityStrength = 10;
					MyCanvas.Background = normalBackground;
					normalMusic.Position = TimeSpan.Zero;
					normalMusic.Play();
					break;

				case Difficulty.Hard:
					gravityStrength = 15;
					MyCanvas.Background = hardBackground;
					hardMusic.Position = TimeSpan.Zero;
					FogImage.Visibility = Visibility.Visible;
					hardMusic.Play();
					break;
			}
		}

		private void StartGame()
		{
			MyCanvas.Focus();

			score = 0;
			jatekvege = false;
			gravitacio = gravityStrength;

			Canvas.SetTop(FlappyHal, 190);

			int temp = 300;

			foreach (var x in MyCanvas.Children.OfType<Image>())
			{
				if ((string)x.Tag == "obs1") Canvas.SetLeft(x, 500);
				if ((string)x.Tag == "obs2") Canvas.SetLeft(x, 800);
				if ((string)x.Tag == "obs3") Canvas.SetLeft(x, 1100);

				if ((string)x.Tag == "cloud")
				{
					Canvas.SetLeft(x, 300 + temp);
					temp = 800;
				}
			}

			if (currentDifficulty == Difficulty.Normal)
				StartRain();
			else
				RainCanvas.Children.Clear();

			timer.Start();
		}

		private void MainEventTimer(object sender, EventArgs e)
		{
			txtScore.Content = "Score: " + score;

			FlappyhalHitbox = new Rect(
				Canvas.GetLeft(FlappyHal),
				Canvas.GetTop(FlappyHal),
				FlappyHal.Width - 20,
				FlappyHal.Height - 20);

			Canvas.SetTop(FlappyHal, Canvas.GetTop(FlappyHal) + gravitacio);

			if (Canvas.GetTop(FlappyHal) < -30 ||
				Canvas.GetTop(FlappyHal) + FlappyHal.Height > 460)
			{
				EndGame();
			}

			foreach (var x in MyCanvas.Children.OfType<Image>())
			{
				if ((string)x.Tag == "obs1" ||
					(string)x.Tag == "obs2" ||
					(string)x.Tag == "obs3")
				{
					Canvas.SetLeft(x, Canvas.GetLeft(x) - 5); //tickenként balra kell vigyem

					if (Canvas.GetLeft(x) < -100)
					{
						Canvas.SetLeft(x, 800);
						score += 0.5;
					}

					Rect pillarHitbox = new Rect(
						//Hitboxok a bigyóknak
						Canvas.GetLeft(x),
						Canvas.GetTop(x),
						x.Width,
						x.Height);

					if (FlappyhalHitbox.IntersectsWith(pillarHitbox))
						EndGame();
				}

				if ((string)x.Tag == "cloud")
				{
					Canvas.SetLeft(x, Canvas.GetLeft(x) - 1);

					if (Canvas.GetLeft(x) < -250)
						Canvas.SetLeft(x, 550);
				}
			}

			if (currentDifficulty == Difficulty.Normal)
				UpdateRain();
		}

		private void EndGame()
		{
			if (jatekvege) return;

			jatekvege = true;
			timer.Stop();
			StopAllMusic();

			GameOverScreen.Visibility = Visibility.Visible;

			gameOverTimer.Interval = TimeSpan.FromSeconds(3);//3mp és főmenübe lépek
			gameOverTimer.Tick += (s, e) =>
			{
				gameOverTimer.Stop();
				//iit mutatja majd meg nekem
				ShowMainMenu();
			};
			gameOverTimer.Start();
		}

		private void ShowMainMenu()
		{
			MyCanvas.Visibility = Visibility.Collapsed;
			GameOverScreen.Visibility = Visibility.Collapsed;
			MainMenu.Visibility = Visibility.Visible;
		}

		private void KeyIsDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Space && !jatekvege)
			{
				FlappyHal.RenderTransform =
					new RotateTransform(-20,
						FlappyHal.Width / 2,
						FlappyHal.Height / 2);

				gravitacio = -gravityStrength;
			}
		}

		private void KeyIsUp(object sender, KeyEventArgs e)
		{
			gravitacio = gravityStrength;
		}


		//Ez mar egy bubble :(
		private void StartRain()
		{
			RainCanvas.Children.Clear();

			for (int i = 0; i < raindropCount; i++)
			{
				Image bubble = new Image()
				{
					Source = new BitmapImage(
						new Uri("pack://application:,,,/images/raindrops.png")),
					Width = 12,
					Height = 12,
					Opacity = 0.7
				};

				Canvas.SetLeft(bubble, rand.Next(0, (int)MyCanvas.ActualWidth));
				Canvas.SetTop(bubble, rand.Next(
					(int)MyCanvas.ActualHeight,
					(int)MyCanvas.ActualHeight + 300));

				RainCanvas.Children.Add(bubble);
			}
		}

		private void UpdateRain()
		{
			foreach (Image bubble in RainCanvas.Children)
			{
				double top = Canvas.GetTop(bubble) - rand.Next(2, 5);

				double left = Canvas.GetLeft(bubble) + rand.Next(-1, 2);

				if (top < -20)
				{
					top = MyCanvas.ActualHeight + rand.Next(50, 200);
					left = rand.Next(0, (int)MyCanvas.ActualWidth);
				}
				Canvas.SetTop(bubble, top);
				Canvas.SetLeft(bubble, left);
			}
		}
	}
}