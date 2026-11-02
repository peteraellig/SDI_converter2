#nullable disable
namespace SdiConverter;
partial class ChannelControl
{
    private System.ComponentModel.IContainer components = null;
    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }
    private void InitializeComponent()
    {
        portLabel = new Label();
        directionLabel = new Label();
        directionBox = new ComboBox();
        resourceLabel = new Label();
        senderLabel = new Label();
        senderBox = new TextBox();
        outputLabel = new Label();
        outputBox = new ComboBox();
        qualityLabel = new Label();
        qualityBox = new ComboBox();
        duplicateLabel = new Label();
        duplicateBox = new ComboBox();
        sourceLabel = new Label();
        kindBox = new ComboBox();
        sourceBox = new ComboBox();
        findButton = new Button();
        audioLabel = new Label();
        audioBox = new ComboBox();
        bufferLabel = new Label();
        bufferBox = new NumericUpDown();
        startButton = new Button();
        logButton = new Button();
        statusLabel = new Label();
        formatLabel = new Label();
        detailLabel = new Label();
        ((System.ComponentModel.ISupportInitialize)bufferBox).BeginInit();
        SuspendLayout();
        // 
        // portLabel
        // 
        portLabel.BackColor = Color.Black;
        portLabel.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        portLabel.ForeColor = Color.White;
        portLabel.Location = new Point(0, 0);
        portLabel.Name = "portLabel";
        portLabel.Size = new Size(1060, 45);
        portLabel.TabIndex = 0;
        portLabel.Text = "SDI port";
        portLabel.TextAlign = ContentAlignment.MiddleCenter;
        portLabel.Click += portLabel_Click;
        // 
        // directionLabel
        // 
        directionLabel.Font = new Font("Segoe UI", 12F);
        directionLabel.Location = new Point(5, 60);
        directionLabel.Name = "directionLabel";
        directionLabel.Size = new Size(130, 22);
        directionLabel.TabIndex = 1;
        directionLabel.Text = "SDI direction";
        directionLabel.TextAlign = ContentAlignment.MiddleRight;
        // 
        // directionBox
        // 
        directionBox.DropDownStyle = ComboBoxStyle.DropDownList;
        directionBox.Font = new Font("Segoe UI", 12F);
        directionBox.Location = new Point(141, 56);
        directionBox.Name = "directionBox";
        directionBox.Size = new Size(190, 29);
        directionBox.TabIndex = 2;
        // 
        // resourceLabel
        // 
        resourceLabel.Font = new Font("Segoe UI", 12F);
        resourceLabel.Location = new Point(498, 60);
        resourceLabel.Name = "resourceLabel";
        resourceLabel.Size = new Size(460, 25);
        resourceLabel.TabIndex = 3;
        resourceLabel.Text = "Channel CPU: —    RAM: —";
        // 
        // senderLabel
        // 
        senderLabel.Font = new Font("Segoe UI", 12F);
        senderLabel.Location = new Point(350, 101);
        senderLabel.Name = "senderLabel";
        senderLabel.Size = new Size(148, 22);
        senderLabel.TabIndex = 4;
        senderLabel.Text = "OMT Sender name";
        senderLabel.TextAlign = ContentAlignment.MiddleRight;
        // 
        // senderBox
        // 
        senderBox.Font = new Font("Segoe UI", 12F);
        senderBox.Location = new Point(498, 97);
        senderBox.Name = "senderBox";
        senderBox.Size = new Size(289, 29);
        senderBox.TabIndex = 5;
        // 
        // outputLabel
        // 
        outputLabel.Font = new Font("Segoe UI", 12F);
        outputLabel.Location = new Point(45, 101);
        outputLabel.Name = "outputLabel";
        outputLabel.Size = new Size(90, 22);
        outputLabel.TabIndex = 6;
        outputLabel.Text = "Send as";
        outputLabel.TextAlign = ContentAlignment.MiddleRight;
        // 
        // outputBox
        // 
        outputBox.DropDownStyle = ComboBoxStyle.DropDownList;
        outputBox.Font = new Font("Segoe UI", 12F);
        outputBox.Items.AddRange(new object[] { "OMT", "NDI", "OMT + NDI" });
        outputBox.Location = new Point(141, 97);
        outputBox.Name = "outputBox";
        outputBox.Size = new Size(190, 29);
        outputBox.TabIndex = 7;
        // 
        // qualityLabel
        // 
        qualityLabel.Font = new Font("Segoe UI", 12F);
        qualityLabel.Location = new Point(5, 141);
        qualityLabel.Name = "qualityLabel";
        qualityLabel.Size = new Size(130, 22);
        qualityLabel.TabIndex = 8;
        qualityLabel.Text = "OMT quality";
        qualityLabel.TextAlign = ContentAlignment.MiddleRight;
        // 
        // qualityBox
        // 
        qualityBox.DropDownStyle = ComboBoxStyle.DropDownList;
        qualityBox.Font = new Font("Segoe UI", 12F);
        qualityBox.Items.AddRange(new object[] { "Normal", "High", "Low" });
        qualityBox.Location = new Point(141, 139);
        qualityBox.Name = "qualityBox";
        qualityBox.Size = new Size(190, 29);
        qualityBox.TabIndex = 9;
        // 
        // duplicateLabel
        // 
        duplicateLabel.Font = new Font("Segoe UI", 12F);
        duplicateLabel.Location = new Point(793, 99);
        duplicateLabel.Name = "duplicateLabel";
        duplicateLabel.Size = new Size(110, 22);
        duplicateLabel.TabIndex = 10;
        duplicateLabel.Text = "If name exists";
        duplicateLabel.TextAlign = ContentAlignment.MiddleRight;
        // 
        // duplicateBox
        // 
        duplicateBox.DropDownStyle = ComboBoxStyle.DropDownList;
        duplicateBox.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
        duplicateBox.Items.AddRange(new object[] { "Abort", "Add a number", "Retry until free", "Retry 10 minutes" });
        duplicateBox.Location = new Point(900, 99);
        duplicateBox.Name = "duplicateBox";
        duplicateBox.Size = new Size(140, 25);
        duplicateBox.TabIndex = 11;
        // 
        // sourceLabel
        // 
        sourceLabel.Font = new Font("Segoe UI", 12F);
        sourceLabel.Location = new Point(5, 203);
        sourceLabel.Name = "sourceLabel";
        sourceLabel.Size = new Size(130, 22);
        sourceLabel.TabIndex = 12;
        sourceLabel.Text = "Network source";
        sourceLabel.TextAlign = ContentAlignment.MiddleRight;
        // 
        // kindBox
        // 
        kindBox.DropDownStyle = ComboBoxStyle.DropDownList;
        kindBox.Font = new Font("Segoe UI", 12F);
        kindBox.Items.AddRange(new object[] { "OMT", "NDI" });
        kindBox.Location = new Point(141, 200);
        kindBox.Name = "kindBox";
        kindBox.Size = new Size(185, 29);
        kindBox.TabIndex = 13;
        // 
        // sourceBox
        // 
        sourceBox.BackColor = Color.White;
        sourceBox.Font = new Font("Segoe UI", 12F);
        sourceBox.Location = new Point(365, 201);
        sourceBox.Name = "sourceBox";
        sourceBox.Size = new Size(515, 29);
        sourceBox.TabIndex = 14;
        // 
        // findButton
        // 
        findButton.BackColor = Color.DimGray;
        findButton.FlatAppearance.BorderSize = 0;
        findButton.FlatStyle = FlatStyle.Flat;
        findButton.Font = new Font("Segoe UI", 12F);
        findButton.ForeColor = Color.White;
        findButton.Location = new Point(900, 199);
        findButton.Name = "findButton";
        findButton.Size = new Size(130, 30);
        findButton.TabIndex = 15;
        findButton.Text = "Find sources";
        findButton.UseVisualStyleBackColor = false;
        // 
        // audioLabel
        // 
        audioLabel.Font = new Font("Segoe UI", 12F);
        audioLabel.Location = new Point(5, 262);
        audioLabel.Name = "audioLabel";
        audioLabel.Size = new Size(130, 22);
        audioLabel.TabIndex = 16;
        audioLabel.Text = "Audio routing";
        audioLabel.TextAlign = ContentAlignment.MiddleRight;
        // 
        // audioBox
        // 
        audioBox.DropDownStyle = ComboBoxStyle.DropDownList;
        audioBox.Font = new Font("Segoe UI", 12F);
        audioBox.Items.AddRange(new object[] { "Stereo 1&2", "Stereo 3&4", "Stereo 5&6", "Stereo 7&8", "Mono 1", "Mono 2", "Mono 3", "Mono 4", "Mono 5", "Mono 6", "Mono 7", "Mono 8", "Off" });
        audioBox.Location = new Point(141, 258);
        audioBox.Name = "audioBox";
        audioBox.Size = new Size(185, 29);
        audioBox.TabIndex = 17;
        // 
        // bufferLabel
        // 
        bufferLabel.Font = new Font("Segoe UI", 12F);
        bufferLabel.Location = new Point(700, 269);
        bufferLabel.Name = "bufferLabel";
        bufferLabel.Size = new Size(180, 22);
        bufferLabel.TabIndex = 18;
        bufferLabel.Text = "Output buffer (frames)";
        bufferLabel.TextAlign = ContentAlignment.MiddleRight;
        // 
        // bufferBox
        // 
        bufferBox.BackColor = Color.White;
        bufferBox.Font = new Font("Segoe UI", 12F);
        bufferBox.Location = new Point(900, 262);
        bufferBox.Maximum = new decimal(new int[] { 8, 0, 0, 0 });
        bufferBox.Minimum = new decimal(new int[] { 3, 0, 0, 0 });
        bufferBox.Name = "bufferBox";
        bufferBox.Size = new Size(130, 29);
        bufferBox.TabIndex = 19;
        bufferBox.Value = new decimal(new int[] { 3, 0, 0, 0 });
        // 
        // startButton
        // 
        startButton.BackColor = Color.DimGray;
        startButton.FlatAppearance.BorderSize = 0;
        startButton.FlatStyle = FlatStyle.Flat;
        startButton.Font = new Font("Segoe UI", 14.25F);
        startButton.ForeColor = Color.White;
        startButton.Location = new Point(20, 392);
        startButton.Name = "startButton";
        startButton.Size = new Size(180, 40);
        startButton.TabIndex = 20;
        startButton.Text = "Start channel";
        startButton.UseVisualStyleBackColor = false;
        // 
        // logButton
        // 
        logButton.BackColor = Color.DimGray;
        logButton.FlatAppearance.BorderSize = 0;
        logButton.FlatStyle = FlatStyle.Flat;
        logButton.Font = new Font("Segoe UI", 14.25F);
        logButton.ForeColor = Color.White;
        logButton.Location = new Point(492, 392);
        logButton.Name = "logButton";
        logButton.Size = new Size(180, 40);
        logButton.TabIndex = 21;
        logButton.Text = "Diagnostics";
        logButton.UseVisualStyleBackColor = false;
        // 
        // statusLabel
        // 
        statusLabel.BackColor = Color.Gainsboro;
        statusLabel.Location = new Point(20, 317);
        statusLabel.Name = "statusLabel";
        statusLabel.Size = new Size(452, 44);
        statusLabel.TabIndex = 22;
        statusLabel.Text = "Ready";
        // 
        // formatLabel
        // 
        formatLabel.BackColor = Color.Gainsboro;
        formatLabel.Location = new Point(492, 317);
        formatLabel.Name = "formatLabel";
        formatLabel.Size = new Size(548, 44);
        formatLabel.TabIndex = 23;
        // 
        // detailLabel
        // 
        detailLabel.Location = new Point(20, 392);
        detailLabel.Name = "detailLabel";
        detailLabel.Size = new Size(1010, 60);
        detailLabel.TabIndex = 24;
        // 
        // ChannelControl
        // 
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.DarkGray;
        Controls.Add(senderBox);
        Controls.Add(duplicateBox);
        Controls.Add(portLabel);
        Controls.Add(directionLabel);
        Controls.Add(directionBox);
        Controls.Add(resourceLabel);
        Controls.Add(senderLabel);
        Controls.Add(outputLabel);
        Controls.Add(outputBox);
        Controls.Add(qualityLabel);
        Controls.Add(qualityBox);
        Controls.Add(duplicateLabel);
        Controls.Add(sourceLabel);
        Controls.Add(kindBox);
        Controls.Add(sourceBox);
        Controls.Add(findButton);
        Controls.Add(audioLabel);
        Controls.Add(audioBox);
        Controls.Add(bufferLabel);
        Controls.Add(bufferBox);
        Controls.Add(startButton);
        Controls.Add(logButton);
        Controls.Add(statusLabel);
        Controls.Add(formatLabel);
        Controls.Add(detailLabel);
        Font = new Font("Segoe UI", 10F);
        Name = "ChannelControl";
        Size = new Size(1060, 470);
        ((System.ComponentModel.ISupportInitialize)bufferBox).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }
    private Label portLabel;
    private Label directionLabel;
    private ComboBox directionBox;
    private Label resourceLabel;
    private Label senderLabel;
    private TextBox senderBox;
    private Label outputLabel;
    private ComboBox outputBox;
    private Label qualityLabel;
    private ComboBox qualityBox;
    private Label duplicateLabel;
    private ComboBox duplicateBox;
    private Label sourceLabel;
    private ComboBox kindBox;
    private ComboBox sourceBox;
    private Button findButton;
    private Label audioLabel;
    private ComboBox audioBox;
    private Label bufferLabel;
    private NumericUpDown bufferBox;
    private Button startButton;
    private Button logButton;
    private Label statusLabel;
    private Label formatLabel;
    private Label detailLabel;
}