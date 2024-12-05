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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            tbcTabs = new TabControl();
            tbpCompilador = new TabPage();
            btnGuardarCodigoFinal = new Button();
            lblEnsamblador = new Label();
            txtEnsamblador = new RichTextBox();
            lblTablaSimbolos = new Label();
            pcbBSharp = new PictureBox();
            lblBSharp = new Label();
            lblTripletas = new Label();
            dgvQuadruple = new DataGridView();
            txtErrors = new RichTextBox();
            lblErrores = new Label();
            lblEntrada = new Label();
            lblSintaxis = new Label();
            lblLexico = new Label();
            txtSintaxis = new RichTextBox();
            btnTablaSimbolos = new Button();
            btnArchivoTexto = new Button();
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
            ((System.ComponentModel.ISupportInitialize)pcbBSharp).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvQuadruple).BeginInit();
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
            tbcTabs.Size = new Size(1657, 952);
            tbcTabs.TabIndex = 0;
            // 
            // tbpCompilador
            // 
            tbpCompilador.Controls.Add(btnGuardarCodigoFinal);
            tbpCompilador.Controls.Add(lblEnsamblador);
            tbpCompilador.Controls.Add(txtEnsamblador);
            tbpCompilador.Controls.Add(lblTablaSimbolos);
            tbpCompilador.Controls.Add(pcbBSharp);
            tbpCompilador.Controls.Add(lblBSharp);
            tbpCompilador.Controls.Add(lblTripletas);
            tbpCompilador.Controls.Add(dgvQuadruple);
            tbpCompilador.Controls.Add(txtErrors);
            tbpCompilador.Controls.Add(lblErrores);
            tbpCompilador.Controls.Add(lblEntrada);
            tbpCompilador.Controls.Add(lblSintaxis);
            tbpCompilador.Controls.Add(lblLexico);
            tbpCompilador.Controls.Add(txtSintaxis);
            tbpCompilador.Controls.Add(btnTablaSimbolos);
            tbpCompilador.Controls.Add(btnArchivoTexto);
            tbpCompilador.Controls.Add(dgvTablaSimbolos);
            tbpCompilador.Controls.Add(txtOutput);
            tbpCompilador.Controls.Add(txtInput);
            tbpCompilador.Controls.Add(btnCompilar);
            tbpCompilador.Location = new Point(4, 24);
            tbpCompilador.Name = "tbpCompilador";
            tbpCompilador.Padding = new Padding(3);
            tbpCompilador.Size = new Size(1649, 924);
            tbpCompilador.TabIndex = 0;
            tbpCompilador.Text = "Compilador";
            tbpCompilador.UseVisualStyleBackColor = true;
            // 
            // btnGuardarCodigoFinal
            // 
            btnGuardarCodigoFinal.Location = new Point(755, 889);
            btnGuardarCodigoFinal.Name = "btnGuardarCodigoFinal";
            btnGuardarCodigoFinal.Size = new Size(876, 23);
            btnGuardarCodigoFinal.TabIndex = 19;
            btnGuardarCodigoFinal.Text = "Generar archivo ensamblador";
            btnGuardarCodigoFinal.UseVisualStyleBackColor = true;
            btnGuardarCodigoFinal.Click += btnGuardarCodigoFinal_Click;
            // 
            // lblEnsamblador
            // 
            lblEnsamblador.AutoSize = true;
            lblEnsamblador.Font = new Font("Graduate", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblEnsamblador.Location = new Point(1202, 646);
            lblEnsamblador.Name = "lblEnsamblador";
            lblEnsamblador.Size = new Size(91, 13);
            lblEnsamblador.TabIndex = 18;
            lblEnsamblador.Text = "Código final";
            // 
            // txtEnsamblador
            // 
            txtEnsamblador.Font = new Font("Comic Sans MS", 9.75F);
            txtEnsamblador.Location = new Point(1202, 663);
            txtEnsamblador.Name = "txtEnsamblador";
            txtEnsamblador.Size = new Size(429, 220);
            txtEnsamblador.TabIndex = 17;
            txtEnsamblador.Text = "";
            txtEnsamblador.WordWrap = false;
            // 
            // lblTablaSimbolos
            // 
            lblTablaSimbolos.AutoSize = true;
            lblTablaSimbolos.Font = new Font("Graduate", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTablaSimbolos.Location = new Point(754, 61);
            lblTablaSimbolos.Name = "lblTablaSimbolos";
            lblTablaSimbolos.Size = new Size(130, 13);
            lblTablaSimbolos.TabIndex = 16;
            lblTablaSimbolos.Text = "Tabla de Símbolos";
            // 
            // pcbBSharp
            // 
            pcbBSharp.Image = (Image)resources.GetObject("pcbBSharp.Image");
            pcbBSharp.Location = new Point(156, 6);
            pcbBSharp.Name = "pcbBSharp";
            pcbBSharp.Size = new Size(60, 60);
            pcbBSharp.SizeMode = PictureBoxSizeMode.StretchImage;
            pcbBSharp.TabIndex = 15;
            pcbBSharp.TabStop = false;
            // 
            // lblBSharp
            // 
            lblBSharp.AutoSize = true;
            lblBSharp.Font = new Font("Graduate", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblBSharp.Location = new Point(22, 17);
            lblBSharp.Name = "lblBSharp";
            lblBSharp.Size = new Size(132, 33);
            lblBSharp.TabIndex = 14;
            lblBSharp.Text = "BSharp";
            // 
            // lblTripletas
            // 
            lblTripletas.AutoSize = true;
            lblTripletas.Font = new Font("Graduate", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTripletas.Location = new Point(754, 646);
            lblTripletas.Name = "lblTripletas";
            lblTripletas.Size = new Size(71, 13);
            lblTripletas.TabIndex = 13;
            lblTripletas.Text = "Tripletas";
            // 
            // dgvQuadruple
            // 
            dgvQuadruple.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvQuadruple.Location = new Point(754, 663);
            dgvQuadruple.Name = "dgvQuadruple";
            dgvQuadruple.Size = new Size(442, 220);
            dgvQuadruple.TabIndex = 12;
            // 
            // txtErrors
            // 
            txtErrors.Location = new Point(3, 663);
            txtErrors.Name = "txtErrors";
            txtErrors.ReadOnly = true;
            txtErrors.Size = new Size(672, 258);
            txtErrors.TabIndex = 11;
            txtErrors.Text = "";
            // 
            // lblErrores
            // 
            lblErrores.AutoSize = true;
            lblErrores.Font = new Font("Graduate", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblErrores.Location = new Point(6, 646);
            lblErrores.Name = "lblErrores";
            lblErrores.Size = new Size(120, 13);
            lblErrores.TabIndex = 10;
            lblErrores.Text = "Lista de Errores";
            // 
            // lblEntrada
            // 
            lblEntrada.AutoSize = true;
            lblEntrada.Font = new Font("Graduate", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblEntrada.Location = new Point(22, 80);
            lblEntrada.Name = "lblEntrada";
            lblEntrada.Size = new Size(64, 13);
            lblEntrada.TabIndex = 9;
            lblEntrada.Text = "Entrada";
            // 
            // lblSintaxis
            // 
            lblSintaxis.AutoSize = true;
            lblSintaxis.Font = new Font("Graduate", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblSintaxis.Location = new Point(465, 80);
            lblSintaxis.Name = "lblSintaxis";
            lblSintaxis.Size = new Size(64, 13);
            lblSintaxis.TabIndex = 8;
            lblSintaxis.Text = "Sintaxis";
            // 
            // lblLexico
            // 
            lblLexico.AutoSize = true;
            lblLexico.Font = new Font("Graduate", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblLexico.Location = new Point(249, 80);
            lblLexico.Name = "lblLexico";
            lblLexico.Size = new Size(50, 13);
            lblLexico.TabIndex = 7;
            lblLexico.Text = "Lexico";
            // 
            // txtSintaxis
            // 
            txtSintaxis.Font = new Font("Comic Sans MS", 9.75F);
            txtSintaxis.Location = new Point(465, 104);
            txtSintaxis.Name = "txtSintaxis";
            txtSintaxis.Size = new Size(210, 367);
            txtSintaxis.TabIndex = 6;
            txtSintaxis.Text = "";
            txtSintaxis.WordWrap = false;
            // 
            // btnTablaSimbolos
            // 
            btnTablaSimbolos.Location = new Point(754, 486);
            btnTablaSimbolos.Name = "btnTablaSimbolos";
            btnTablaSimbolos.Size = new Size(877, 31);
            btnTablaSimbolos.TabIndex = 5;
            btnTablaSimbolos.Text = "Generar archivo de texto Tabla de simbolos";
            btnTablaSimbolos.UseVisualStyleBackColor = true;
            btnTablaSimbolos.Click += button2_Click;
            // 
            // btnArchivoTexto
            // 
            btnArchivoTexto.Location = new Point(754, 453);
            btnArchivoTexto.Name = "btnArchivoTexto";
            btnArchivoTexto.Size = new Size(877, 31);
            btnArchivoTexto.TabIndex = 4;
            btnArchivoTexto.Text = "Generar archivo de texto";
            btnArchivoTexto.UseVisualStyleBackColor = true;
            btnArchivoTexto.Click += button1_Click;
            // 
            // dgvTablaSimbolos
            // 
            dgvTablaSimbolos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTablaSimbolos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvTablaSimbolos.Columns.AddRange(new DataGridViewColumn[] { Column2, Column3, Column4 });
            dgvTablaSimbolos.Location = new Point(755, 80);
            dgvTablaSimbolos.Name = "dgvTablaSimbolos";
            dgvTablaSimbolos.RowHeadersWidth = 51;
            dgvTablaSimbolos.Size = new Size(876, 367);
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
            txtOutput.Location = new Point(249, 104);
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
            txtInput.Location = new Point(22, 104);
            txtInput.Name = "txtInput";
            txtInput.ScrollBars = RichTextBoxScrollBars.Horizontal;
            txtInput.Size = new Size(210, 367);
            txtInput.TabIndex = 1;
            txtInput.Text = "";
            txtInput.TextChanged += txtInput_TextChanged;
            // 
            // btnCompilar
            // 
            btnCompilar.Location = new Point(22, 486);
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
            tbpMatriz.Size = new Size(1649, 924);
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
            ClientSize = new Size(1681, 976);
            Controls.Add(tbcTabs);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "BSharp";
            tbcTabs.ResumeLayout(false);
            tbpCompilador.ResumeLayout(false);
            tbpCompilador.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pcbBSharp).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvQuadruple).EndInit();
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
        private Button btnArchivoTexto;
        private DataGridViewTextBoxColumn Column2;
        private DataGridViewTextBoxColumn Column3;
        private DataGridViewTextBoxColumn Column4;
        private Button btnTablaSimbolos;
        private Label lblEntrada;
        private Label lblSintaxis;
        private Label lblLexico;
        private RichTextBox txtSintaxis;
        private Label lblErrores;
        private RichTextBox txtErrors;
        private DataGridView dgvQuadruple;
        private Label lblBSharp;
        private Label lblTripletas;
        private PictureBox pcbBSharp;
        private Label lblTablaSimbolos;
        private Button btnGuardarCodigoFinal;
        private Label lblEnsamblador;
        private RichTextBox txtEnsamblador;
    }
}
