namespace AtmbTestDevices
{
    partial class FormMain
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <bufferParam name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</bufferParam>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            this.serialPort = new System.IO.Ports.SerialPort(this.components);
            this.tbToPay = new System.Windows.Forms.TextBox();
            this.labelAmountToPay = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.stripLabelCom = new System.Windows.Forms.ToolStripStatusLabel();
            this.stripLabelCashReaderStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.labelAmountReceived = new System.Windows.Forms.Label();
            this.tbReceived = new System.Windows.Forms.TextBox();
            this.labelAmountRemaining = new System.Windows.Forms.Label();
            this.tbRemaining = new System.Windows.Forms.TextBox();
            this.tbDenomination = new System.Windows.Forms.TextBox();
            this.labelDenomination = new System.Windows.Forms.Label();
            this.tbInfo = new System.Windows.Forms.TextBox();
            this.contextMenuStripClear = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.dataGridViewHopper = new System.Windows.Forms.DataGridView();
            this.Identifiant = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Valeur = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Present = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ToDispense = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Dispensed = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LevelSW = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LevelHW = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ToEmpty = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Reload = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ToLoad = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.buttonHoppers = new System.Windows.Forms.Button();
            this.dataGridViewCompteurs = new System.Windows.Forms.DataGridView();
            this.Nom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Montant = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonCounters = new System.Windows.Forms.Button();
            this.buttonRAZCompteurs = new System.Windows.Forms.Button();
            this.labelInfo = new System.Windows.Forms.Label();
            this.statusStrip.SuspendLayout();
            this.contextMenuStripClear.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewHopper)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCompteurs)).BeginInit();
            this.SuspendLayout();
            // 
            // tbToPay
            // 
            this.tbToPay.AllowDrop = true;
            resources.ApplyResources(this.tbToPay, "tbToPay");
            this.tbToPay.Name = "tbToPay";
            this.tbToPay.Enter += new System.EventHandler(this.TBAmountToPay_Enter);
            this.tbToPay.Leave += new System.EventHandler(this.TBAmountToPay_Leave);
            // 
            // labelAmountToPay
            // 
            resources.ApplyResources(this.labelAmountToPay, "labelAmountToPay");
            this.labelAmountToPay.Name = "labelAmountToPay";
            // 
            // btnExit
            // 
            resources.ApplyResources(this.btnExit, "btnExit");
            this.btnExit.Name = "btnExit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.BtnExit_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stripLabelCom,
            this.stripLabelCashReaderStatus});
            resources.ApplyResources(this.statusStrip, "statusStrip");
            this.statusStrip.Name = "statusStrip";
            // 
            // stripLabelCom
            // 
            this.stripLabelCom.AutoToolTip = true;
            this.stripLabelCom.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.stripLabelCom.BorderStyle = System.Windows.Forms.Border3DStyle.Bump;
            this.stripLabelCom.LinkColor = System.Drawing.SystemColors.Control;
            this.stripLabelCom.Name = "stripLabelCom";
            resources.ApplyResources(this.stripLabelCom, "stripLabelCom");
            // 
            // stripLabelCashReaderStatus
            // 
            this.stripLabelCashReaderStatus.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.stripLabelCashReaderStatus.BorderStyle = System.Windows.Forms.Border3DStyle.Bump;
            resources.ApplyResources(this.stripLabelCashReaderStatus, "stripLabelCashReaderStatus");
            this.stripLabelCashReaderStatus.Name = "stripLabelCashReaderStatus";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            resources.ApplyResources(this.toolStripStatusLabel1, "toolStripStatusLabel1");
            // 
            // labelAmountReceived
            // 
            resources.ApplyResources(this.labelAmountReceived, "labelAmountReceived");
            this.labelAmountReceived.Name = "labelAmountReceived";
            // 
            // tbReceived
            // 
            this.tbReceived.AllowDrop = true;
            resources.ApplyResources(this.tbReceived, "tbReceived");
            this.tbReceived.Name = "tbReceived";
            // 
            // labelAmountRemaining
            // 
            resources.ApplyResources(this.labelAmountRemaining, "labelAmountRemaining");
            this.labelAmountRemaining.Name = "labelAmountRemaining";
            // 
            // tbRemaining
            // 
            this.tbRemaining.AllowDrop = true;
            resources.ApplyResources(this.tbRemaining, "tbRemaining");
            this.tbRemaining.Name = "tbRemaining";
            // 
            // tbDenomination
            // 
            this.tbDenomination.AllowDrop = true;
            resources.ApplyResources(this.tbDenomination, "tbDenomination");
            this.tbDenomination.Name = "tbDenomination";
            // 
            // labelDenomination
            // 
            resources.ApplyResources(this.labelDenomination, "labelDenomination");
            this.labelDenomination.Name = "labelDenomination";
            // 
            // tbInfo
            // 
            this.tbInfo.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tbInfo.ContextMenuStrip = this.contextMenuStripClear;
            resources.ApplyResources(this.tbInfo, "tbInfo");
            this.tbInfo.Name = "tbInfo";
            this.tbInfo.ReadOnly = true;
            this.tbInfo.ShortcutsEnabled = false;
            // 
            // contextMenuStripClear
            // 
            this.contextMenuStripClear.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.contextMenuStripClear.Name = "contextMenuStripClear";
            resources.ApplyResources(this.contextMenuStripClear, "contextMenuStripClear");
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            this.toolStripMenuItem1.Click += new System.EventHandler(this.ToolStripMenuItem1_Click);
            // 
            // dataGridViewHopper
            // 
            this.dataGridViewHopper.AllowUserToAddRows = false;
            this.dataGridViewHopper.AllowUserToDeleteRows = false;
            this.dataGridViewHopper.AllowUserToOrderColumns = true;
            this.dataGridViewHopper.AllowUserToResizeColumns = false;
            this.dataGridViewHopper.AllowUserToResizeRows = false;
            this.dataGridViewHopper.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridViewHopper.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
            this.dataGridViewHopper.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewHopper.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewHopper.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewHopper.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Identifiant,
            this.Valeur,
            this.Present,
            this.ToDispense,
            this.Dispensed,
            this.LevelSW,
            this.LevelHW,
            this.ToEmpty,
            this.Reload,
            this.ToLoad});
            resources.ApplyResources(this.dataGridViewHopper, "dataGridViewHopper");
            this.dataGridViewHopper.Name = "dataGridViewHopper";
            this.dataGridViewHopper.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            this.dataGridViewHopper.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridViewHopper.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridViewHopper_CellClick);
            this.dataGridViewHopper.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridViewHopper_CellValueChanged);
            // 
            // Identifiant
            // 
            this.Identifiant.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.Identifiant.DefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(this.Identifiant, "Identifiant");
            this.Identifiant.Name = "Identifiant";
            this.Identifiant.ReadOnly = true;
            // 
            // Valeur
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle3.Format = "c2";
            this.Valeur.DefaultCellStyle = dataGridViewCellStyle3;
            this.Valeur.FillWeight = 50F;
            resources.ApplyResources(this.Valeur, "Valeur");
            this.Valeur.Name = "Valeur";
            this.Valeur.ReadOnly = true;
            this.Valeur.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // Present
            // 
            this.Present.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.Present, "Present");
            this.Present.Name = "Present";
            this.Present.ReadOnly = true;
            this.Present.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Present.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // ToDispense
            // 
            this.ToDispense.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.ToDispense.DefaultCellStyle = dataGridViewCellStyle4;
            this.ToDispense.FillWeight = 30F;
            resources.ApplyResources(this.ToDispense, "ToDispense");
            this.ToDispense.MaxInputLength = 3;
            this.ToDispense.Name = "ToDispense";
            // 
            // Dispensed
            // 
            this.Dispensed.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.Dispensed.DefaultCellStyle = dataGridViewCellStyle5;
            this.Dispensed.FillWeight = 30F;
            resources.ApplyResources(this.Dispensed, "Dispensed");
            this.Dispensed.MaxInputLength = 3;
            this.Dispensed.Name = "Dispensed";
            this.Dispensed.ReadOnly = true;
            // 
            // LevelSW
            // 
            this.LevelSW.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.LevelSW.DefaultCellStyle = dataGridViewCellStyle6;
            resources.ApplyResources(this.LevelSW, "LevelSW");
            this.LevelSW.Name = "LevelSW";
            this.LevelSW.ReadOnly = true;
            // 
            // LevelHW
            // 
            this.LevelHW.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.LevelHW.DefaultCellStyle = dataGridViewCellStyle7;
            resources.ApplyResources(this.LevelHW, "LevelHW");
            this.LevelHW.Name = "LevelHW";
            this.LevelHW.ReadOnly = true;
            // 
            // ToEmpty
            // 
            this.ToEmpty.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ToEmpty.FillWeight = 50F;
            resources.ApplyResources(this.ToEmpty, "ToEmpty");
            this.ToEmpty.Name = "ToEmpty";
            this.ToEmpty.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // Reload
            // 
            this.Reload.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.Reload.DefaultCellStyle = dataGridViewCellStyle8;
            resources.ApplyResources(this.Reload, "Reload");
            this.Reload.Name = "Reload";
            // 
            // ToLoad
            // 
            this.ToLoad.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ToLoad.FillWeight = 50F;
            resources.ApplyResources(this.ToLoad, "ToLoad");
            this.ToLoad.Name = "ToLoad";
            this.ToLoad.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // buttonHoppers
            // 
            resources.ApplyResources(this.buttonHoppers, "buttonHoppers");
            this.buttonHoppers.Name = "buttonHoppers";
            this.buttonHoppers.UseVisualStyleBackColor = true;
            this.buttonHoppers.Click += new System.EventHandler(this.ButtonHoppers_Click);
            // 
            // dataGridViewCompteurs
            // 
            this.dataGridViewCompteurs.AllowUserToAddRows = false;
            this.dataGridViewCompteurs.AllowUserToDeleteRows = false;
            dataGridViewCellStyle9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dataGridViewCompteurs.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle9;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewCompteurs.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle10;
            this.dataGridViewCompteurs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewCompteurs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Nom,
            this.Montant});
            resources.ApplyResources(this.dataGridViewCompteurs, "dataGridViewCompteurs");
            this.dataGridViewCompteurs.Name = "dataGridViewCompteurs";
            this.dataGridViewCompteurs.ReadOnly = true;
            // 
            // Nom
            // 
            this.Nom.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.Nom, "Nom");
            this.Nom.Name = "Nom";
            this.Nom.ReadOnly = true;
            // 
            // Montant
            // 
            this.Montant.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle11.NullValue = "0";
            this.Montant.DefaultCellStyle = dataGridViewCellStyle11;
            resources.ApplyResources(this.Montant, "Montant");
            this.Montant.Name = "Montant";
            this.Montant.ReadOnly = true;
            // 
            // buttonCounters
            // 
            resources.ApplyResources(this.buttonCounters, "buttonCounters");
            this.buttonCounters.Name = "buttonCounters";
            this.buttonCounters.UseVisualStyleBackColor = true;
            this.buttonCounters.Click += new System.EventHandler(this.ButtonCounters_Click);
            // 
            // buttonRAZCompteurs
            // 
            resources.ApplyResources(this.buttonRAZCompteurs, "buttonRAZCompteurs");
            this.buttonRAZCompteurs.Name = "buttonRAZCompteurs";
            this.buttonRAZCompteurs.UseVisualStyleBackColor = true;
            this.buttonRAZCompteurs.Click += new System.EventHandler(this.ButtonRAZCompteurs_Click);
            // 
            // labelInfo
            // 
            resources.ApplyResources(this.labelInfo, "labelInfo");
            this.labelInfo.Name = "labelInfo";
            // 
            // FormMain
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.BackColor = System.Drawing.Color.LightSkyBlue;
            this.Controls.Add(this.labelInfo);
            this.Controls.Add(this.buttonRAZCompteurs);
            this.Controls.Add(this.buttonCounters);
            this.Controls.Add(this.dataGridViewCompteurs);
            this.Controls.Add(this.buttonHoppers);
            this.Controls.Add(this.dataGridViewHopper);
            this.Controls.Add(this.tbInfo);
            this.Controls.Add(this.labelDenomination);
            this.Controls.Add(this.tbDenomination);
            this.Controls.Add(this.labelAmountRemaining);
            this.Controls.Add(this.tbRemaining);
            this.Controls.Add(this.labelAmountReceived);
            this.Controls.Add(this.tbReceived);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.labelAmountToPay);
            this.Controls.Add(this.tbToPay);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "FormMain";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.FormMain_Shown);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.contextMenuStripClear.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewHopper)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCompteurs)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.IO.Ports.SerialPort serialPort;
        private System.Windows.Forms.TextBox tbToPay;
        private System.Windows.Forms.Label labelAmountToPay;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Label labelAmountReceived;
        private System.Windows.Forms.TextBox tbReceived;
        private System.Windows.Forms.Label labelAmountRemaining;
        private System.Windows.Forms.TextBox tbRemaining;
        private System.Windows.Forms.TextBox tbDenomination;
        private System.Windows.Forms.Label labelDenomination;
        private System.Windows.Forms.ToolStripStatusLabel stripLabelCom;
        private System.Windows.Forms.TextBox tbInfo;
        private System.Windows.Forms.ToolStripStatusLabel stripLabelCashReaderStatus;
        private System.Windows.Forms.DataGridView dataGridViewHopper;
        private System.Windows.Forms.Button buttonHoppers;
        private System.Windows.Forms.DataGridView dataGridViewCompteurs;
        private System.Windows.Forms.Button buttonCounters;
        private System.Windows.Forms.Button buttonRAZCompteurs;
        private System.Windows.Forms.DataGridViewTextBoxColumn Identifiant;
        private System.Windows.Forms.DataGridViewTextBoxColumn Valeur;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Present;
        private System.Windows.Forms.DataGridViewTextBoxColumn ToDispense;
        private System.Windows.Forms.DataGridViewTextBoxColumn Dispensed;
        private System.Windows.Forms.DataGridViewTextBoxColumn LevelSW;
        private System.Windows.Forms.DataGridViewTextBoxColumn LevelHW;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ToEmpty;
        private System.Windows.Forms.DataGridViewTextBoxColumn Reload;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ToLoad;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripClear;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Nom;
        private System.Windows.Forms.DataGridViewTextBoxColumn Montant;
    }
}

