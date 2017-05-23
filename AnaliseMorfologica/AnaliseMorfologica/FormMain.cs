using AnaliseMorfologicaLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AnaliseMorfologica {
	public partial class FormMain : Form {
		private Imagem entrada;

		public FormMain() {
			InitializeComponent();
		}

		private void img_Click(object sender, EventArgs e) {
			PictureBox pic = (sender as PictureBox);
			if (pic.SizeMode == PictureBoxSizeMode.Zoom) {
				pic.SizeMode = PictureBoxSizeMode.CenterImage;
			} else {
				pic.SizeMode = PictureBoxSizeMode.Zoom;
			}
		}

		private void btnAbrir_Click(object sender, EventArgs e) {
			if (openFileDialog.ShowDialog(this) != DialogResult.OK) {
				return;
			}

			try {
				entrada = new Imagem(openFileDialog.FileName);

				imgEntrada.Image = entrada.CriarBitmap();
			} catch (Exception ex) {
				MessageBox.Show("Erro: " + ex.Message, "Oops...", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void btnSalvar_Click(object sender, EventArgs e) {
			if (imgSaida.Image == null) {
				MessageBox.Show("Nada para salvar na saída!", "Oops...", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			if (saveFileDialog.ShowDialog(this) != DialogResult.OK) {
				return;
			}

			try {
				imgSaida.Image.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
			} catch (Exception ex) {
				MessageBox.Show("Erro: " + ex.Message, "Oops...", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void btnProcessar_Click(object sender, EventArgs e) {
			if (entrada == null) {
				MessageBox.Show("Nada para processar na entrada!", "Oops...", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			Imagem saida = entrada.Clonar();

			// Processar!!!
			saida.ConverterParaEscalaDeCinza();
            
            saida.LimitarInvertido(180);

            List<Forma> formas = new List<Forma>();
            saida.CriarMapaDeFormas(formas);

            List<Forma> validadoresProva = new List<Forma>();
            foreach (Forma form in formas)
            {
                if ((form.X0 == 29 || form.X0 == 636) 
                    && (form.Y1 == 85 || form.Y1 == 548 || form.Y1 == 1012))                    
                {
                    validadoresProva.Add(form);
                    System.Diagnostics.Debug.Write("Forma " + validadoresProva.Count + ": " + form.X0 + "," + form.Y0 + " - " + form.X1 + "," + form.Y1 + "\n");                    
                }                
            }

            imgSaida.Image = saida.CriarBitmap();

            if (validadoresProva.Count == 6)
            {
                // Exibir saída                
                MessageBox.Show("Isto é uma prova!", "Boa!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }else { 
                MessageBox.Show("Isto não é uma prova!", "Opss..", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            List<string> resp = new List<string>();
            foreach (Forma form in formas)
            {
                if(form.X0 == validadoresProva[0].X1+121)
                {
                    System.Diagnostics.Debug.Write(form.X0 + "," + form.Y0 + " - " + form.X1 + "," + form.Y1 + " Área: " + form.Area + "\n");
                }
                if (form.X0 == validadoresProva[0].X1 + 186)
                {
                    System.Diagnostics.Debug.Write(form.X0 + "," + form.Y0 + " - " + form.X1 + "," + form.Y1 + " Área: " + form.Area + "\n");
                }
                if (form.X0 == validadoresProva[0].X1 + 151)
                {
                    System.Diagnostics.Debug.Write(form.X0 + "," + form.Y0 + " - " + form.X1 + "," + form.Y1 + " Área: " + form.Area + "\n");
                }
                if (form.X0 == validadoresProva[0].X1 + 216)
                {
                    System.Diagnostics.Debug.Write(form.X0 + "," + form.Y0 + " - " + form.X1 + "," + form.Y1 + " Área: " + form.Area + "\n");
                }
                if (form.X0 == validadoresProva[5].X1 - 122)
                {
                    System.Diagnostics.Debug.Write(form.X0 + "," + form.Y0 + " - " + form.X1 + "," + form.Y1 + " Área: " + form.Area + "\n");
                }
            }

        }
	}
}
