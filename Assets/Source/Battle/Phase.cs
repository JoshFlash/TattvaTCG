using System;

public record Phase
{
    private const int MAX = 4;

    public static readonly Phase Prep = new Phase(1, "Prep");
    public static readonly Phase Spell = new Phase(2, "Spell");
    public static readonly Phase Ability = new Phase(3, "Ability");
    public static readonly Phase Recovery = new Phase(4, "Recovery");

    private Phase(int value, string name)
    {
        Value = value;
        Name = name;
    }

    public int Value { get; }
    public string Name { get; }

    public static implicit operator Phase(int value)
    {
        value = (value - 1) % MAX + 1;
        switch (value)
        {
            case 1:
                return Prep;
            case 2:
                return Spell;
            case 3:
                return Ability;
            case 4:
                return Recovery;
            default:
                throw new ArgumentException("Invalid enum value");
        }
    }

    public static implicit operator int(Phase phase) => phase.Value;

    public override string ToString() => Name;

    public Phase Next()
    {
        switch (Value)
        {
            case 1:
                return Spell;
            case 2:
                return Ability;
            case 3:
                return Recovery;
            case 4:
                return Prep;
            default:
                throw new InvalidOperationException("Invalid enumeration value");
        }
    }
}