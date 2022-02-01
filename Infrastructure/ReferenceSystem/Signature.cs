using System.Collections.ObjectModel;
using System.Security.Cryptography;

namespace ProjectBank.Infrastructure
{
    public class Signature
    {
        public int Id {get; set;}

        [Required]
        public IReadOnlyCollection<string> Hashes;

        public Signature(IReadOnlyCollection<Tag> tags)
        {
            if(tags.Count == 0) {throw new ArgumentException("Cannot initialize a signature with an empty list of Tag.");}
            Hashes = GetHashedTags(tags);
        }
        public Signature() {}

        private static IReadOnlyCollection<string> GetHashedTags(IReadOnlyCollection<Tag> tags)
        {
            string minSHA1 = string.Empty;
            string minSHA256 = string.Empty;
            string minMD5 = string.Empty;
            string minSHA384 = string.Empty;
            string minSHA512 = string.Empty;
            string minNoHash = string.Empty;

            foreach (Tag tag in tags)
            {
                var SHA1Value = HashBuilder.HashString(tag.Name, SHA1.Create());
                if (minSHA1 == string.Empty || minSHA1.CompareTo(SHA1Value) > 0) minSHA1 = SHA1Value;
                
                var SHA256Value = HashBuilder.HashString(tag.Name, SHA256.Create());
                if (minSHA256 == string.Empty || minSHA256.CompareTo(SHA256Value) > 0) minSHA256 = SHA256Value;

                var MD5Value = HashBuilder.HashString(tag.Name, MD5.Create());
                if (minMD5 == string.Empty || minMD5.CompareTo(MD5Value) > 0) minMD5 = MD5Value;

                var SHA384Value = HashBuilder.HashString(tag.Name, SHA384.Create());
                if (minSHA384 == string.Empty || minSHA384.CompareTo(SHA384Value) > 0) minSHA384 = SHA384Value;

                var SHA512Value = HashBuilder.HashString(tag.Name, SHA512.Create());
                if (minSHA512 == string.Empty || minSHA512.CompareTo(SHA512Value) > 0) minSHA512 = SHA512Value;

                var NoHash = tag.Name;
                if (minNoHash == string.Empty || minNoHash.CompareTo(NoHash) > 0) minNoHash = NoHash;
            }

            return new ReadOnlyCollection<string>(new List<string> { minSHA1, minSHA256, minMD5, minSHA384, minSHA512, minNoHash});

        }
    }
}
