namespace Dota_Geek.DataTypes
{
    internal static class RankParser
    {
        public static Rank ParseMedal(this int id)
        {
            if (id == 1)
                return Rank.Herald;
            if (id == 2)
                return Rank.Guardian;
            if (id == 3)
                return Rank.Crusader;
            if (id == 4)
                return Rank.Archon;
            if (id == 5)
                return Rank.Legend;
            if (id == 6)
                return Rank.Ancient;
            if (id == 7)
                return Rank.Divine;
            if (id == 8)
                return Rank.Immortal;
            return Rank.Unknown;
        }
    }

    internal enum Rank
    {
        Herald,
        Guardian,
        Crusader,
        Archon,
        Legend,
        Ancient,
        Divine,
        Immortal,
        Unknown
    }
}