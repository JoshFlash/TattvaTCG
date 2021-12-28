public class DamageSpell
{
    public string Name;
    public int Damage;

    public void Cast(ICharacter target)
    {
        BattleActions.DamageCharacter(target, Damage);
    }
}