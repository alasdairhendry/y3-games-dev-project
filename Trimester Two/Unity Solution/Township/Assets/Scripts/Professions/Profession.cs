
public class Profession
{
    public ProfessionType type { get; protected set; }
    public int minimumAge { get; protected set; }
    public int maximumAge { get; protected set; }

    public Profession (ProfessionType type, int minimumAge = 6, int maximumAge = 70)
    {
        this.type = type;
        this.minimumAge = minimumAge;
        this.maximumAge = maximumAge;
    }
}
