namespace Xadrez;

public enum Cor { Branca, Preta }

public enum TipoPeca { Peao, Torre, Cavalo, Bispo, Rainha, Rei }

public class Peca
{
    public TipoPeca Tipo { get; }
    public Cor Cor { get; }

    public Peca(TipoPeca tipo, Cor cor)
    {
        Tipo = tipo;
        Cor = cor;
    }

    public string Simbolo => Tipo switch
    {
        TipoPeca.Rei => "♚",
        TipoPeca.Rainha => "♛",
        TipoPeca.Torre => "♜",
        TipoPeca.Bispo => "♝",
        TipoPeca.Cavalo => "♞",
        TipoPeca.Peao => "♟",
        _ => "?"
    };
}