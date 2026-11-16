using System.Globalization;
using Pos.Domain.Common;

namespace Pos.Domain.Grading;

public sealed record FinalGrade
{
    public const decimal MaxValue = 10m;

    public decimal Value { get; }

    public FinalGrade(decimal value)
    {
        if (value < 0 || value > MaxValue) throw new DomainException($"Итог должен быть в диапазоне от 0 до {MaxValue}.");
        Value = value;
    }

    public override string ToString() => Value.ToString("0.##", CultureInfo.InvariantCulture);
}
