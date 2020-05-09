using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BattleCity.Domain;

namespace BattleCity.Vision
{
    public partial class StartScreen : UserControl
    {
        TableLayoutPanel table;
        public StartScreen()
        {
            Dock = DockStyle.Fill;
            table = new TableLayoutPanel();
            BuildStartScreen();
            Controls.Add(table);
        }

        public void BuildStartScreen()
        {
            table.Dock = DockStyle.Fill;
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            FillEmptyCages();
            var startButton = new Button();
            startButton.Dock = DockStyle.Fill;
            startButton.Text = "Start Game";
            startButton.Font = new Font("Arial", 30, FontStyle.Regular);
            startButton.BackColor = Color.LightGray;
            startButton.Click += (s, e) => Game.ChangeState(GameStage.Playing);
            table.Controls.Add(startButton);
        }

        public void FillEmptyCages()
        {
            table.Controls.Add(new Panel(), 0, 0);
            table.Controls.Add(new Panel(), 0, 1);
            table.Controls.Add(new Panel(), 0, 2);

            table.Controls.Add(new Panel(), 1, 0);
            table.Controls.Add(new Panel(), 1, 2);

            table.Controls.Add(new Panel(), 2, 0);
            table.Controls.Add(new Panel(), 2, 1);
            table.Controls.Add(new Panel(), 2, 2);
        }
    }
}
