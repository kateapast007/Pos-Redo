using System.Globalization;

namespace Pos.Domain.Common;

public sealed record Score
{
    public decimal Value { get; }

    public Score(decimal value)
    {
        if (value < 0) throw new DomainException("Балл не может быть отрицательным.");
        Value = value;
    }

    public override string ToString() => Value.ToString("0.##", CultureInfo.InvariantCulture);
}
