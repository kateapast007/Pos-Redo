using System.Globalization;

namespace Pos.Domain.Common;

public sealed record Weight
{
    public decimal Value { get; }

    public Weight(decimal value)
    {
        if (value <= 0) throw new DomainException("Вес критерия должен быть больше нуля.");
        Value = value;
    }

    public override string ToString() => Value.ToString("0.##", CultureInfo.InvariantCulture);
}
