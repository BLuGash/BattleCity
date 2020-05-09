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
using System.Runtime.InteropServices.WindowsRuntime;

namespace BattleCity.Vision
{
    public class EndScreen : UserControl
    {
        private Label label;
        public EndScreen()
        {
            Dock = DockStyle.Fill;
            var table = new TableLayoutPanel();
            table.Dock = DockStyle.Fill;
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 500));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            FillEmptyCages(table);
            label = new Label();
            label.Dock = DockStyle.Fill;
            label.Font = new Font("Arial", 50, FontStyle.Regular);
            label.Margin = new Padding(100, 0, 0, 0);
            table.Controls.Add(label);
            Controls.Add(table);
        }

        public void Configure()
        {
            label.Text = "You " + (Game.Stage == GameStage.Won ? "won" : "lost");
        }

        public void FillEmptyCages(TableLayoutPanel table)
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
