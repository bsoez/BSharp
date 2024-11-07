namespace BSharp
{
    partial class Form1
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
            tbcTabs = new TabControl();
            tbpCompilador = new TabPage();
            lblEntrada = new Label();
            lblSintaxis = new Label();
            lblLexico = new Label();
            txtSintaxis = new RichTextBox();
            button2 = new Button();
            button1 = new Button();
            dgvTablaSimbolos = new DataGridView();
            Column2 = new DataGridViewTextBoxColumn();
            Column3 = new DataGridViewTextBoxColumn();
            Column4 = new DataGridViewTextBoxColumn();
            txtOutput = new RichTextBox();
            txtInput = new RichTextBox();
            btnCompilar = new Button();
            tbpMatriz = new TabPage();
            dgvMatriz = new DataGridView();
            tbcTabs.SuspendLayout();
            tbpCompilador.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvTablaSimbolos).BeginInit();
            tbpMatriz.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMatriz).BeginInit();
            SuspendLayout();
            // 
            // tbcTabs
            // 
            tbcTabs.Controls.Add(tbpCompilador);
            tbcTabs.Controls.Add(tbpMatriz);
            tbcTabs.Location = new Point(12, 12);
            tbcTabs.Name = "tbcTabs";
            tbcTabs.SelectedIndex = 0;
            tbcTabs.Size = new Size(1623, 794);
            tbcTabs.TabIndex = 0;
            // 
            // tbpCompilador
            // 
            tbpCompilador.Controls.Add(lblEntrada);
            tbpCompilador.Controls.Add(lblSintaxis);
            tbpCompilador.Controls.Add(lblLexico);
            tbpCompilador.Controls.Add(txtSintaxis);
            tbpCompilador.Controls.Add(button2);
            tbpCompilador.Controls.Add(button1);
            tbpCompilador.Controls.Add(dgvTablaSimbolos);
            tbpCompilador.Controls.Add(txtOutput);
            tbpCompilador.Controls.Add(txtInput);
            tbpCompilador.Controls.Add(btnCompilar);
            tbpCompilador.Location = new Point(4, 24);
            tbpCompilador.Name = "tbpCompilador";
            tbpCompilador.Padding = new Padding(3);
            tbpCompilador.Size = new Size(1615, 766);
            tbpCompilador.TabIndex = 0;
            tbpCompilador.Text = "Compilador";
            tbpCompilador.UseVisualStyleBackColor = true;
            // 
            // lblEntrada
            // 
            lblEntrada.AutoSize = true;
            lblEntrada.Font = new Font("Graduate", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblEntrada.Location = new Point(22, 25);
            lblEntrada.Name = "lblEntrada";
            lblEntrada.Size = new Size(64, 13);
            lblEntrada.TabIndex = 9;
            lblEntrada.Text = "Entrada";
            // 
            // lblSintaxis
            // 
            lblSintaxis.AutoSize = true;
            lblSintaxis.Font = new Font("Graduate", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblSintaxis.Location = new Point(465, 25);
            lblSintaxis.Name = "lblSintaxis";
            lblSintaxis.Size = new Size(64, 13);
            lblSintaxis.TabIndex = 8;
            lblSintaxis.Text = "Sintaxis";
            // 
            // lblLexico
            // 
            lblLexico.AutoSize = true;
            lblLexico.Font = new Font("Graduate", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblLexico.Location = new Point(249, 25);
            lblLexico.Name = "lblLexico";
            lblLexico.Size = new Size(50, 13);
            lblLexico.TabIndex = 7;
            lblLexico.Text = "Lexico";
            // 
            // txtSintaxis
            // 
            txtSintaxis.Font = new Font("Comic Sans MS", 9.75F);
            txtSintaxis.Location = new Point(465, 49);
            txtSintaxis.Name = "txtSintaxis";
            txtSintaxis.Size = new Size(210, 367);
            txtSintaxis.TabIndex = 6;
            txtSintaxis.Text = "";
            txtSintaxis.WordWrap = false;
            // 
            // button2
            // 
            button2.Location = new Point(755, 457);
            button2.Name = "button2";
            button2.Size = new Size(633, 33);
            button2.TabIndex = 5;
            button2.Text = "Generar archivo de texto Tabla de simbolos";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button1
            // 
            button1.Location = new Point(754, 407);
            button1.Name = "button1";
            button1.Size = new Size(633, 33);
            button1.TabIndex = 4;
            button1.Text = "Generar archivo de texto";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // dgvTablaSimbolos
            // 
            dgvTablaSimbolos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTablaSimbolos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvTablaSimbolos.Columns.AddRange(new DataGridViewColumn[] { Column2, Column3, Column4 });
            dgvTablaSimbolos.Location = new Point(755, 25);
            dgvTablaSimbolos.Name = "dgvTablaSimbolos";
            dgvTablaSimbolos.RowHeadersWidth = 51;
            dgvTablaSimbolos.Size = new Size(632, 367);
            dgvTablaSimbolos.TabIndex = 3;
            // 
            // Column2
            // 
            Column2.HeaderText = "Lexema";
            Column2.MinimumWidth = 6;
            Column2.Name = "Column2";
            // 
            // Column3
            // 
            Column3.HeaderText = "Token";
            Column3.MinimumWidth = 6;
            Column3.Name = "Column3";
            // 
            // Column4
            // 
            Column4.HeaderText = "Tipo";
            Column4.MinimumWidth = 6;
            Column4.Name = "Column4";
            // 
            // txtOutput
            // 
            txtOutput.Font = new Font("Comic Sans MS", 9.75F);
            txtOutput.Location = new Point(249, 49);
            txtOutput.Name = "txtOutput";
            txtOutput.ScrollBars = RichTextBoxScrollBars.Horizontal;
            txtOutput.Size = new Size(210, 367);
            txtOutput.TabIndex = 2;
            txtOutput.Text = "";
            txtOutput.WordWrap = false;
            // 
            // txtInput
            // 
            txtInput.Font = new Font("Comic Sans MS", 9.75F);
            txtInput.Location = new Point(22, 49);
            txtInput.Name = "txtInput";
            txtInput.Size = new Size(210, 367);
            txtInput.TabIndex = 1;
            txtInput.Text = "";
            txtInput.TextChanged += txtInput_TextChanged;
            // 
            // btnCompilar
            // 
            btnCompilar.Location = new Point(22, 431);
            btnCompilar.Name = "btnCompilar";
            btnCompilar.Size = new Size(653, 33);
            btnCompilar.TabIndex = 0;
            btnCompilar.Text = "Compilar";
            btnCompilar.UseVisualStyleBackColor = true;
            btnCompilar.Click += btnCompilar_Click;
            // 
            // tbpMatriz
            // 
            tbpMatriz.Controls.Add(dgvMatriz);
            tbpMatriz.Location = new Point(4, 24);
            tbpMatriz.Name = "tbpMatriz";
            tbpMatriz.Padding = new Padding(3);
            tbpMatriz.Size = new Size(1615, 766);
            tbpMatriz.TabIndex = 1;
            tbpMatriz.Text = "Matriz";
            tbpMatriz.UseVisualStyleBackColor = true;
            // 
            // dgvMatriz
            // 
            dgvMatriz.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMatriz.Location = new Point(24, 24);
            dgvMatriz.Name = "dgvMatriz";
            dgvMatriz.RowHeadersWidth = 51;
            dgvMatriz.Size = new Size(1021, 401);
            dgvMatriz.TabIndex = 5;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1681, 830);
            Controls.Add(tbcTabs);
            Name = "Form1";
            Text = "Form1";
            tbcTabs.ResumeLayout(false);
            tbpCompilador.ResumeLayout(false);
            tbpCompilador.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvTablaSimbolos).EndInit();
            tbpMatriz.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvMatriz).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tbcTabs;
        private TabPage tbpCompilador;
        private TabPage tbpMatriz;
        private Button btnCompilar;
        private DataGridView dgvMatriz;
        private RichTextBox txtOutput;
        private RichTextBox txtInput;
        private DataGridView dgvTablaSimbolos;
        private Button button1;
        private DataGridViewTextBoxColumn Column2;
        private DataGridViewTextBoxColumn Column3;
        private DataGridViewTextBoxColumn Column4;
        private Button button2;
        private Label lblEntrada;
        private Label lblSintaxis;
        private Label lblLexico;
        private RichTextBox txtSintaxis;
    }
}
