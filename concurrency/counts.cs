// the universal counter used by tasks
class Counts{
    public long Neg, Pos, Zero;

    public void Add(int n){
        if (n < 0) Neg ++;
        else if (n > 0) Pos++;
        else Zero++;
    }

    public void Merge(Counts o){
        Neg += o.Neg;
        Pos += o.Pos;
        Zero += o.Zero;
    }

    public override string ToString() => $"neg={Neg} pos{Pos} zero={Zero} total={Neg + Pos + Zero}";
}