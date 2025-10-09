using System.Drawing;
using System.Windows.Forms;

namespace UPG_SP_2024
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            battleField1 = new BattleField();
            SuspendLayout();
            // 
            // battleField1
            // 
            battleField1.Dock = DockStyle.Fill;
            battleField1.Location = new Point(0, 0);
            battleField1.Margin = new Padding(2, 2, 2, 2);
            battleField1.Name = "battleField1";
            battleField1.Size = new Size(782, 553);
            battleField1.TabIndex = 0;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(782, 553);
            Controls.Add(battleField1);
            Margin = new Padding(4, 4, 4, 4);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "<A24B0234P> - Semestrální práce KIV/UPG 2025/2026";
            Load += MainForm_Load;
            ResumeLayout(false);
        }

        #endregion

        private Panel drawingPanel;
        private BattleField battleField1;
    }
}
