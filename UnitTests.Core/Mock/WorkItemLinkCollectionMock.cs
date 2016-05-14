﻿using System;
using System.Collections;
using System.Collections.Generic;

using Aggregator.Core.Interfaces;
using System.Linq;

namespace UnitTests.Core.Mock
{
    internal class WorkItemLinkCollectionMock : IWorkItemLinkCollection
    {
        private readonly List<IWorkItemLink> links = new List<IWorkItemLink>();
        private readonly IWorkItemRepository store;

        public WorkItemLinkCollectionMock(IWorkItemRepository repository)
        {
            this.store = repository;
        }

        public bool Contains(IWorkItemLink link)
        {
            return this.links.Contains(link);
        }

        public void Add(IWorkItemLink link)
        {
            this.links.Add(link);
        }

        public IEnumerator<IWorkItemLink> GetEnumerator()
        {
            return this.links.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.links.GetEnumerator();
        }

        public void Remove(IWorkItemLink link)
        {
            this.links.Remove(link);
        }

        public IEnumerable<IWorkItemLink> Filter(string linkTypeName)
        {
            // be kind and accept lousy references to a link type
            var workItemLinkType = this.store.WorkItemLinkTypes.FirstOrDefault(
                    t => new string[] { t.ReferenceName, t.ForwardEndImmutableName, t.ForwardEndName, t.ReverseEndImmutableName, t.ReverseEndName }
                        .Contains(linkTypeName, StringComparer.OrdinalIgnoreCase));
            if (workItemLinkType == null)
            {
                throw new ArgumentOutOfRangeException(nameof(linkTypeName));
            }

            foreach (IWorkItemLink link in this)
            {
                if (link.LinkTypeEndImmutableName == workItemLinkType.ForwardEndImmutableName
                    || link.LinkTypeEndImmutableName == workItemLinkType.ReverseEndImmutableName)
                {
                    yield return link;
                }
            }
        }
    }
}
