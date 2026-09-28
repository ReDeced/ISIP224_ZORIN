namespace ISIP224_ZORIN.Models
{
    public class LetterStat
    {
        public char Letter { get; }

        public int Count { get; }

        public LetterStat(char letter, int count)
        {
            Letter = letter;
            Count = count;
        }
    }
}
