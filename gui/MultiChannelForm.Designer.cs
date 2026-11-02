#nullable disable
namespace SdiConverter;
partial class MultiChannelForm
{
    private System.ComponentModel.IContainer components = null;
    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose();
        base.Dispose(disposing);
        }
    private void InitializeComponent()
    {
        refreshButton = new Button();
        startAllButton = new Button();
        stopAllButton = new Button();
        saveButton = new Button();
        detectionLabel = new Label();
        overview = new DataGridView();
        portColumn = new DataGridViewTextBoxColumn();
        directionColumn = new DataGridViewTextBoxColumn();
        statusColumn = new DataGridViewTextBoxColumn();
        cpuColumn = new DataGridViewTextBoxColumn();
        ramColumn = new DataGridViewTextBoxColumn();
        tabs = new TabControl();
        loadLabel = new Label();
        warningLabel = new Label();
        footer = new LinkLabel();
        label1 = new Label();
        linkLabel1 = new LinkLabel();
        ((System.ComponentModel.ISupportInitialize)overview).BeginInit();
        SuspendLayout();
        // 
        // refreshButton
        // 
        refreshButton.BackColor = Color.DimGray;
        refreshButton.FlatAppearance.BorderSize = 0;
        refreshButton.FlatStyle = FlatStyle.Flat;
        refreshButton.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
        refreshButton.ForeColor = Color.White;
        refreshButton.Location = new Point(20, 68);
        refreshButton.Name = "refreshButton";
        refreshButton.Size = new Size(140, 34);
        refreshButton.TabIndex = 0;
        refreshButton.Text = "Refresh ports";
        refreshButton.UseVisualStyleBackColor = false;
        // 
        // startAllButton
        // 
        startAllButton.BackColor = Color.DimGray;
        startAllButton.FlatAppearance.BorderSize = 0;
        startAllButton.FlatStyle = FlatStyle.Flat;
        startAllButton.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
        startAllButton.ForeColor = Color.White;
        startAllButton.Location = new Point(166, 68);
        startAllButton.Name = "startAllButton";
        startAllButton.Size = new Size(140, 34);
        startAllButton.TabIndex = 1;
        startAllButton.Text = "Start all";
        startAllButton.UseVisualStyleBackColor = false;
        // 
        // stopAllButton
        // 
        stopAllButton.BackColor = Color.DimGray;
        stopAllButton.FlatAppearance.BorderSize = 0;
        stopAllButton.FlatStyle = FlatStyle.Flat;
        stopAllButton.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
        stopAllButton.ForeColor = Color.White;
        stopAllButton.Location = new Point(312, 68);
        stopAllButton.Name = "stopAllButton";
        stopAllButton.Size = new Size(140, 34);
        stopAllButton.TabIndex = 2;
        stopAllButton.Text = "Stop all";
        stopAllButton.UseVisualStyleBackColor = false;
        // 
        // saveButton
        // 
        saveButton.BackColor = Color.DimGray;
        saveButton.FlatAppearance.BorderSize = 0;
        saveButton.FlatStyle = FlatStyle.Flat;
        saveButton.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
        saveButton.ForeColor = Color.White;
        saveButton.Location = new Point(960, 68);
        saveButton.Name = "saveButton";
        saveButton.Size = new Size(140, 34);
        saveButton.TabIndex = 3;
        saveButton.Text = "Save settings";
        saveButton.UseVisualStyleBackColor = false;
        // 
        // detectionLabel
        // 
        detectionLabel.BackColor = SystemColors.AppWorkspace;
        detectionLabel.Location = new Point(20, 108);
        detectionLabel.Name = "detectionLabel";
        detectionLabel.Size = new Size(1080, 28);
        detectionLabel.TabIndex = 4;
        detectionLabel.Text = "Detecting SDI ports …";
        // 
        // overview
        // 
        overview.AllowUserToAddRows = false;
        overview.AllowUserToDeleteRows = false;
        overview.AllowUserToResizeRows = false;
        overview.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        overview.BackgroundColor = Color.Gainsboro;
        overview.Columns.AddRange(new DataGridViewColumn[] { portColumn, directionColumn, statusColumn, cpuColumn, ramColumn });
        overview.Location = new Point(20, 139);
        overview.MultiSelect = false;
        overview.Name = "overview";
        overview.ReadOnly = true;
        overview.RowHeadersVisible = false;
        overview.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        overview.Size = new Size(1080, 160);
        overview.TabIndex = 5;
        // 
        // portColumn
        // 
        portColumn.FillWeight = 23F;
        portColumn.HeaderText = "SDI port";
        portColumn.Name = "portColumn";
        portColumn.ReadOnly = true;
        portColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        // 
        // directionColumn
        // 
        directionColumn.FillWeight = 9F;
        directionColumn.HeaderText = "Direction";
        directionColumn.Name = "directionColumn";
        directionColumn.ReadOnly = true;
        directionColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        // 
        // statusColumn
        // 
        statusColumn.FillWeight = 50F;
        statusColumn.HeaderText = "Status";
        statusColumn.Name = "statusColumn";
        statusColumn.ReadOnly = true;
        statusColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        // 
        // cpuColumn
        // 
        cpuColumn.FillWeight = 8F;
        cpuColumn.HeaderText = "CPU";
        cpuColumn.Name = "cpuColumn";
        cpuColumn.ReadOnly = true;
        cpuColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        // 
        // ramColumn
        // 
        ramColumn.FillWeight = 10F;
        ramColumn.HeaderText = "RAM";
        ramColumn.Name = "ramColumn";
        ramColumn.ReadOnly = true;
        ramColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        // 
        // tabs
        // 
        tabs.AccessibleRole = AccessibleRole.TitleBar;
        tabs.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        tabs.Appearance = TabAppearance.FlatButtons;
        tabs.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
        tabs.Location = new Point(20, 305);
        tabs.Name = "tabs";
        tabs.SelectedIndex = 0;
        tabs.Size = new Size(1080, 510);
        tabs.TabIndex = 6;
        // 
        // loadLabel
        // 
        loadLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        loadLabel.BackColor = SystemColors.AppWorkspace;
        loadLabel.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
        loadLabel.ForeColor = Color.Black;
        loadLabel.Location = new Point(20, 834);
        loadLabel.Name = "loadLabel";
        loadLabel.Size = new Size(525, 26);
        loadLabel.TabIndex = 7;
        // 
        // warningLabel
        // 
        warningLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        warningLabel.BackColor = SystemColors.AppWorkspace;
        warningLabel.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
        warningLabel.ForeColor = Color.Black;
        warningLabel.Location = new Point(575, 834);
        warningLabel.Name = "warningLabel";
        warningLabel.Size = new Size(525, 26);
        warningLabel.TabIndex = 8;
        // 
        // footer
        // 
        footer.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        footer.BackColor = SystemColors.ControlDark;
        footer.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        footer.ForeColor = Color.Black;
        footer.LinkArea = new LinkArea(48, 18);
        footer.Location = new Point(575, 877);
        footer.Name = "footer";
        footer.Size = new Size(525, 24);
        footer.TabIndex = 9;
        footer.TabStop = true;
        footer.Text = "NDI® is a registered trademark of Vizrt NDI AB · https://ndi.video/";
        footer.TextAlign = ContentAlignment.MiddleRight;
        footer.UseCompatibleTextRendering = true;
        // 
        // label1
        // 
        label1.BackColor = Color.Black;
        label1.Font = new Font("Segoe UI Semibold", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
        label1.ForeColor = Color.White;
        label1.Location = new Point(20, 0);
        label1.Name = "label1";
        label1.Size = new Size(1080, 54);
        label1.TabIndex = 10;
        label1.Text = "Bidirectional SDI ↔ OMT/NDI Converter";
        label1.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // linkLabel1
        // 
        linkLabel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        linkLabel1.BackColor = SystemColors.ControlDark;
        linkLabel1.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        linkLabel1.ForeColor = Color.Black;
        linkLabel1.LinkArea = new LinkArea(29, 11);
        linkLabel1.Location = new Point(20, 877);
        linkLabel1.Name = "linkLabel1";
        linkLabel1.Size = new Size(525, 24);
        linkLabel1.TabIndex = 11;
        linkLabel1.TabStop = true;
        linkLabel1.Text = "Open Media Transport (OMT) · MIT License";
        linkLabel1.TextAlign = ContentAlignment.MiddleLeft;
        linkLabel1.UseCompatibleTextRendering = true;
        // 
        // MultiChannelForm
        // 
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = SystemColors.ControlDark;
        ClientSize = new Size(1124, 910);
        Controls.Add(linkLabel1);
        Controls.Add(label1);
        Controls.Add(refreshButton);
        Controls.Add(startAllButton);
        Controls.Add(stopAllButton);
        Controls.Add(saveButton);
        Controls.Add(detectionLabel);
        Controls.Add(overview);
        Controls.Add(tabs);
        Controls.Add(loadLabel);
        Controls.Add(warningLabel);
        Controls.Add(footer);
        Font = new Font("Segoe UI", 10F);
        MinimumSize = new Size(1140, 800);
        Name = "MultiChannelForm";
        StartPosition = FormStartPosition.CenterScreen;
        ((System.ComponentModel.ISupportInitialize)overview).EndInit();
        ResumeLayout(false);
    }
    private Button refreshButton, startAllButton, stopAllButton, saveButton;
    private Label detectionLabel, loadLabel, warningLabel;
    private DataGridView overview;
    private DataGridViewTextBoxColumn portColumn;
    private DataGridViewTextBoxColumn directionColumn;
    private DataGridViewTextBoxColumn statusColumn;
    private DataGridViewTextBoxColumn cpuColumn;
    private DataGridViewTextBoxColumn ramColumn;

    private TabControl tabs;
    private LinkLabel footer;
    private Label label1;
    private LinkLabel linkLabel1;
}
