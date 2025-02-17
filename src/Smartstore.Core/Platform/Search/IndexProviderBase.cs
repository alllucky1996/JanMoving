﻿namespace Smartstore.Core.Search
{
    public abstract class IndexProviderBase : IIndexProvider
    {
        public virtual bool IsActive(string scope)
        {
            return true;
        }

        public abstract Task<IEnumerable<string>> EnumerateIndexesAsync();

        public virtual IIndexDocument CreateDocument(int id, SearchDocumentType? documentType)
        {
            Guard.IsPositive(id, nameof(id));

            return new IndexDocument(id, documentType);
        }

        public abstract IIndexStore GetIndexStore(string scope);

        public abstract ISearchEngine GetSearchEngine(IIndexStore store, ISearchQuery query);
    }
}
