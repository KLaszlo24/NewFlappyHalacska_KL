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
		Difficulty currentDifficulty = Difficulty.Normal;


		private ImageBrush normalBackground;
		private ImageBrush hardBackground;

		public MainWindow()
		{
			InitializeComponent();

			normalBackground = new ImageBrush(new BitmapImage(
				new Uri("pack://application:,,,/images/dark.jpg")));
			hardBackground = new ImageBrush(new BitmapImage(
				new Uri("pack://application:,,,/images/max.jpg")));

			MyCanvas.Background = normalBackground;

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

		private void SetDifficulty(Difficulty diff)
		{
			currentDifficulty = diff;

			switch (diff)
			{
				case Difficulty.Easy:
					gravityStrength = 6;
					MyCanvas.Background = normalBackground;
					break;
				case Difficulty.Normal:
					gravityStrength = 10;
					MyCanvas.Background = normalBackground;
					break;
				case Difficulty.Hard:
					gravityStrength = 25;
					MyCanvas.Background = hardBackground;
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

			timer.Start();
		}

		private void MainEventTimer(object sender, EventArgs e)
		{
			txtScore.Content = "Score: " + score;

			FlappyhalHitbox = new Rect(
				Canvas.GetLeft(FlappyHal),
				Canvas.GetTop(FlappyHal),
				FlappyHal.Width - 20,
				FlappyHal.Height - 10
			);

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
					Canvas.SetLeft(x, Canvas.GetLeft(x) - 5);

					if (Canvas.GetLeft(x) < -100)
					{
						Canvas.SetLeft(x, 800);
						score += 0.5;
					}

					Rect pillarHitbox = new Rect(
						Canvas.GetLeft(x),
						Canvas.GetTop(x),
						x.Width,
						x.Height
					);

					if (FlappyhalHitbox.IntersectsWith(pillarHitbox))
					{
						EndGame();
					}
				}

				if ((string)x.Tag == "cloud")
				{
					Canvas.SetLeft(x, Canvas.GetLeft(x) - 1);

					if (Canvas.GetLeft(x) < -250)
					{
						Canvas.SetLeft(x, 550);
					}
				}
			}
		}

		private void EndGame()
		{
			if (jatekvege) return;

			jatekvege = true;
			timer.Stop();

			GameOverScreen.Visibility = Visibility.Visible;

			gameOverTimer.Interval = TimeSpan.FromSeconds(3);
			gameOverTimer.Tick += (s, e) =>
			{
				gameOverTimer.Stop();
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
	}
}