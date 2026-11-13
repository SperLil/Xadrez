namespace Xadrez;

public partial class Form1 : Form
{
    Peca?[,] tabuleiro = new Peca?[8, 8];
    const int TamanhoCasa = 80;
    int linhaSelecionada = -1;
    int colunaSelecionada = -1;

    public Form1()
    {
        InitializeComponent();

        Text = "Xadrez";
        ClientSize = new Size(640, 640);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        DoubleBuffered = true;

        PosicionarPecas();
    }

    void PosicionarPecas()
    {
        TipoPeca[] ordem =
        {
            TipoPeca.Torre, TipoPeca.Cavalo, TipoPeca.Bispo, TipoPeca.Rainha,
            TipoPeca.Rei, TipoPeca.Bispo, TipoPeca.Cavalo, TipoPeca.Torre
        };

        for (int coluna = 0; coluna < 8; coluna++)
        {
            tabuleiro[0, coluna] = new Peca(ordem[coluna], Cor.Preta);
            tabuleiro[1, coluna] = new Peca(TipoPeca.Peao, Cor.Preta);
            tabuleiro[6, coluna] = new Peca(TipoPeca.Peao, Cor.Branca);
            tabuleiro[7, coluna] = new Peca(ordem[coluna], Cor.Branca);
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Graphics g = e.Graphics;

        for (int linha = 0; linha < 8; linha++)
        {
            for (int coluna = 0; coluna < 8; coluna++)
            {
                bool casaClara = (linha + coluna) % 2 == 0;

                Color cor = casaClara
                    ? Color.FromArgb(240, 217, 181)
                    : Color.FromArgb(181, 136, 99);

                if (linha == linhaSelecionada && coluna == colunaSelecionada)
                {
                    cor = Color.FromArgb(246, 246, 105);
                }

                using var pincel = new SolidBrush(cor);

                g.FillRectangle(pincel,
                    coluna * TamanhoCasa, linha * TamanhoCasa,
                    TamanhoCasa, TamanhoCasa);

                Peca? peca = tabuleiro[linha, coluna];
                if (peca != null)
                {
                    DesenharPeca(g, peca, linha, coluna);
                }
            }
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);

        int linha = e.Y / TamanhoCasa;
        int coluna = e.X / TamanhoCasa;

        if (linha < 0 || linha > 7 || coluna < 0 || coluna > 7)
            return;

        if (linhaSelecionada == -1)
        {
            if (tabuleiro[linha, coluna] != null)
            {
                linhaSelecionada = linha;
                colunaSelecionada = coluna;
            }
        }
        else
        {
            bool mesmaCasa = linha == linhaSelecionada && coluna == colunaSelecionada;

            if (!mesmaCasa && MovimentoValido(linhaSelecionada, colunaSelecionada, linha, coluna))
            {
                tabuleiro[linha, coluna] = tabuleiro[linhaSelecionada, colunaSelecionada];
                tabuleiro[linhaSelecionada, colunaSelecionada] = null;
            }

            linhaSelecionada = -1;
            colunaSelecionada = -1;
        }

        Invalidate();
    }

    bool MovimentoValido(int lo, int co, int ld, int cd)
    {
        Peca? peca = tabuleiro[lo, co];
        if (peca == null) return false;

        Peca? destino = tabuleiro[ld, cd];
        if (destino != null && destino.Cor == peca.Cor) return false;

        int dl = ld - lo;
        int dc = cd - co;
        int adl = Math.Abs(dl);
        int adc = Math.Abs(dc);

        switch (peca.Tipo)
        {
            case TipoPeca.Torre:
                return (dl == 0 || dc == 0) && CaminhoLivre(lo, co, ld, cd);

            case TipoPeca.Bispo:
                return adl == adc && CaminhoLivre(lo, co, ld, cd);

            case TipoPeca.Rainha:
                return (dl == 0 || dc == 0 || adl == adc) && CaminhoLivre(lo, co, ld, cd);

            case TipoPeca.Cavalo:
                return (adl == 2 && adc == 1) || (adl == 1 && adc == 2);

            case TipoPeca.Rei:
                return adl <= 1 && adc <= 1;

            case TipoPeca.Peao:
                return MovimentoPeao(peca, lo, co, ld, cd);
        }

        return false;
    }

    bool CaminhoLivre(int lo, int co, int ld, int cd)
    {
        int passoLinha = Math.Sign(ld - lo);
        int passoColuna = Math.Sign(cd - co);

        int l = lo + passoLinha;
        int c = co + passoColuna;

        while (l != ld || c != cd)
        {
            if (tabuleiro[l, c] != null) return false;
            l += passoLinha;
            c += passoColuna;
        }

        return true;
    }

    bool MovimentoPeao(Peca peao, int lo, int co, int ld, int cd)
    {
        int direcao = peao.Cor == Cor.Branca ? -1 : 1;
        int linhaInicial = peao.Cor == Cor.Branca ? 6 : 1;

        int dl = ld - lo;
        int dc = Math.Abs(cd - co);
        Peca? destino = tabuleiro[ld, cd];

        if (dc == 0 && destino == null)
        {
            if (dl == direcao) return true;

            if (lo == linhaInicial && dl == 2 * direcao && tabuleiro[lo + direcao, co] == null)
                return true;
        }

        if (dc == 1 && dl == direcao && destino != null)
            return true;

        return false;
    }

    void DesenharPeca(Graphics g, Peca peca, int linha, int coluna)
    {
        using var fonte = new Font("Segoe UI Symbol", 44);
        using var formato = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        var area = new RectangleF(
            coluna * TamanhoCasa, linha * TamanhoCasa, TamanhoCasa, TamanhoCasa);

        if (peca.Cor == Cor.Branca)
        {
            int[,] deslocamentos = { { -2, 0 }, { 2, 0 }, { 0, -2 }, { 0, 2 } };
            for (int i = 0; i < 4; i++)
            {
                var areaContorno = new RectangleF(
                    area.X + deslocamentos[i, 0], area.Y + deslocamentos[i, 1],
                    area.Width, area.Height);
                g.DrawString(peca.Simbolo, fonte, Brushes.Black, areaContorno, formato);
            }
            g.DrawString(peca.Simbolo, fonte, Brushes.White, area, formato);
        }
        else
        {
            g.DrawString(peca.Simbolo, fonte, Brushes.Black, area, formato);
        }
    }
}