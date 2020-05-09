using BattleCity.Domain;
using BattleCity.Vision;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleCity
{
    public class MainForm : Form
    {
        private readonly StartScreen startScreen;
        private readonly GameScreen gameScreen;
        private readonly EndScreen endScreen;
        public MainForm()
        {
            WindowState = FormWindowState.Maximized;
            BackColor = Color.DarkGray;
            Game.StageChanged += Game_OnStateChanged;
            startScreen = new StartScreen();
            gameScreen = new GameScreen();
            endScreen = new EndScreen();
            InitializeScreens();
        }

        private void Game_OnStateChanged()
        {
            if (Game.Stage == GameStage.Start)
                ShowStartScreen();
            else if (Game.Stage == GameStage.Playing)
            {
                ShowGameScreen();
                gameScreen.Focus();
            }
            else if (Game.Stage == GameStage.Lost || Game.Stage == GameStage.Won)
                ShowEndScreen();
        }

        private void InitializeScreens()
        {
            Controls.Add(startScreen);
            Controls.Add(gameScreen);
            Controls.Add(endScreen);
            ShowStartScreen();
        }

        private void ShowStartScreen()
        {
            HideScreens();
            startScreen.Show();
        }

        private void ShowGameScreen()
        {
            HideScreens();
            gameScreen.Show();
        }

        private void ShowEndScreen()
        {
            HideScreens();
            endScreen.Configure();
            endScreen.Show();
        }

        private void HideScreens()
        {
            startScreen.Hide();
            gameScreen.Hide();
            endScreen.Hide();
        }
    }
}
