﻿using System;
using System.Linq;
using System.Linq.Expressions;
using Couchbase.N1QL;
using Remotion.Linq;
using Remotion.Linq.Parsing.Structure;

namespace Couchbase.Linq
{
    public sealed class QueryClientQueryable<T> : QueryableBase<T>
    {
        public QueryClientQueryable(IQueryParser queryParser, IQueryExecutor executor)
            : base(queryParser, executor)
        {
        }

        public QueryClientQueryable(IQueryProvider provider)
            : base(provider)
        {
        }

        public QueryClientQueryable(IQueryProvider provider, Expression expression)
            : base(provider, expression)
        {
        }

        internal QueryClientQueryable(IQueryClient queryClient, string bucketName, Uri uri)
            : base(QueryParserHelper.CreateQueryParser(), new QueryClientQueryExecuter(queryClient, bucketName, uri))
        {
        }
    }
}