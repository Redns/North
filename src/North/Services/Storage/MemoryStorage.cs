namespace North.Services.Storage
{
    public class MemoryStorage<T> : IStorage<T>
    {
        private readonly List<T> _items;

        public MemoryStorage()
        {
            _items = new List<T>();
        }

        public MemoryStorage(List<T> items)
        {
            _items = items;
        }

        public void Add(T item)
        {
            _items.Add(item);
        }

        public Task AddAsync(T item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            _items.Clear();
        }

        public Task ClearAsync()
        {
            throw new NotImplementedException();
        }

        public T? Find(Predicate<T> predicate)
        {
            return _items.Find(predicate);
        }

        public Task<T?> FindAsync(Predicate<T> predicate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Get(Predicate<T>? predicate = null)
        {
            if(predicate is null)
            {
                return _items;
            }
            return _items.FindAll(predicate);
        }

        public Task<IEnumerable<T>> GetAsync(Predicate<T> predicate)
        {
            throw new NotImplementedException();
        }

        public void Remove(T item)
        {
            _items.Remove(item);
        }

        public Task RemoveAsync(T item)
        {
            throw new NotImplementedException();
        }

        public void Update(T item)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(T item)
        {
            throw new NotImplementedException();
        }
    }
}
