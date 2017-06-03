using AnaliseMorfologicaLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AnaliseMorfologica
{
    public partial class FormMain : Form
    {
        private Imagem entrada;

        public FormMain()
        {
            InitializeComponent();
        }

        private void img_Click(object sender, EventArgs e)
        {
            PictureBox pic = (sender as PictureBox);
            if (pic.SizeMode == PictureBoxSizeMode.Zoom)
            {
                pic.SizeMode = PictureBoxSizeMode.CenterImage;
            }
            else
            {
                pic.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }

        private void btnAbrir_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            try
            {
                entrada = new Imagem(openFileDialog.FileName);

                imgEntrada.Image = entrada.CriarBitmap();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message, "Oops...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            if (imgSaida.Image == null)
            {
                MessageBox.Show("Nada para salvar na saída!", "Oops...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (saveFileDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            try
            {
                imgSaida.Image.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message, "Oops...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnProcessar_Click(object sender, EventArgs e)
        {
            if (entrada == null)
            {
                MessageBox.Show("Nada para processar na entrada!", "Oops...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Imagem saida = entrada.Clonar();

            // Processar!!!
            saida.ConverterParaEscalaDeCinza();

            int media = saida.CalcularMedia();

            saida.LimitarInvertido(media);

            imgSaida.Image = saida.CriarBitmap();

            List<Forma> formas = new List<Forma>();
            saida.CriarMapaDeFormas(formas);

            int cont = 0;

            int[] x0s = new int[] { 0, 609, 0, 609, 0, 609 };
            int[] y0s = new int[] { 0, 0, 474, 474, 934, 934 };
            int[] x1s = new int[] { 117, 719, 117, 719, 117, 719 };
            int[] y1s = new int[] { 112, 112, 570, 570, 1039, 1039 };

            for (int ss = 0; ss < 6; ss++)
            {
                int posicao = 0;
                foreach (Forma form in formas)
                {
                    if (formas[posicao].FazInterseccao(x0s[ss], y0s[ss], x1s[ss], y1s[ss]))
                    {
                        if (formas[posicao].Area > 3150 && formas[posicao].Area < 3300)
                        {
                            cont += 1;
                        }
                        else
                        {
                            imgSaida.Image = saida.CriarBitmap();
                            MessageBox.Show("Isto não é uma prova!", "Opss..", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    posicao++;
                }
            }

            if (cont != 6)
            {
                MessageBox.Show("Isto não é uma prova!", "Opss..", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string[] resp = new string[10];
            int X0inicial = 198, Y0inicial = 321;
            int X1inicial = 262, Y1inicial = 385;
            int area = 0;

            for (int linha = 0; linha < 10; linha++)
            {
                for (int coluna = 0; coluna < 5; coluna++)
                {
                    foreach (Forma form in formas)
                    {
                        if (form.FazInterseccao(X0inicial + (65 * coluna), Y0inicial, X1inicial + (65 * coluna), Y1inicial))
                        {
                            area += form.Area;
                            if (area > 800 && area < 3800)
                            {
                                if (resp[linha] == null)
                                {
                                    string resposta = ((char)('A' + coluna)).ToString();
                                    resp[linha] = resposta;
                                    System.Diagnostics.Debug.WriteLine("Questão " + (linha + 1) + ": " + resposta);
                                }
                                else
                                {
                                    string resposta = ((char)('A' + coluna)).ToString();
                                    resp[linha] = string.Concat(resp[linha], ", ", resposta);
                                    System.Diagnostics.Debug.WriteLine("Questão " + (linha + 1) + ": " + resp[linha]);
                                }
                            }
                        }
                    }
                    area = 0;
                }
                Y0inicial += 65;
                Y1inicial += 65;
            }

            StringBuilder output = new StringBuilder();

            for (int i = 0; i < resp.Length; i++)
            {
                output.Append(i + 1);
                output.Append(": ");
                output.AppendLine(resp[i] ?? "");
            }

            MessageBox.Show(output.ToString(), "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
    }
}

