public struct DamageParams
{
    public float ammount;
    public float critChance;
    public Team team;

    public DamageParams(float ammount, float critChance, Team team)
    {
        this.ammount = ammount;
        this.critChance = critChance;
        this.team = team;
    }
}
