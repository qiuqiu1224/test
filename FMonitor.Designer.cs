namespace WindowsFormsApp1
{
    partial class FMonitor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.uiPanel1 = new Sunny.UI.UIPanel();
            this.downButton = new Sunny.UI.UISymbolButton();
            this.rightButton = new Sunny.UI.UISymbolButton();
            this.leftButton = new Sunny.UI.UISymbolButton();
            this.upButton = new Sunny.UI.UISymbolButton();
            this.uiButton1 = new Sunny.UI.UIButton();
            this.uiPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // uiPanel1
            // 
            this.uiPanel1.Controls.Add(this.downButton);
            this.uiPanel1.Controls.Add(this.rightButton);
            this.uiPanel1.Controls.Add(this.leftButton);
            this.uiPanel1.Controls.Add(this.upButton);
            this.uiPanel1.Controls.Add(this.uiButton1);
            this.uiPanel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.uiPanel1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiPanel1.Location = new System.Drawing.Point(477, 0);
            this.uiPanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiPanel1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel1.Name = "uiPanel1";
            this.uiPanel1.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.uiPanel1.Radius = 1;
            this.uiPanel1.Size = new System.Drawing.Size(323, 450);
            this.uiPanel1.Style = Sunny.UI.UIStyle.Custom;
            this.uiPanel1.StyleCustomMode = true;
            this.uiPanel1.TabIndex = 0;
            this.uiPanel1.Text = null;
            this.uiPanel1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiPanel1.Visible = false;
            // 
            // downButton
            // 
            this.downButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.downButton.FillColor = System.Drawing.Color.Transparent;
            this.downButton.FillColor2 = System.Drawing.Color.Transparent;
            this.downButton.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.downButton.Image = global::WindowsFormsApp1.Properties.Resources.downArrow;
            this.downButton.Location = new System.Drawing.Point(144, 212);
            this.downButton.MinimumSize = new System.Drawing.Size(1, 1);
            this.downButton.Name = "downButton";
            this.downButton.Size = new System.Drawing.Size(52, 52);
            this.downButton.Style = Sunny.UI.UIStyle.Custom;
            this.downButton.StyleCustomMode = true;
            this.downButton.TabIndex = 4;
            this.downButton.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.downButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DownButton_MouseDown);
            this.downButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DownButton_MouseUp);
            // 
            // rightButton
            // 
            this.rightButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rightButton.FillColor = System.Drawing.Color.Transparent;
            this.rightButton.FillColor2 = System.Drawing.Color.Transparent;
            this.rightButton.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rightButton.Image = global::WindowsFormsApp1.Properties.Resources.rightArrow;
            this.rightButton.Location = new System.Drawing.Point(203, 154);
            this.rightButton.MinimumSize = new System.Drawing.Size(1, 1);
            this.rightButton.Name = "rightButton";
            this.rightButton.Size = new System.Drawing.Size(52, 52);
            this.rightButton.Style = Sunny.UI.UIStyle.Custom;
            this.rightButton.StyleCustomMode = true;
            this.rightButton.TabIndex = 3;
            this.rightButton.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rightButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RightButton_MouseDown);
            this.rightButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RightButton_MouseUp);
            // 
            // leftButton
            // 
            this.leftButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.leftButton.FillColor = System.Drawing.Color.Transparent;
            this.leftButton.FillColor2 = System.Drawing.Color.Transparent;
            this.leftButton.FillHoverColor = System.Drawing.Color.Transparent;
            this.leftButton.FillPressColor = System.Drawing.Color.Transparent;
            this.leftButton.FillSelectedColor = System.Drawing.Color.Transparent;
            this.leftButton.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.leftButton.Image = global::WindowsFormsApp1.Properties.Resources.leftArrow;
            this.leftButton.Location = new System.Drawing.Point(88, 154);
            this.leftButton.MinimumSize = new System.Drawing.Size(1, 1);
            this.leftButton.Name = "leftButton";
            this.leftButton.Size = new System.Drawing.Size(52, 52);
            this.leftButton.Style = Sunny.UI.UIStyle.Custom;
            this.leftButton.StyleCustomMode = true;
            this.leftButton.TabIndex = 2;
            this.leftButton.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.leftButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LeftButton_MouseDown);
            this.leftButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.LeftButton_MouseUp);
            // 
            // upButton
            // 
            this.upButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.upButton.FillColor = System.Drawing.Color.Transparent;
            this.upButton.FillColor2 = System.Drawing.Color.Transparent;
            this.upButton.FillHoverColor = System.Drawing.Color.Transparent;
            this.upButton.FillPressColor = System.Drawing.Color.Transparent;
            this.upButton.FillSelectedColor = System.Drawing.Color.Transparent;
            this.upButton.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.upButton.Image = global::WindowsFormsApp1.Properties.Resources.topArrow;
            this.upButton.Location = new System.Drawing.Point(144, 96);
            this.upButton.MinimumSize = new System.Drawing.Size(1, 1);
            this.upButton.Name = "upButton";
            this.upButton.Size = new System.Drawing.Size(52, 52);
            this.upButton.Style = Sunny.UI.UIStyle.Custom;
            this.upButton.StyleCustomMode = true;
            this.upButton.TabIndex = 1;
            this.upButton.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.upButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.UpButton_MouseDown);
            this.upButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.UpButton_MouseUp);
            // 
            // uiButton1
            // 
            this.uiButton1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uiButton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton1.Location = new System.Drawing.Point(0, 0);
            this.uiButton1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton1.Name = "uiButton1";
            this.uiButton1.Radius = 1;
            this.uiButton1.Size = new System.Drawing.Size(323, 35);
            this.uiButton1.Style = Sunny.UI.UIStyle.Custom;
            this.uiButton1.TabIndex = 0;
            this.uiButton1.Text = "云 台";
            this.uiButton1.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton1.ZoomScaleDisabled = true;
            // 
            // FMonitor
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.uiPanel1);
            this.Name = "FMonitor";
            this.Text = "FormMonitor";
            this.uiPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UIPanel uiPanel1;
        private Sunny.UI.UIButton uiButton1;
        private Sunny.UI.UISymbolButton upButton;
        private Sunny.UI.UISymbolButton leftButton;
        private Sunny.UI.UISymbolButton rightButton;
        private Sunny.UI.UISymbolButton downButton;
    }
}