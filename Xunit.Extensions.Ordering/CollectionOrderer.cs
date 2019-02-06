﻿using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Extensions.Ordering
{
	public class CollectionOrderer : OrdererBase, ITestCollectionOrderer
	{
		public CollectionOrderer(IMessageSink diagnosticSink)
			: base(diagnosticSink) { }

		public IEnumerable<ITestCollection> OrderTestCollections(IEnumerable<ITestCollection> testCollections)
		{
			int lastOrder = 0;

			foreach (var g in
				testCollections
					.GroupBy(tc => GetOrder(tc))
					.OrderBy(g => g.Key))
			{
				int count = g.Count();
				
				if (count > 1)
				{
					string cols = string.Join(
							"], [",
							g.Select(
								tc => tc.CollectionDefinition != null 
								? tc.DisplayName 
								: TypeNameFromDisplayName(tc)));

					DiagnosticSink.OnMessage(
						new DiagnosticMessage(
							g.Key == 0
								? "Found {0} test collections with unassigned or '0' order [{2}]"
								: "Found {0} duplicate order '{1}' on test collections [{2}]",
							count,
							g.Key,
							cols));
				}

				if (lastOrder < g.Key - 1)
				{
					int lower = lastOrder + 1;
					int upper = g.Key - 1;

					DiagnosticSink.OnMessage(
						new DiagnosticMessage(
							lower == upper
								? "Missing test collection order '{0}'."
								: "Missing test collection order sequence from '{0}' to '{1}'.",
							lower,
							upper));
				}

				lastOrder = g.Key;
			}

			return testCollections.OrderBy(c => GetOrder(c));
		}

		protected virtual int GetOrder(ITestCollection col)
		{
			ITypeInfo type = 
				col.CollectionDefinition 
				?? col.TestAssembly.Assembly.GetType(TypeNameFromDisplayName(col));

			return ExtractOrderFromAttribute(type.GetCustomAttributes(typeof(OrderAttribute)));
		}

		protected virtual string TypeNameFromDisplayName(ITestCollection col)
		{
			return 
				col
					.DisplayName
					.Substring(col.DisplayName.LastIndexOf(' ') + 1);
		} 
	}

}