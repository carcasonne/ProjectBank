using System.Data;
using System.Text;
using ProjectBank.Infrastructure.Entities;

namespace ProjectBank.Infrastructure.ReferenceSystem
{
    public class LocalitySensitiveHashTable<Tagable> 
    where Tagable : ITagable
    {
        private readonly int groupSize = 2;
        private readonly int k = 6;
        public int NumberOfGroups;
        public ConcurrentDictionary<string, Bucket<Tagable>> Map;

        public LocalitySensitiveHashTable()
        {
            NumberOfGroups = k / groupSize;
            Map = new ConcurrentDictionary<string, Bucket<Tagable>>();
        }

        private void AddSignature(string bucketString)
        {
            Map[bucketString] = new Bucket<Tagable>();
        }

        public virtual async Task<Response> Insert(Tagable tagable)
        {
            if (tagable.Tags.Count == 0) { return Response.Conflict; }
            var bucketStrings = HashesToBucketString(tagable.Signature);
            await foreach (string bucketString in bucketStrings.ToAsyncEnumerable())
            {
                if (!Map.ContainsKey(bucketString)) AddSignature(bucketString);
                if (Map[bucketString].Projects.Where(x => x.Id == tagable.Id).ToList().Count != 0 || !Map[bucketString].Projects.Add(tagable)) { return Response.Conflict;; }
            }
            return Response.Created;
        }

        public async Task<Response> Update(Tagable tagable)
        {
            if(Delete(tagable) != Response.Deleted) return Response.NotFound;
            return await Insert(tagable);
        }

        public Response Delete(Tagable tagable)
        {
            var bucketStrings = HashesToBucketString(tagable.Signature);
            foreach (string bucketString in bucketStrings)
            {
                if(!Map.ContainsKey(bucketString)) return Response.NotFound;
                var toRemove = Map[bucketString].Projects.Where(x => x.Id == tagable.Id).ToList();
                if (!Map[bucketString].Projects.Remove(toRemove.FirstOrDefault())) return Response.NotFound;
            }
            return Response.Deleted;
        }


        public async Task<IEnumerable<Tagable>> Get(Tagable tagable)
        {
            var set = new HashSet<Tagable>();
            var bucketStrings = HashesToBucketString(tagable.Signature);
            await foreach (string bucketString in bucketStrings.ToAsyncEnumerable())
            {
                foreach (Tagable relatedTagable in Map[bucketString].Projects)
                {
                    if (!relatedTagable.Id.Equals(tagable.Id)) set.Add(relatedTagable);
                }
            }
            return set.AsEnumerable();
        }
        

        public string[] HashesToBucketString(Signature signature)
        {

            string[] bucketStrings = new string[NumberOfGroups];
            for (int i = 0; i < NumberOfGroups; i++)
            {
                StringBuilder bucketStringBuilder = new StringBuilder();

                for (int j = 0; j < groupSize; j++)
                {
                    bucketStringBuilder.Append(signature.Hashes.ElementAt(i * groupSize + j));
                }

                bucketStrings[i] = bucketStringBuilder.ToString();
            }
            return bucketStrings;
        }
    }
}